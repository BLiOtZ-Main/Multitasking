using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Multitasking
{
    public enum GameState
    {
        Menu,
        Tutorial,
        Game,
        GameOver,
        WindowDemo
    }
    
    
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private TypingGame typingGame;

        public SpriteFont arial;
        public int screenWidth;
        public int screenHeight;

        public GameState currentState = GameState.Tutorial;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            base.Initialize();
            // write code below

            typingGame = new TypingGame();

            screenWidth = _graphics.GraphicsDevice.Viewport.Width;
            screenHeight = _graphics.GraphicsDevice.Viewport.Height;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // write code below


            // load textures


            // load fonts
            arial = Content.Load<SpriteFont>("arial");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
            // write code below

            //FSM managing GameStates
            switch (currentState)
            {
                //Main Menu Code goes here
                case GameState.Menu:

                    break;
                
                //Typing Tutorial Code goes here
                case GameState.Tutorial:
                    typingGame.Update();
                    break;

                //Main Game Code goes here
                case GameState.Game:

                    break;
                
                //GameOver Screen Code goes here
                case GameState.GameOver:

                    break;
                
                //Window Demo Code goes here
                case GameState.WindowDemo:

                    break;
            }


            // write code above
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Tan);
            _spriteBatch.Begin();
            // write code below

            //FSM managing GameStates
            switch (currentState)
            {
                //Main Menu Draw Code goes here
                case GameState.Menu:
                    
                    break;
                
                //Typing Tutorial Draw Code goes here
                case GameState.Tutorial:
                    typingGame.Draw(_spriteBatch, arial, screenWidth, screenHeight);
                    break;

                //Main Game Draw Code goes here
                case GameState.Game:

                    break;

                //GameOver Screen Draw Code goes here
                case GameState.GameOver:

                    break;

                //Window Demo Draw Code goes here
                case GameState.WindowDemo:

                    break;
            }

            // write code above
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
