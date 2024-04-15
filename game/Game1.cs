using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// notes
// - make the padding around highlighted words scale properly


namespace Multitasking
{
    public enum GameState { MainMenu, Settings, DayStart, ClockIn, Day, GameOver, LeaderBoard, Endless }

    public enum GameDay { Day1, Day2, Day3, Day4, Day5, Day6, Day7, Day8, Day9, Day10, Day11 }
    
    
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
        private int score = 0;
        bool swapRow;

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
        public const double EnemySpawnTime = 4;
        public int time;
        public int score1 = 0;
        public int score2 = 0;
        public int score3 = 0;

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
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 1050;
            _graphics.ApplyChanges();
            base.Initialize();
            // write code below

            // numbers
            screenWidth = _graphics.GraphicsDevice.Viewport.Width;
            screenHeight = _graphics.GraphicsDevice.Viewport.Height;
            timer = EnemySpawnTime;
            time = 0;

            // important
            currentState = GameState.MainMenu;
            currentDay = GameDay.Day1;
            typingGame = new TypingGame();
            player = new ArcadePlayer(playerImg, new Rectangle(3 * (screenWidth / 4), screenHeight - 200, 50, 50), screenWidth, playerBulletImg);
            enemyList = new List<ArcadeEnemy>();
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);
            swapRow = true;
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
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            // do something based on the current state
            switch (currentState)
            {
                // main menu
                case GameState.MainMenu:
                    UpdateMainMenu(currentKeyboardState, currentMouseState);
                    break;

                // settings screen
                case GameState.Settings:
                    UpdateSettings(currentKeyboardState, currentMouseState);
                    break;

                // start of day screen
                case GameState.DayStart:
                    UpdateDayStart(currentKeyboardState, currentMouseState);
                    break;

                // pre day message
                case GameState.ClockIn:
                    UpdateClockIn(currentKeyboardState, currentMouseState);
                    break;

                //day gameplay
                case GameState.Day:
                    UpdateDay(currentKeyboardState, currentMouseState, gameTime);
                    break;

                //endless gameplay
                case GameState.Endless:
                    UpdateDay(currentKeyboardState, currentMouseState, gameTime);
                    break;

                // game over screen
                case GameState.GameOver:
                    UpdateGameOver(currentKeyboardState, currentMouseState);
                    break;

                //leaderboard screen
                case GameState.LeaderBoard:
                    UpdateLeaderBoard(currentKeyboardState, currentMouseState);
                    break;
            }

            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

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
                // main menu
                case GameState.MainMenu:
                    DrawMainMenu(_spriteBatch);
                    break;

                // settings screen
                case GameState.Settings:
                    DrawSettings(_spriteBatch);
                    break;
                
                // start of day screen
                case GameState.DayStart:
                    DrawDayStart(_spriteBatch);
                    break;
                
                // pre day message
                case GameState.ClockIn:
                    DrawClockIn(_spriteBatch);
                    break;

                // gameplay
                case GameState.Day:
                    DrawDay(_spriteBatch);
                    break;

                // endless
                case GameState.Endless:
                    DrawEndless(_spriteBatch);
                    break;

                // game over screen
                case GameState.GameOver:
                    DrawGameOver(_spriteBatch);
                    break;

                // leaderboard screen
                case GameState.LeaderBoard:
                    DrawLeaderBoard(_spriteBatch);
                    break;
            }

            // write code above
            ShapeBatch.End();
            _spriteBatch.End();
            base.Draw(gameTime);
        }


        // UPDATE METHODS
        public void UpdateMainMenu(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            if(SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.DayStart;
                ResetGame();
            }
            if (SingleKeyPress(currentKeyboardState, Keys.LeftShift))
            {
                currentState = GameState.Endless;
                ResetGame();
            }
            if (SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.Settings;
            }
            if (SingleKeyPress(currentKeyboardState, Keys.OemTilde))
            {
                player.GodMode = true;
            }
        }

        public void UpdateSettings(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            if(SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.MainMenu;
            }
        }

        public void UpdateDayStart(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            if(SingleKeyPress(currentKeyboardState, Keys.Space))
            {
                currentState = GameState.ClockIn;
            }
            else if(SingleKeyPress(currentKeyboardState, Keys.RightAlt) && currentDay != GameDay.Day11)
            {
                currentDay++;
            }
            else if(SingleKeyPress(currentKeyboardState, Keys.LeftAlt) && currentDay != GameDay.Day1)
            {
                currentDay--;
            }
        }

        public void UpdateClockIn(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            currentState = typingGame.UpdateTypingTutorial(currentState);

            // debug
            if(SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.Day;
            }
        }

        public void UpdateDay(KeyboardState currentKeyboardState, MouseState currentMouseState, GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            //Switch Statement for anything that changes between the days
            switch (currentDay)
            {
                //Only day 1 and 2 have something different between them so far
                case GameDay.Day1:
                case GameDay.Day3:
                case GameDay.Day4:
                case GameDay.Day5:
                case GameDay.Day6:
                case GameDay.Day7:
                case GameDay.Day8:
                case GameDay.Day9:
                case GameDay.Day10:
                case GameDay.Day11:
                    //Day One Enemy Spawn Logic
                    if (enemyList.Count == 0)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            int enemyPos = EnemyDist;
                            ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + (int)(1.4 * i * EnemyDist), 200, 75, 75), screenHeight, screenWidth, player, playerBulletImg);
                            enemyList.Add(newEnemy);
                        }
                    }
                    else if (timer <= 0)
                    {
                        
                        if (swapRow)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                int enemyPos = EnemyDist;
                                ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1050 + (int)(1.4 * i * EnemyDist), 200, 75, 75), screenHeight, screenWidth, player, playerBulletImg);
                                enemyList.Add(newEnemy);
                            }
                            swapRow = false;
                        }
                        else
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                int enemyPos = EnemyDist;
                                ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + (int)(1.4 * i * EnemyDist), 200, 75, 75), screenHeight, screenWidth, player, playerBulletImg);
                                enemyList.Add(newEnemy);
                            }
                            swapRow = true;
                        }
                        
                        timer = EnemySpawnTime;
                    }


                    break;

                case GameDay.Day2:
                    //Day Two Enemy Spawn Logic
                    if (enemyList.Count == 0)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            int enemyPos = EnemyDist;
                            ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1050 + (int)(1.4 * i * EnemyDist), 200, 75, 75), screenHeight, screenWidth, player, playerBulletImg);
                            enemyList.Add(newEnemy);
                        }
                    }
                    else if (timer <= 0)
                    {

                        if (swapRow)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                int enemyPos = EnemyDist;
                                ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(990 + (int)(i * EnemyDist), 200, 60, 60), screenHeight, screenWidth, player, playerBulletImg);
                                enemyList.Add(newEnemy);
                            }
                            swapRow = false;
                        }
                        else
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                int enemyPos = EnemyDist;
                                ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + (int)(1.4 * i * EnemyDist), 200, 75, 75), screenHeight, screenWidth, player, playerBulletImg);
                                enemyList.Add(newEnemy);
                            }
                            swapRow = true;
                        }

                        timer = EnemySpawnTime;
                    }
                    break;

            }


            //Enemy collision check
            foreach(ArcadeEnemy enemy in enemyList)
            {
                if(enemy.IsAlive)
                {
                    foreach(ArcadeProjectile projectile in player.Projectiles)
                    {
                        if(projectile.CheckCollision(enemy))
                        {
                            enemy.IsAlive = false;
                            projectile.Active = false;
                            score += 10;
                        }
                    }
                }
            }

            //Temp code to swap to GameOver
            if(SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.GameOver;
            }

            player.Update(gameTime);

            //Updates every enemy
            foreach(ArcadeEnemy e in enemyList)
            {
                if(e.IsAlive)
                {
                    e.Update(gameTime);
                }
            }

            //player collision check
            foreach(ArcadeEnemy e in enemyList)
            {
                foreach(ArcadeProjectile projectile in e.projectiles)
                {
                    projectile.Update(gameTime);
                    if(projectile.CheckCollision(player))
                    {
                        player.IsAlive = false;
                        projectile.Active = false;
                    }
                }
            }

            //updates the player's projectiles
            foreach(ArcadeProjectile projectile in player.Projectiles)
            {
                if(projectile.Active)
                {
                    projectile.Update(gameTime);
                }

            }

            //Is enemies reach the player game over
            foreach(ArcadeEnemy e in enemyList)
            {
                if(e.position.Y >= screenHeight - 200)
                {
                    currentState = GameState.GameOver;
                }
            }

            //If the player dies game over
            if(!player.IsAlive)
            {
                currentState = GameState.GameOver;
            }
        }

        public void UpdateEndless(KeyboardState currentKeyboardState, MouseState currentMouseState, GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
            time = gameTime.ElapsedGameTime.Seconds;

            //Creates a new row of enemies every ____ seconds
            if (enemyList.Count == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    int enemyPos = EnemyDist;
                    ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + (int)(1.4 * i * EnemyDist), 200, 75, 75), screenHeight, screenWidth, player, playerBulletImg);
                    enemyList.Add(newEnemy);

                }
            }
            else if (timer <= 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    int enemyPos = EnemyDist;
                    ArcadeEnemy newEnemy = new ArcadeEnemy(enemyImg, new Rectangle(1000 + (int)(1.4 * i * EnemyDist), 200, 75, 75), screenHeight, screenWidth, player, playerBulletImg);
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
                            score += 10;
                        }
                    }
                }
            }

            //Temp code to swap to GameOver
            if (SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.GameOver;
            }

            player.Update(gameTime);

            //Updates every enemy
            foreach (ArcadeEnemy e in enemyList)
            {
                if (e.IsAlive)
                {
                    e.Update(gameTime);
                }
            }

            //player collision check
            foreach (ArcadeEnemy e in enemyList)
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
            foreach (ArcadeProjectile projectile in player.Projectiles)
            {
                if (projectile.Active)
                {
                    projectile.Update(gameTime);
                }

            }

            //Is enemies reach the player game over
            foreach (ArcadeEnemy e in enemyList)
            {
                if (e.position.Y >= screenHeight - 200)
                {
                    currentState = GameState.GameOver;
                }
            }

            //If the player dies game over
            if (!player.IsAlive)
            {
                currentState = GameState.GameOver;
            }
        }



        public void UpdateGameOver(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            if(SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.MainMenu;
            }
            if (SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.LeaderBoard;
            }
        }

        public void UpdateLeaderBoard(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            if (SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.MainMenu;
            }
        }

        // DRAW METHODS
        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            _spriteBatch.DrawString(menuFont, "multitasking", new Vector2((screenWidth / 2) - (menuFont.MeasureString("multitasking").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, "by.....................omni_absence", new Vector2((screenWidth / 2) - (typingFont.MeasureString("by.....................omni_absence").X / 2), 400), backgroundColor);
            _spriteBatch.DrawString(typingFont, "ENTER.........................start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.........................start").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "LEFT SHIFT.............endless mode", new Vector2((screenWidth / 2) - (typingFont.MeasureString("LEFT SHIFT.............endless mode").X / 2), 550), Color.White);
            _spriteBatch.DrawString(typingFont, "TAB........................settings", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB........................settings").X / 2), 600), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 650), Color.White);
            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("by.....................omni_absence").X / 2 - 8, 400, (int)typingFont.MeasureString("by.....................omni_absence").X + 16, 35), new Color(255, 255, 255));

        }

        public void DrawSettings(SpriteBatch spriteBatch)
        {
            _spriteBatch.DrawString(menuFont, "settings", new Vector2((screenWidth / 2) - (menuFont.MeasureString("settings").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, "TAB............................menu", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB............................menu").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 550), Color.White);
        }

        public void DrawDayStart(SpriteBatch spriteBatch)
        {
            switch(currentDay)
            {
                case GameDay.Day1:
                    DrawDayStartManager(spriteBatch, 1, "12.20.1999", "post payday blues");
                    break;
                case GameDay.Day2:
                    DrawDayStartManager(spriteBatch, 2, "12.21.1999", "???");
                    break;
                case GameDay.Day3:
                    DrawDayStartManager(spriteBatch, 3, "12.22.1999", "update day");
                    break;
                case GameDay.Day4:
                    DrawDayStartManager(spriteBatch, 4, "12.23.1999", "millenium bug");
                    break;
                case GameDay.Day5:
                    DrawDayStartManager(spriteBatch, 5, "12.24.1999", "holiday party");
                    break;
                case GameDay.Day6:
                    DrawDayStartManager(spriteBatch, 6, "12.25.1999", "a blue screen of death christmas");
                    break;
                case GameDay.Day7:
                    DrawDayStartManager(spriteBatch, 7, "12.27.1999", "prod notes");
                    break;
                case GameDay.Day8:
                    DrawDayStartManager(spriteBatch, 8, "12.28.1999", "CAPS LOCK");
                    break;
                case GameDay.Day9:
                    DrawDayStartManager(spriteBatch, 9, "12.29.1999", "???");
                    break;
                case GameDay.Day10:
                    DrawDayStartManager(spriteBatch, 10, "12.30.1999", "???");
                    break;
                case GameDay.Day11:
                    DrawDayStartManager(spriteBatch, 11, "12.31.1999", "payday");
                    break;
            }
        }

        public void DrawDayStartManager(SpriteBatch spriteBatch, int dayNumber, String date, String dayName)
        {
            _spriteBatch.DrawString(menuFont, "day " + dayNumber, new Vector2((screenWidth / 2) - (menuFont.MeasureString("day " + dayNumber).X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, dayName, new Vector2((screenWidth / 2) - (typingFont.MeasureString(dayName).X / 2), 400), backgroundColor);
            _spriteBatch.DrawString(typingFont, date, new Vector2((screenWidth / 2) - (typingFont.MeasureString(date).X / 2), 450), Color.White);
            _spriteBatch.DrawString(typingFont, "press SPACE to start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("press SPACE to start").X / 2), 600), Color.White);
            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString(dayName).X / 2 - 8, 400, (int)typingFont.MeasureString(dayName).X + 16, 35), new Color(255, 255, 255));
        }

        public void DrawClockIn(SpriteBatch spriteBatch)
        {
            _spriteBatch.DrawString(menuFont, "type the following", new Vector2((screenWidth / 2) - (menuFont.MeasureString("type the following").X / 2), 25), Color.White);
            typingGame.DrawTypingTutorial(_spriteBatch, typingFont, typingFontBold, screenWidth, screenHeight);
        }

        public void DrawDay(SpriteBatch spriteBatch)
        {
            //Reset the windows to the right sizes because the demo messes them up otherwise
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);

            //Temp code to visualize the two game window
            ShapeBatch.Box(typingWindow, Color.LightGray);
            ShapeBatch.Box(shooterWindow, Color.White);

            _spriteBatch.DrawString(typingFont, "Typing", new Vector2(530, 100), Color.Black);
            _spriteBatch.DrawString(typingFont, "Space Game Again TM", new Vector2(1200, 100), Color.Black);
            _spriteBatch.DrawString(typingFont, $"Score: {score}", new Vector2(1000, 100), Color.Blue);
            

            // Draws player sprite
            player.Draw(_spriteBatch, Color.White);

            //draws each player projectile
            foreach(ArcadeProjectile projectile in player.Projectiles)
            {
                if(projectile.Active)
                {
                    projectile.Draw(_spriteBatch, Color.White);
                }
            }

            //Draws each enemy
            foreach(ArcadeEnemy e in enemyList)
            {
                if(e.IsAlive)
                {
                    e.Draw(_spriteBatch, Color.White);
                }

            }

            //Draws each enemy projectile
            foreach(ArcadeEnemy e in enemyList)
            {
                foreach(ArcadeProjectile projectile in e.projectiles)
                {
                    if(projectile.Active)
                    {
                        projectile.Draw(_spriteBatch, Color.Red);
                    }
                }
            }
        }

        public void DrawEndless(SpriteBatch spriteBatch)
        {
            //Reset the windows to the right sizes because the demo messes them up otherwise
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);

            //time

            //Temp code to visualize the two game window
            ShapeBatch.Box(typingWindow, Color.LightGray);
            ShapeBatch.Box(shooterWindow, Color.White);

            _spriteBatch.DrawString(typingFont, "Typing", new Vector2(530, 100), Color.Black);
            _spriteBatch.DrawString(typingFont, "Space Game Again TM", new Vector2(1200, 100), Color.Black);
            _spriteBatch.DrawString(typingFont, $"Score: {score}", new Vector2(1000, 100), Color.Blue);
            _spriteBatch.DrawString(typingFont, $"Time: {time}", new Vector2(1550, 100), Color.Blue);

            // Draws player sprite
            player.Draw(_spriteBatch, Color.White);

            //draws each player projectile
            foreach (ArcadeProjectile projectile in player.Projectiles)
            {
                if (projectile.Active)
                {
                    projectile.Draw(_spriteBatch, Color.White);
                }
            }

            //Draws each enemy
            foreach (ArcadeEnemy e in enemyList)
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
        }

        public void DrawGameOver(SpriteBatch spriteBatch)
        {
            _spriteBatch.DrawString(menuFont, "game over", new Vector2((screenWidth / 2) - (menuFont.MeasureString("game over").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, "ENTER..........................menu", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.....................main menu").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "TAB.....................leaderboard", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB.....................leaderboard").X / 2), 550), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 600), Color.White);
        }

        //placeholder for what leaderboard might look like when finished
        public void DrawLeaderBoard(SpriteBatch spriteBatch)
        {
            if (score > score1)
            {
                score3 = score2;
                score2 = score1;
                score1 = score;
            }
            else if (score > score2 && score <= score1)
            {
                score3 = score2;
                score2 = score;
            }
            else if (score > score3 && score < score1 && score <= score2)
            {
                score3 = score;
            }
            _spriteBatch.DrawString(menuFont, "leaderboard", new Vector2((screenWidth / 2) - (menuFont.MeasureString("leaderboard").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, "FIRST........................." + score1, new Vector2((screenWidth / 2) - (typingFont.MeasureString("FIRST.........................").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "SECOND........................" + score2, new Vector2((screenWidth / 2) - (typingFont.MeasureString("SECOND........................").X / 2), 550), Color.White);
            _spriteBatch.DrawString(typingFont, "THIRD........................." + score3, new Vector2((screenWidth / 2) - (typingFont.MeasureString("THIRD.........................").X / 2), 600), Color.White);
            _spriteBatch.DrawString(typingFont, "ENTER..........................menu", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.....................main menu").X / 2), 700), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 750), Color.White);
        }

        // UTILITY METHODS

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
            score = 0;
            swapRow = true;
        }
    }
}
