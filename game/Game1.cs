using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

// notes
// - make the padding around highlighted words scale properly


namespace Multitasking
{
    public enum GameState
    {
        MainMenu,
        Settings,
        Tutorial,
        Game,
        GameOver,
    }

    public enum GameDay
    {
        Day0,
        Day1,
        Day2,
        Day3,
        Day4,
        Day5,
        Day6,
        Day7,
        Day8,
        Day9,
        Day10,
        Day11
    }
    
    
    public class Game1 : Game
    {
        // monogame
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // important
        public GameState currentState;
        public GameDay currentDay;
        private TypingGame typingGame;
        private ArcadePlayer player;
        private List<ArcadeEnemy> enemyList;
        public Rectangle typingWindow;
        public Rectangle shooterWindow;

        // assets
        public SpriteFont typingFont;
        public SpriteFont typingFontBold;
        public SpriteFont menuFont;
        public Texture2D squareImg;
        public Texture2D playerImg;
        public Texture2D enemyImg;
        public Texture2D playerBulletImg;
        public Color backgroundColor = new Color(31, 0, 171);

        // numbers
        public int screenWidth;
        public int screenHeight;
        public const int EnemyDist = 70;
        public double timer;
        public const double EnemySpawnTime = 5;

        // input
        public KeyboardState previousKeyboardState;
        public MouseState previousMouseState;

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


            // important
            currentState = GameState.MainMenu;
            currentDay = GameDay.Day0;
            typingGame = new TypingGame();
            player = new ArcadePlayer(playerImg, new Rectangle(3 * (screenWidth / 4), screenHeight - 200, 50, 50), screenWidth, playerBulletImg);
            enemyList = new List<ArcadeEnemy>();
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);

            // numbers
            screenWidth = _graphics.GraphicsDevice.Viewport.Width;
            screenHeight = _graphics.GraphicsDevice.Viewport.Height;
            timer = EnemySpawnTime;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // write code below


            // assets
            typingFont = Content.Load<SpriteFont>("typingFont");
            typingFontBold = Content.Load<SpriteFont>("typingFontBold");
            menuFont = Content.Load<SpriteFont>("menuFont");
            squareImg = Content.Load<Texture2D>("Square");
            playerImg = Content.Load<Texture2D>("Main Ship - Base - Full health");
            enemyImg = Content.Load<Texture2D>("Turtle");
            playerBulletImg = Content.Load<Texture2D>("PlayerBullet");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
            // write code below
            

            // update input states
            KeyboardState swapKeyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            // do something based on the current state
            switch (currentState)
            {
                //Main Menu Code goes here
                case GameState.MainMenu:

                    //Temp code to swap to game
                    if (SingleKeyPress(swapKeyboardState, Keys.Enter))
                    {
                        currentState = GameState.Tutorial;
                        ResetGame();
                    }

                    //Temp code to swap to demo
                    if (SingleKeyPress(swapKeyboardState, Keys.Tab))
                    {
                        currentState = GameState.Settings;
                    }

                    if(SingleKeyPress(swapKeyboardState, Keys.RightControl) && currentDay != GameDay.Day11)
                    {
                        currentDay++;
                    }

                    if(SingleKeyPress(swapKeyboardState, Keys.LeftControl) && currentDay != GameDay.Day0)
                    {
                        currentDay--;
                    }

                    break;

                // Settings code goes here
                case GameState.Settings:
                    if(SingleKeyPress(swapKeyboardState, Keys.Escape))
                    {
                        currentState = GameState.MainMenu;
                    }
                    break;
                
                //Typing Tutorial Code goes here
                case GameState.Tutorial:

                    currentState = typingGame.UpdateTypingTutorial(currentState);

                    //Temp code to swap to the main game
                    if (SingleKeyPress(swapKeyboardState, Keys.Enter))
                    {
                        currentState = GameState.Game;
                    }

                    if(SingleKeyPress(swapKeyboardState, Keys.Tab))
                    {
                        currentState = GameState.MainMenu;
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
                            int enemyPos = EnemyDist;
                            ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + i*EnemyDist, 200, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
                            enemyList.Add(newEnemy);
                            
                        }
                    }
                    else if (timer <= 0)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int enemyPos = EnemyDist;
                            ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + i * EnemyDist, 200, 50, 50), screenHeight, screenWidth, player, playerBulletImg);
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
                    if (SingleKeyPress(swapKeyboardState, Keys.Enter))
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
                    if (SingleKeyPress(swapKeyboardState, Keys.Enter))
                    {
                        currentState = GameState.MainMenu;
                    }

                    break;
            }

            previousKeyboardState = swapKeyboardState;
            previousMouseState = mouseState;

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
                case GameState.MainMenu:
                    
                    switch (currentDay)
                    {
                        case GameDay.Day0:
                            _spriteBatch.DrawString(menuFont, "multitasking", new Vector2((screenWidth / 2) - (menuFont.MeasureString("multitasking").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "by.....................omni_absence", new Vector2((screenWidth / 2) - (typingFont.MeasureString("by.....................omni_absence").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "ENTER.........................start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.........................start").X / 2), 500), Color.White);
                            _spriteBatch.DrawString(typingFont, "TAB........................settings", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB........................settings").X / 2), 550), Color.White);
                            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("by.....................omni_absence").X / 2 - 8, 400, (int)typingFont.MeasureString("by.....................omni_absence").X + 16, 35), new Color(255, 255, 255));
                            
                            break;


                        case GameDay.Day1:
                            _spriteBatch.DrawString(menuFont, "day 1", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 1").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "post payday blues", new Vector2((screenWidth / 2) - (typingFont.MeasureString("post payday blues").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.20.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.20.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("post payday blues").X / 2 - 8, 400, (int)typingFont.MeasureString("post payday blues").X + 16, 35), new Color(255, 255, 255));

                            break;


                        case GameDay.Day2:
                            _spriteBatch.DrawString(menuFont, "day 2", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 2").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "???", new Vector2((screenWidth / 2) - (typingFont.MeasureString("???").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.21.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.21.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("???").X / 2 - 8, 400, (int)typingFont.MeasureString("???").X + 16, 35), new Color(255, 255, 255));
                            
                            break;


                        case GameDay.Day3:
                            _spriteBatch.DrawString(menuFont, "day 3", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 3").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "update day", new Vector2((screenWidth / 2) - (typingFont.MeasureString("update day").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.22.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.22.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("update day").X / 2 - 8, 400, (int)typingFont.MeasureString("update day").X + 16, 35), new Color(255, 255, 255));
                            
                            break;


                        case GameDay.Day4:
                            _spriteBatch.DrawString(menuFont, "day 4", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 4").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "???", new Vector2((screenWidth / 2) - (typingFont.MeasureString("???").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.23.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.23.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("???").X / 2 - 8, 400, (int)typingFont.MeasureString("???").X + 16, 35), new Color(255, 255, 255));
                            
                            break;


                        case GameDay.Day5:
                            _spriteBatch.DrawString(menuFont, "day 5", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 5").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "holiday party", new Vector2((screenWidth / 2) - (typingFont.MeasureString("holiday party").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.24.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.24.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("holiday party").X / 2 - 8, 400, (int)typingFont.MeasureString("holiday party").X + 16, 35), new Color(255, 255, 255));
                            
                            break;


                        case GameDay.Day6:
                            _spriteBatch.DrawString(menuFont, "day 6", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 6").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "a blue screen of death christmas", new Vector2((screenWidth / 2) - (typingFont.MeasureString("a blue screen of death christmas").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.25.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.25.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("a blue screen of death christmas").X / 2 - 8, 400, (int)typingFont.MeasureString("a blue screen of death christmas").X + 16, 35), new Color(255, 255, 255));
                            
                            break;


                        case GameDay.Day7:
                            _spriteBatch.DrawString(menuFont, "day 7", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 7").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "prod notes", new Vector2((screenWidth / 2) - (typingFont.MeasureString("prod notes").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.27.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.27.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("prod notes").X / 2 - 8, 400, (int)typingFont.MeasureString("prod notes").X + 16, 35), new Color(255, 255, 255));

                            break;


                        case GameDay.Day8:
                            _spriteBatch.DrawString(menuFont, "DAY 8", new Vector2((screenWidth / 2) - (menuFont.MeasureString("DAY 8").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "CAPS LOCK", new Vector2((screenWidth / 2) - (typingFont.MeasureString("CAPS LOCK").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.28.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.28.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "PRESS ANY KEY TO START", new Vector2((screenWidth / 2) - (typingFont.MeasureString("PRESS ANY KEY TO START").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("CAPS LOCK").X / 2 - 8, 400, (int)typingFont.MeasureString("CAPS LOCK").X + 16, 35), new Color(255, 255, 255));

                            break;


                        case GameDay.Day9:
                            _spriteBatch.DrawString(menuFont, "day 9", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 9").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "???", new Vector2((screenWidth / 2) - (typingFont.MeasureString("???").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.29.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.29.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("???").X / 2 - 8, 400, (int)typingFont.MeasureString("???").X + 16, 35), new Color(255, 255, 255));

                            break;


                        case GameDay.Day10:
                            _spriteBatch.DrawString(menuFont, "day 10", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 10").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "millennium bug", new Vector2((screenWidth / 2) - (typingFont.MeasureString("millennium bug").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.30.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.30.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("millennium bug").X / 2 - 8, 400, (int)typingFont.MeasureString("millennium bug").X + 16, 35), new Color(255, 255, 255));

                            break;


                        case GameDay.Day11:
                            _spriteBatch.DrawString(menuFont, "day 11", new Vector2((screenWidth / 2) - (menuFont.MeasureString("day 11").X / 2), 300), Color.White);
                            _spriteBatch.DrawString(typingFont, "payday", new Vector2((screenWidth / 2) - (typingFont.MeasureString("payday").X / 2), 400), backgroundColor);
                            _spriteBatch.DrawString(typingFont, "12.31.1999", new Vector2((screenWidth / 2) - (typingFont.MeasureString("12.31.1999").X / 2), 450), Color.White);
                            _spriteBatch.DrawString(typingFont, "press any key to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press any key to start").X / 2), 600), Color.White);
                            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("payday").X / 2 - 8, 400, (int)typingFont.MeasureString("payday").X + 16, 35), new Color(255, 255, 255));

                            break;
                    }
                    
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
                                projectile.Draw(_spriteBatch, Color.Red);
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
            return currentKBState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the mouse was clicked once.
        /// </summary>
        /// <param name="currentMouseState"></param>
        /// <returns>Whether the mouse was clicked once or not.</returns>
        private bool SingleLeftClick(MouseState currentMouseState)
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
        }

        private void ResetGame()
        {
            player.IsAlive = true;
            enemyList.Clear();
            player.Projectiles.Clear();
            timer = EnemySpawnTime;
            typingGame = new TypingGame();
        }
    }
}
