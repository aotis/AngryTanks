using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Nuclex.Game.States;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Input;
using Nuclex.Input;


namespace AngryTanks.Client
{
    /// <summary>
    /// This is a class that inherits from the Nuclex Gamestate.
    /// </summary>
    public class MainMenu : GameState
    {
        private Game game; 
        private Texture2D bg; // textures for buttons and background
        private GameStateManager gameStateManager;  // stores refernece to the GameStateManager used in AngryTanks
                                            // this allows switching to new gamestates from within a gamestate
        private GuiManager gui; // stores reference to the GuiManager
        private InputManager input; // stores reference to the InputManager
        private GraphicsDevice graphicsDevice; // needed for spriteBatch
        private SpriteBatch spriteBatch; // stores reference to SpriteBatch

        private Screen mainMenuScreen; // screen to be used by the guimanager for this object
        private ButtonControl joinButton, helpButton, quitButton; // the buttons

        public MainMenu(Game game, GameStateManager gameStateManager, GuiManager gui, InputManager input)
            : base(gameStateManager)
        {
            // load textures
            // this can probably go in OnEntered()
            bg = game.Content.Load<Texture2D>("textures/menu/greentank");

            // stays in constructor
            this.gameStateManager = gameStateManager;
            this.game = game;
            this.gui = gui;
            this.input = input;

            // the following code can probably go in OnEntered() as well
            graphicsDevice = game.GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            
            Viewport viewport = game.GraphicsDevice.Viewport;
            mainMenuScreen = new Screen(viewport.Width, viewport.Height);

            gui.Screen = mainMenuScreen;
            mainMenuScreen.Desktop.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.0f, 0.0f),
                                                        new UniScalar(1.0f, 0.0f), new UniScalar(1.0f, 0.0f));
            InitializeComponents();

            joinButton.Pressed += new EventHandler(joinButton_Pressed);
            helpButton.Pressed += new EventHandler(helpButton_Pressed);
            quitButton.Pressed += new EventHandler(quitButton_Pressed);
        }

        

        

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bg,
                            new Vector2(game.Window.ClientBounds.Width /2, game.Window.ClientBounds.Height /2),
                            null,
                            Color.White,
                            0.0f,
                            new Vector2(bg.Width /2, bg.Height /2),
                            1.0f,
                            SpriteEffects.None,
                            1.0f);
            spriteBatch.End();
        }

        protected override void OnEntered() { }

        protected override void OnLeaving() { }

        protected override void OnPause() { }

        protected override void OnResume() { }
        
        protected void InitializeComponents()
        {
            joinButton = new ButtonControl();
            helpButton = new ButtonControl();
            quitButton = new ButtonControl();

            joinButton.Text = "Join";
            joinButton.Bounds = new UniRectangle(new UniScalar(0.5f, 0.0f), new UniScalar(0.3f, 0.0f), 240, 48);

            helpButton.Text = "Help/Credits";
            helpButton.Bounds = new UniRectangle(new UniScalar(0.5f, 0.0f), new UniScalar(0.5f, 0.0f), 240, 48);

            quitButton.Text = "Quit";
            quitButton.Bounds = new UniRectangle(new UniScalar(0.5f, 0.0f), new UniScalar(0.7f, 0.0f), 240, 48);

            mainMenuScreen.Desktop.Children.Add(joinButton);
            mainMenuScreen.Desktop.Children.Add(helpButton);
            mainMenuScreen.Desktop.Children.Add(quitButton);
        }
        void joinButton_Pressed(object sender, EventArgs e)
        {
            gameStateManager.Switch(new JoinMenu(game, gameStateManager, gui, input));

        }
        void helpButton_Pressed(object sender, EventArgs e)
        {
            // not implemented yet
            return;
        }

        void quitButton_Pressed(object sender, EventArgs e)
        {
            game.Exit();
        }

    }
}