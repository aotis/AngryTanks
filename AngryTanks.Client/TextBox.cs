using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.Input;
using log4net;

using AngryTanks.Common;
using AngryTanks.Common.Extensions.StringExtensions;


namespace AngryTanks.Client
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TextBox : DrawableGameComponent, IGameConsole
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler<PromptInputEvent> PromptReceivedInput;

        #region TextBox Properties

        public bool Opened
        {
            get;
            set;
        }

        public Vector2 Position
        {
            get;
            set;
        }

        public Vector2 Size
        {
            get;
            set;
        }

        public Vector2 Margin
        {
            get;
            set;
        }

        public Vector2 Padding
        {
            get;
            set;
        }

        public Color BackgroundColor
        {
            get;
            set;
        }

        public ConsoleMessagePart PromptPrefix
        {
            get;
            set;
        }

        public bool PromptActive
        {
            get;
            set;
        }

        public Int16 PromptBlinkRate
        {
            get;
            set;
        }

        public String PromptCursor
        {
            get;
            set;
        }

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF(Position + Margin, Size - (Margin * 2));
            }
        }

        public RectangleF InnerBounds
        {
            get
            {
                return new RectangleF(Bounds.X + Padding.X, Bounds.Y + Padding.Y,
                                      Bounds.Width - Padding.X, Bounds.Height - Padding.Y);
            }
        }

        #endregion

        private IInputService inputService;

        private SpriteBatch spriteBatch;
        private SpriteFont consoleFont;
        private Texture2D background;

        //private int currentBlinkTime;
        //private bool promptBlinking;

        private String currentPromptInput = "";
        public String returnCurrentPromptInput
        {
            get { return currentPromptInput; }
        }

        private int promptInputPointer = 0;
        private bool promptJustOpened = false;

        private LinkedList<ConsoleMessageLine> lines = new LinkedList<ConsoleMessageLine>();

        public TextBox(Game game, Vector2 position, Vector2 size, Vector2 margin, Vector2 padding, Color backgroundColor)
            : base(game)
        {
            this.Position = position;
            this.Size = size;
            this.Margin = margin;
            this.Padding = padding;
            this.BackgroundColor = backgroundColor;

            this.Opened = true;
            this.PromptPrefix = new ConsoleMessagePart("", Color.Black);
            this.PromptActive = false;
            this.PromptBlinkRate = 0; // 0 ms prompt blink rate

            this.PromptCursor = ""; // no prompt

            inputService = (IInputService)Game.Services.GetService(typeof(IInputService));

            inputService.GetKeyboard().CharacterEntered += CharacterEntered;
            inputService.GetKeyboard().KeyPressed += KeyPressed;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // unregister event handlers
                try
                {
                    inputService.GetKeyboard().CharacterEntered -= CharacterEntered;
                    inputService.GetKeyboard().KeyPressed -= KeyPressed;
                }
                // our input service disposed before we did... a dirty hack
                catch (NullReferenceException e)
                {
                    Log.Error(e.Message);
                    Log.Error(e.StackTrace);
                }

                // remove service
                if (Game.Services != null)
                {
                    IGameConsole gameConsoleService = (IGameConsole)Game.Services.GetService(typeof(IGameConsole));

                    if (ReferenceEquals(gameConsoleService, this))
                        Game.Services.RemoveService(typeof(IGameConsole));
                }
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }
        protected override void LoadContent()
        {
            consoleFont = Game.Content.Load<SpriteFont>("fonts/ConsoleFont12");
            background = new Texture2D(GraphicsDevice, 1, 1);
            background.SetData(new Color[] { Color.Black });

            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Opened)
            {
                base.Draw(gameTime);
                return;
            }
            //blink time not needed
            //currentBlinkTime += gameTime.ElapsedGameTime.Milliseconds;

            Vector2 position = new Vector2(InnerBounds.X, InnerBounds.Y + InnerBounds.Height);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

            spriteBatch.Draw(background,
                             (Rectangle)Bounds,
                             null, BackgroundColor, 0,
                             Vector2.Zero,
                             SpriteEffects.None, 0);

            position.Y -= DrawLine(gameTime, new ConsoleMessageLine(currentPromptInput) ,position);

            foreach (ConsoleMessageLine line in lines)
            {
                // we have no more room left to draw
                if (position.Y < (InnerBounds.Y + consoleFont.MeasureString("qABC!").Y))
                    break;

                position.Y -= DrawLine(gameTime, line, position);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void KeyPressed(Keys key)
        {
            switch (key)
            {
                // backspaces character behind cursor
                case Keys.Back:
                    if (promptInputPointer > 0 && promptInputPointer <= currentPromptInput.Length)
                    {
                        currentPromptInput = currentPromptInput.Remove(promptInputPointer - 1, 1);
                        promptInputPointer--;
                    }

                    break;
                    
                default:
                    break;
            }
        }

        protected void CharacterEntered(char c)
        {
            // we should ignore this key input because it was used to open the prompt
            if (promptJustOpened)
            {
                promptJustOpened = false;
                return;
            }

            // just to make sure we don't royally screw up the insertion...
            if (promptInputPointer < 0)
                promptInputPointer = 0;
            else if (promptInputPointer > currentPromptInput.Length)
                promptInputPointer = currentPromptInput.Length;

            if (PromptActive && c >= (char)32 && c <= (char)126)
            {
                currentPromptInput = currentPromptInput.Insert(promptInputPointer, new String(c, 1));
                promptInputPointer++;
            }
        }

        public virtual Single DrawLine(GameTime gameTime, ConsoleMessageLine line, Vector2 position)
        {
            // decrement our intial position
            Vector2 messageSize = consoleFont.MeasureString("ABC!");
            position.Y -= messageSize.Y / 2; // nuclex's truetype font importer measures from the baseline

            foreach (ConsoleMessagePart part in line.Parts)
            {
                // TODO break lines/words to span across multiple lines if necessary
                spriteBatch.DrawString(consoleFont,
                                       part.Message,
                                       position,
                                       part.Color, 0,
                                       Vector2.Zero,
                                       1, SpriteEffects.None, 1);

                position.X += consoleFont.MeasureString(part.Message).X;
            }

            return messageSize.Y;
        }

        public void WriteLine(String message)
        {
            WriteLine(new ConsoleMessageLine(message));
        }

        public void WriteLine(String message, Color color)
        {
            WriteLine(new ConsoleMessageLine(message, color));
        }

        public void WriteLine(ConsoleMessageLine consoleMessage)
        {
            lines.AddFirst(consoleMessage);
        }

    }
}