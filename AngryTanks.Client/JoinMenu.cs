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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class JoinMenu : GameState
    {
        private Game game; // needed to load textures
        GameStateManager gameStateManager;  // stores the gameStateManager used in AngryTanks
                                            // this allows switching to new gamestates from within a gamestate
        GuiManager gui;
        InputManager input;
        GameServiceContainer gameService;
        Texture2D bg;

        private SpriteBatch spriteBatch; // passing spriteBatch from AngryTanks didn't work
        KeyboardState keyboardState, oldKeyboardState;
        
        private Screen joinScreen;
        private LabelControl callsignLabel, tagLabel, hostLabel, teamLabel;
        private ButtonControl joinButton, backButton;
        private InputControl callsignInput, tagInput, hostInput;
        private ChoiceControl teamChoice;


        public JoinMenu(Game game, GameStateManager gameStateManager, GuiManager gui, InputManager input)
            : base(gameStateManager)
        {
            this.game = game;
            this.gameStateManager = gameStateManager;
            this.gameService = new GameServiceContainer();
            this.gui = gui;
            this.input = input;

            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            bg = game.Content.Load<Texture2D>("textures/menu/ScoutTank");
            
            Viewport viewport = game.GraphicsDevice.Viewport;
            joinScreen = new Screen(viewport.Width, viewport.Height);
            gui.Screen = joinScreen;

            joinScreen.Desktop.Bounds = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(0.0f, 0.0f),
                                                        new UniScalar(1.0f, 0.0f), new UniScalar(1.0f, 0.0f));
            InitializeComponents();
        }
        public override void Initialize()
        {
            
            

            base.Initialize();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyUp(Keys.Escape) && oldKeyboardState.IsKeyDown(Keys.Escape))
                gameStateManager.Switch(new MainMenu(game, gameStateManager, gui, input));

           

            oldKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            spriteBatch.Draw(bg,
                            new Vector2(bg.Width / 2, game.Window.ClientBounds.Height / 2),
                            null,
                            Color.White,
                            0.0f,
                            new Vector2(bg.Width / 2, bg.Height / 2),
                            1.0f,
                            SpriteEffects.None,
                            1.0f);
            spriteBatch.End();
        }

        protected override void OnEntered() { }

        protected override void OnLeaving() {
            joinScreen.Desktop.Children.Remove(callsignLabel);
            joinScreen.Desktop.Children.Remove(callsignInput);
            //joinScreen.Desktop.Children.Remove(joinButton);

        }

        protected override void OnPause() { }

        protected override void OnResume() { }

        protected void InitializeComponents()
        {
            this.callsignLabel = new LabelControl();
            this.tagLabel = new LabelControl();
            this.hostLabel = new LabelControl();
            this.teamLabel = new LabelControl();

            this.callsignInput = new InputControl();
            this.tagInput = new InputControl();
            this.hostInput = new InputControl();

            this.joinButton = new ButtonControl();
            this.backButton = new ButtonControl();
            this.teamChoice = new ChoiceControl();

            this.callsignLabel.Text = "Callsign: ";
            this.callsignLabel.Bounds = new UniRectangle(new UniScalar(.7f, 0f), new UniScalar(.2f, 0f), 50, 24);

            this.callsignInput.Bounds = new UniRectangle(new UniScalar(.7f, 80f), new UniScalar(0.2f, 0f), 150, 24);
            this.callsignInput.Enabled = true;
            
            this.tagLabel.Text = "Tag: ";
            this.tagLabel.Bounds = new UniRectangle(new UniScalar(0.7f, 0f), new UniScalar(0.3f, 0f), 50, 24);

            this.tagInput.Bounds = new UniRectangle(new UniScalar(0.7f, 80f), new UniScalar(0.3f, 0f), 150, 24);
            this.tagInput.Enabled = true;

            this.hostLabel.Text = "Host: ";
            this.hostLabel.Bounds = new UniRectangle(new UniScalar(0.7f, 0f), new UniScalar(0.4f, 0f), 50, 24);

            this.hostInput.Bounds = new UniRectangle(new UniScalar(0.7f, 80f), new UniScalar(0.4f, 0f), 150, 24);
            this.hostInput.Enabled = true;

            this.teamLabel.Text = "Team: ";
            this.teamLabel.Bounds = new UniRectangle(new UniScalar(0.7f, 0f), new UniScalar(0.5f, 0f), 50, 24);


            this.joinButton.Text = "Join!";
            this.joinButton.Bounds = new UniRectangle(new UniScalar(.7f, 80f), new UniScalar(0.7f, 0f), 150, 72);
            // 80, 24 for regular button

            this.backButton.Text = "Back";
            this.backButton.Bounds = new UniRectangle(new UniScalar(0.2f, 0.0f), new UniScalar(0.9f, 0f), 80, 24);

            joinScreen.Desktop.Children.Add(callsignLabel);
            joinScreen.Desktop.Children.Add(callsignInput);
            joinScreen.Desktop.Children.Add(tagLabel);
            joinScreen.Desktop.Children.Add(tagInput);
            joinScreen.Desktop.Children.Add(hostLabel);
            joinScreen.Desktop.Children.Add(hostInput);
            joinScreen.Desktop.Children.Add(teamLabel);
            joinScreen.Desktop.Children.Add(joinButton);
            joinScreen.Desktop.Children.Add(backButton);
        }
    }
}