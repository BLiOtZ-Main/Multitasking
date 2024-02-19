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
        WindowDemo1,
        WindowDemo2
    }
    
    
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private TypingGame typingGame;

        public SpriteFont arial;
        public int screenWidth;
        public int screenHeight;

        public GameState currentState = GameState.Menu;
        
        public KeyboardState previousKBState;
        public MouseState previousMouse;

        //Windows
        Rectangle typingWindow;
        Rectangle shooterWindow;

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

            //Temp initilizes the game window sizes
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth/2 - 200, screenHeight - 200);

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
            
            KeyboardState swapKBState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            //FSM managing GameStates
            switch (currentState)
            {
                //Main Menu Code goes here
                case GameState.Menu:


                    //Temp code to swap to game
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.Tutorial;
                    }

                    //Temp code to swap to demo
                    if (SingleKeyPress(swapKBState, Keys.Tab))
                    {
                        currentState = GameState.WindowDemo1;
                    }

                    break;
                
                //Typing Tutorial Code goes here
                case GameState.Tutorial:
                    
                    typingGame.Update();

                    //Temp code to swap to the main game
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.Game;
                    }

                    break;

                //Main Game Code goes here
                case GameState.Game:

                    
                    //Temp code to swap to GameOver
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.GameOver;
                    }

                    break;
                
                //GameOver Screen Code goes here
                case GameState.GameOver:

                    //Temp code to swap back to main menu
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.Menu;
                    }

                    break;
                
                //Window Demo Code goes here
                case GameState.WindowDemo1:
                    //If other window is clicked swap to that window
                    if(shooterWindow.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        //Click Swap
                        //If click then swap to window 1
                        if (SingleLeftClick(mouseState))
                        {
                            currentState = GameState.WindowDemo2;
                        }

                        //Temp code to swap states back to menu
                        if (SingleKeyPress(swapKBState, Keys.Enter))
                        {
                            currentState = GameState.Menu;
                        }

                    }
                    break;

                case GameState.WindowDemo2:
                    //If other window is clicked swap to that window
                    if (typingWindow.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        //Click Swap
                        //If click then swap to window 1
                        if (SingleLeftClick(mouseState))
                        {
                            currentState = GameState.WindowDemo1;
                        }

                        //Temp code to swap states back to menu
                        if (SingleKeyPress(swapKBState, Keys.Enter))
                        {
                            currentState = GameState.Menu;
                        }

                    }
                    break;
            }

            previousKBState = swapKBState;
            previousMouse = mouseState;

            // write code above
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSteelBlue);
            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);
            // write code below

            //FSM managing GameStates
            switch (currentState)
            {
                //Main Menu Draw Code goes here
                case GameState.Menu:

                    _spriteBatch.DrawString(arial, "Multitasking      Main      Menu", new Vector2((screenWidth / 2)- 200, 300), Color.Black);
                    //Demo instructions
                    _spriteBatch.DrawString(arial, "Press   enter   to   Start", new Vector2((screenWidth / 2) - 400, 400), Color.Black);
                    _spriteBatch.DrawString(arial, "Tab   for   Window   Demo", new Vector2((screenWidth / 2) - 400, 500), Color.Black);
                    break;
                
                //Typing Tutorial Draw Code goes here
                case GameState.Tutorial:
                    typingGame.Draw(_spriteBatch, arial, screenWidth, screenHeight);
                    break;

                //Main Game Draw Code goes here
                case GameState.Game:

                    //Temp code to visualize the two game window
                    ShapeBatch.Box(typingWindow, Color.LightGray);
                    ShapeBatch.Box(shooterWindow, Color.Black);

                    //Labels for game windows
                    _spriteBatch.DrawString(arial, "Shooter", new Vector2(1300, 100), Color.White);
                    _spriteBatch.DrawString(arial, "Typing", new Vector2(530, 100), Color.Black);

                    break;

                //GameOver Screen Draw Code goes here
                case GameState.GameOver:

                    _spriteBatch.DrawString(arial, "GAME OVER", new Vector2((screenWidth / 2) - 100, 100), Color.Black);

                    break;

                //Window Demo Draw Code goes here
                case GameState.WindowDemo1:

                    typingWindow = new Rectangle(100, 100, 650, 400);
                    shooterWindow = new Rectangle(1000, 500, 650, 400);

                    ShapeBatch.Box(typingWindow, Color.Red);

                    ShapeBatch.Box(shooterWindow, Color.Black);


                    break;

                case GameState.WindowDemo2:

                    typingWindow = new Rectangle(100, 100, 650, 400);
                    shooterWindow = new Rectangle(1000, 500, 650, 400);

                    ShapeBatch.Box(typingWindow, Color.Black);

                    ShapeBatch.Box(shooterWindow, Color.Red);


                    break;
            }

            // write code above
            ShapeBatch.End();
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        
        /// <summary>
        /// Method to check if a key was press once
        /// probably here temp
        /// needed for testing 
        /// </summary>
        /// <param name="currentKBState"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool SingleKeyPress(KeyboardState currentKBState, Keys key)
        {
            return currentKBState.IsKeyDown(key) && previousKBState.IsKeyUp(key);
        }

        /// <summary>
        /// Methos to check if mouse was clicked once
        /// </summary>
        /// <param name="currentMouseState"></param>
        /// <returns></returns>
        private bool SingleLeftClick(MouseState currentMouseState)
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed;
        }
    }
}
