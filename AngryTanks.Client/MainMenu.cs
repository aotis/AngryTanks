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


namespace AngryTanks.Client
{
    /// <summary>
    /// This is a class that inherits from the Nuclex Gamestate.
    /// </summary>
    public class MainMenu : GameState
    {
        private Game game; // needed to load textures
        private Texture2D bg, join, help, quit; // textures for buttons and background
        GameStateManager gameStateManager; // stores the gameStateManager used in AngryTanks
                                            // this allows switching to new gamestates from within a gamestate
        GuiManager gui;
        private GraphicsDevice graphicsDevice; // needed for spriteBatch
        private SpriteBatch spriteBatch; // passing spriteBatch from AngryTanks didn't work
        private KeyboardState keyboardState, oldKeyboardState;

        private enum buttons { join, help, quit } // an enum for each of the button selections
        private int highlightedButton; // to be used as an index for the enum buttons
        private Color[] buttonColor; // using an array instead of List for direct access

        private Screen mainMenuScreen; // screen to be used by the guimanager for this object

        public MainMenu(Game game, GameStateManager gameStateManager, GuiManager gui)
            : base(gameStateManager)
        {
            // load textures
            // this can probably go in OnEntered()
            bg = game.Content.Load<Texture2D>("textures/menu/greentank");
            join = game.Content.Load<Texture2D>("textures/menu/join");
            help = game.Content.Load<Texture2D>("textures/menu/help");
            quit = game.Content.Load<Texture2D>("textures/menu/quit");

            // stays in constructor
            this.gameStateManager = gameStateManager;
            this.game = game;
            this.gui = gui;

            // the following code can probably go in OnEntered() as well
            graphicsDevice = game.GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            highlightedButton = (int)buttons.join;

            buttonColor = new Color[3];
            buttonColor[0] = Color.Yellow;
            buttonColor[1] = Color.White;
            buttonColor[2] = Color.White;

            Viewport viewport = game.GraphicsDevice.Viewport;
            mainMenuScreen = new Screen(viewport.Width, viewport.Height);

            gui.Screen = mainMenuScreen;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            
            // I'm still not familuar with delagates, so I didn't use them
            // changes the selected button using the arrow keys
            if (keyboardState.IsKeyUp(Keys.Down) && oldKeyboardState.IsKeyDown(Keys.Down))
                changeHighlightedButton(1);
            else if(keyboardState.IsKeyUp(Keys.Up) && oldKeyboardState.IsKeyDown(Keys.Up))
                changeHighlightedButton(-1);

            // when enter is pressed, the gamestate changes to the selected option
            if (keyboardState.IsKeyUp(Keys.Enter) && oldKeyboardState.IsKeyDown(Keys.Enter))
            {
                if (highlightedButton == (int)buttons.join)
                    gameStateManager.Switch(new JoinMenu(game, gameStateManager, gui));
                if (highlightedButton == (int)buttons.help)
                    // TODO: switch to help screen
                // also exits the game
                if(highlightedButton == (int)buttons.quit)
                    game.Exit();
            }

            oldKeyboardState = keyboardState;

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
            spriteBatch.Draw(join,
                            new Vector2(game.Window.ClientBounds.Width * .75f, (game.Window.ClientBounds.Height * .75f) -
                                                                                                    join.Height),
                            null,
                            buttonColor[(int)buttons.join],
                            0.0f,
                            new Vector2(join.Width / 2, join.Height / 2),
                            1.0f,
                            SpriteEffects.None,
                            0.0f);
            spriteBatch.Draw(help,
                            new Vector2(game.Window.ClientBounds.Width * .75f, (game.Window.ClientBounds.Height * .75f)),
                            null,
                            buttonColor[(int)buttons.help],
                            0.0f,
                            new Vector2(help.Width / 2, help.Height / 2),
                            1.0f,
                            SpriteEffects.None,
                            0.0f);
            spriteBatch.Draw(quit,
                            new Vector2(game.Window.ClientBounds.Width * .75f, (game.Window.ClientBounds.Height * .75f) +
                                                                                                    26),
                            null,
                            buttonColor[(int)buttons.quit],
                            0.0f,
                            new Vector2(quit.Width / 2, quit.Height / 2),
                            1.0f,
                            SpriteEffects.None,
                            0.0f);
            spriteBatch.End();
        }

        protected override void OnEntered() { }

        protected override void OnLeaving() { }

        protected override void OnPause() { }

        protected override void OnResume() { }

        // updates members in array with correct tint
        protected void changeHighlightedButton(int direction)
        {
            buttonColor[highlightedButton] = Color.White;
                highlightedButton += direction;
                if (highlightedButton > (int)buttons.quit)
                    highlightedButton = (int)buttons.join;
                else if (highlightedButton < (int)buttons.join)
                    highlightedButton = (int)buttons.quit;
            buttonColor[highlightedButton] = Color.Yellow;
        }
    }
}