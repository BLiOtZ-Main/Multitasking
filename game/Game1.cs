using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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

        public SpriteFont typingFont;
        public SpriteFont typingFontBold;
        public SpriteFont menuFont;
        public Texture2D squareImg;
        public Texture2D playerImg;
        public Texture2D enemyImg;
        public Texture2D playerBulletImg;
        public int screenWidth;
        public int screenHeight;
        public double timer;
        public const double EnemySpawnTime = 5;

        public GameState currentState = GameState.Menu;
        
        public KeyboardState previousKBState;
        public MouseState previousMouse;

        public const int enemyDist = 70;

        private List<ArcadeEnemy> enemyList;

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

            player = new ArcadePlayer(playerImg, new Rectangle(3 * (screenWidth / 4), screenHeight - 200, 50, 50), screenWidth, playerBulletImg);


            enemyList = new List<ArcadeEnemy>();

            //Temp initilizes the game window sizes
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth/2 - 200, screenHeight - 200);

            timer = EnemySpawnTime;

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

                        //Resets the game (should make a player.Reset method)
                        //Player Positions should be store ina varible of some kind so that we can reset it here
                        player.IsAlive = true;
                        enemyList.Clear();
                        player.Projectiles.Clear();
                        timer = EnemySpawnTime;
                        //VVVVVV Typing tutortial reset code here VVVVV
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

                    timer -= gameTime.ElapsedGameTime.TotalSeconds;

                    //Creates a new row of enemies every ____ seconds
                    if(enemyList.Count == 0)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int enemyPos = enemyDist;
                            ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + i*enemyDist, 200, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
                            enemyList.Add(newEnemy);
                            
                        }
                    }
                    else if (timer <= 0)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int enemyPos = enemyDist;
                            ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + i * enemyDist, 200, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
                            enemyList.Add(newEnemy);

                        }
                        timer = EnemySpawnTime;
                    }


                    //Enemy collision check
                    foreach (ArcadeEnemy enemy in enemyList)
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
                    
                    //Temp code to swap to GameOver
                    if (SingleKeyPress(swapKBState, Keys.Enter))
                    {
                        currentState = GameState.GameOver;
                    }

                    player.Update(gameTime);
                    
                    //Updates every enemy
                    foreach(ArcadeEnemy e in enemyList)
                    {
                        if (e.IsAlive)
                        {
                            e.Update(gameTime);
                        }
                    }

                    //player collision check
                    foreach(ArcadeEnemy e in enemyList)
                    {
                        foreach (ArcadeProjectile projectile in e.projectiles)
                        {
                            projectile.Update(gameTime);
                            if (projectile.CheckCollision(player))
                            {
                                player.IsAlive = false;
                                projectile.Active = false;
                            }
                        }
                    }

                    //updates the player's projectiles
                    foreach(ArcadeProjectile projectile in player.Projectiles)
                    {
                        if (projectile.Active)
                        {
                            projectile.Update(gameTime);
                        }
                    }

                    //Is enemies reach the player game over
                    foreach (ArcadeEnemy e in enemyList)
                    {
                        if (e.position.Y >= screenHeight-200)
                        {
                            currentState = GameState.GameOver;
                        }
                    }

                    //If the player dies game over
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

                    //draws each player projectile
                    foreach(ArcadeProjectile projectile in player.Projectiles)
                    {
                        if (projectile.Active)
                        {
                            projectile.Draw(_spriteBatch, Color.White);
                        } 
                    }
                    
                    //Draws each enemy
                    foreach(ArcadeEnemy e in enemyList)
                    {
                        if (e.IsAlive)
                        {
                            e.Draw(_spriteBatch, Color.White);
                        }
                    }

                    //Draws each enemy projectile
                    foreach (ArcadeEnemy e in enemyList)
                    {
                        foreach (ArcadeProjectile projectile in e.projectiles)
                        {
                            if (projectile.Active)
                            {
                                projectile.Draw(_spriteBatch, Color.White);
                            }
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
