using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

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
        public Color backgroundColor = new Color(31, 0, 171);

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private TypingGame typingGame;
        private ArcadePlayer player;
        private ArcadeEnemy enemy;
        private ArcadeEnemy enemy1;
        private ArcadeEnemy enemy2;
        private ArcadeEnemy enemy3;
        private ArcadeEnemy enemy4;
        private ArcadeEnemy enemy5;

        public SpriteFont typingFont;
        public SpriteFont typingFontBold;
        public SpriteFont menuFont;
        public Texture2D squareImg;
        public Texture2D playerImg;
        public Texture2D enemyImg;
        public Texture2D playerBulletImg;
        public int screenWidth;
        public int screenHeight;

        public GameState currentState = GameState.Menu;
        
        public KeyboardState previousKBState;
        public MouseState previousMouse;

        public int enemyDist = 20;

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

            currentState = GameState.Menu;
            typingGame = new TypingGame();

            screenWidth = _graphics.GraphicsDevice.Viewport.Width;
            screenHeight = _graphics.GraphicsDevice.Viewport.Height;

            player = new ArcadePlayer(playerImg, new Rectangle(3 * (screenWidth / 4), screenHeight - 200, 100, 100), screenWidth, playerBulletImg);
            enemy = new ArcadeEnemy(enemyImg, new Rectangle(1000, 300, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
            enemy1 = new ArcadeEnemy(enemyImg, new Rectangle(1100, 300, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
            enemy2 = new ArcadeEnemy(enemyImg, new Rectangle(1200, 300, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
            enemy3 = new ArcadeEnemy(enemyImg, new Rectangle(1300, 300, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
            enemy4 = new ArcadeEnemy(enemyImg, new Rectangle(1400, 300, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
            enemy5 = new ArcadeEnemy(enemyImg, new Rectangle(1500, 300, 50, 50), screenHeight, screenWidth, player, playerBulletImg);

            /*
            for (int i = 0; i < 10; i++)
            {
                ArcadeEnemy newEnemy  = new ArcadeEnemy(enemyImg, new Rectangle(1000 + enemyDist, 300, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
                enemyDist += 20;
            }
            */

            //Temp initilizes the game window sizes
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth/2 - 200, screenHeight - 200);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // write code below


            // load textures
            squareImg = Content.Load<Texture2D>("Square");
            playerImg = Content.Load<Texture2D>("Main Ship - Base - Full health");
            enemyImg = Content.Load<Texture2D>("Turtle");
            playerBulletImg = Content.Load<Texture2D>("PlayerBullet");

            // load fonts
            typingFont = Content.Load<SpriteFont>("typingFont");
            typingFontBold = Content.Load<SpriteFont>("typingFontBold");
            menuFont = Content.Load<SpriteFont>("menuFont");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
            // write code below
            
            KeyboardState swapKBState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            Debug.WriteLine("currentState:" + currentState.ToString());

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

                    currentState = typingGame.UpdateTypingTutorial(currentState);

                    //Temp code to swap to the main game
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.Game;
                    }

                    break;

                //Main Game Code goes here
                case GameState.Game:

                    /*
                    foreach (ArcadeEnemy enemy in enemy.EnemyList)
                    {
                        if (enemy.IsAlive)
                        {
                            foreach (ArcadeProjectile projectile in player.Projectiles)
                            {
                                if (projectile.CheckCollision(enemy))
                                {
                                    enemy.IsAlive = false;
                                    projectile.Active = false;
                                }
                            }
                        } 
                    }
                    */
                    //Temp code to swap to GameOver
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.GameOver;
                    }

                    player.Update(gameTime);
                    if (enemy.IsAlive)
                    {
                        enemy.Update(gameTime);
                    }
                    if (enemy1.IsAlive)
                    {
                        enemy1.Update(gameTime);
                    }
                    if (enemy2.IsAlive)
                    {
                        enemy2.Update(gameTime);
                    }
                    if (enemy3.IsAlive)
                    {
                        enemy3.Update(gameTime);
                    }
                    if (enemy4.IsAlive)
                    {
                        enemy4.Update(gameTime);
                    }
                    if (enemy5.IsAlive)
                    {
                        enemy5.Update(gameTime);
                    }
                    foreach (ArcadeProjectile projectile in enemy.projectiles)
                    {
                        projectile.Update(gameTime);
                        if (projectile.CheckCollision(player))
                        {
                            player.IsAlive = false;
                            projectile.Active = false;
                        }
                    }

                    foreach(ArcadeProjectile projectile in player.Projectiles)
                    {
                        if (projectile.Active)
                        {
                            projectile.Update(gameTime);
                        }
                    }

                    if (!player.IsAlive)
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
                    //If mouse is hovered over the other window
                    if(shooterWindow.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        //Click Swap
                        //If click then swap to windows
                        if (SingleLeftClick(mouseState))
                        {
                            currentState = GameState.WindowDemo2;
                        }
                    }

                    //Temp code to swap states back to menu
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.Menu;
                    }

                    break;

                case GameState.WindowDemo2:
                    //If mouse is hovered over the other window
                    if (typingWindow.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        //Click Swap
                        //If click then swap to window
                        if (SingleLeftClick(mouseState))
                        {
                            currentState = GameState.WindowDemo1;
                        }
                    }

                    //Temp code to swap states back to menu
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.Menu;
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
            GraphicsDevice.Clear(backgroundColor);
            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);
            // write code below

            //FSM managing GameStates
            switch (currentState)
            {
                //Main Menu Draw Code goes here
                case GameState.Menu:

                    _spriteBatch.DrawString(menuFont, "multitasking.exe", new Vector2((screenWidth / 2) - (menuFont.MeasureString("multitasking.exe").X / 2), 300), Color.White);
                    //Demo instructions
                    _spriteBatch.DrawString(typingFont, "ENTER.........................start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.........................start").X / 2), 500), Color.White);
                    _spriteBatch.DrawString(typingFont, "TAB.....................window demo", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB.....................window demo").X / 2), 550), Color.White);
                    _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 600), Color.White);
                    break;
                
                //Typing Tutorial Draw Code goes here
                case GameState.Tutorial:
                    typingGame.DrawTypingTutorial(_spriteBatch, typingFont, typingFontBold, screenWidth, screenHeight);
                    break;

                //Main Game Draw Code goes here
                case GameState.Game:

                    //Reset the windows to the right sizes because the demo messes them up otherwise
                    typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
                    shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);

                    //Temp code to visualize the two game window
                    ShapeBatch.Box(typingWindow, Color.LightGray);
                    ShapeBatch.Box(shooterWindow, Color.White);
                    
                    _spriteBatch.DrawString(typingFont, "Typing", new Vector2(530, 100), Color.Black);
                    _spriteBatch.DrawString(typingFont, "Space Game Again TM", new Vector2(1200, 100), Color.Black);

                    // Draws player sprite
                    player.Draw(_spriteBatch, Color.White);
                    foreach(ArcadeProjectile projectile in player.Projectiles)
                    {
                        if (projectile.Active)
                        {
                            projectile.Draw(_spriteBatch, Color.White);
                        } 
                    }
                    if (enemy.IsAlive)
                    {
                        enemy.Draw(_spriteBatch, Color.White);
                    }
                    if (enemy1.IsAlive)
                    {
                        enemy1.Draw(_spriteBatch, Color.White);
                    }
                    if (enemy2.IsAlive)
                    {
                        enemy2.Draw(_spriteBatch, Color.White);
                    }
                    if (enemy3.IsAlive)
                    {
                        enemy3.Draw(_spriteBatch, Color.White);
                    }
                    if (enemy4.IsAlive)
                    {
                        enemy4.Draw(_spriteBatch, Color.White);
                    }
                    if (enemy5.IsAlive)
                    {
                        enemy5.Draw(_spriteBatch, Color.White);
                    }
                    foreach (ArcadeProjectile projectile in enemy.projectiles)
                    {
                        if (projectile.Active)
                        {
                            projectile.Draw(_spriteBatch, Color.White);
                        }
                    }

                    break;

                //GameOver Screen Draw Code goes here
                case GameState.GameOver:

                    _spriteBatch.DrawString(menuFont, "game over", new Vector2((screenWidth / 2) - (menuFont.MeasureString("game over").X / 2), 300), Color.White);
                    _spriteBatch.DrawString(typingFont, "ENTER.....................main menu", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.....................main menu").X / 2), 500), Color.White);
                    _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 550), Color.White);

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
        /// Checks if a key was pressed a single time.
        /// </summary>
        /// <param name="currentKBState">The current keyboard state.</param>
        /// <param name="key">The key to check.</param>
        /// <returns>Whether the given key was pressed a single time.</returns>
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
