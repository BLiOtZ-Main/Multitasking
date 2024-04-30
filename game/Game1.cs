using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

// notes
// - make the padding around highlighted words scale properly


namespace Multitasking
{
    public enum GameState { MainMenu, Settings, DayStart, DayLetter, Day, GameOver, LeaderBoard, Endless,  Pause }

    public enum GameDay { Day1, Day2, Day3, Day4, Day5, Day6, Day7, Day8, Day9, Day10, Day11, }
    
    
    public class Game1 : Game
    {
        // monogame
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // important
        public GameState currentState;
        public GameDay currentDay;
        private TypingGame typingGame;
        private ShooterGame shooterGame;
        private ArcadePlayer player;
        private List<ArcadeEnemy> enemyList;
        public Rectangle typingWindow;
        public Rectangle shooterWindow;
        public Rectangle boredomMeterBorder;
        public Rectangle boredomMeter;
        public string loseMessage;
        bool swapRow;
        Song music;
        string musicFile = "AdhesiveWombat - Night Shade  NO COPYRIGHT 8-bit Music";
        List<SoundEffect> soundEffects;
        string enemyShot = "fire(enemy)";
        string die = "crash";

        // assets
        public SpriteFont typingFont;
        public SpriteFont typingFontBold;
        public SpriteFont menuFont;
        public Texture2D squareImg;
        public Texture2D playerImg;
        public Texture2D enemyImg;
        public Texture2D playerBulletImg;
        public Texture2D whitePixel;
        public Texture2D spaceBackground;
        public Color backgroundColor = new Color(31, 0, 171);

        // numbers
        public int screenWidth;
        public int screenHeight;
        public const int EnemyDist = 70;
        public double enemyTimer;
        public const double EnemySpawnTime = 4;
        public double time;
        public int timeFinal;
        public int score1 = 0;
        public int score2 = 0;
        public int score3 = 0;
        public const int maxBoredom = 500;
        public int boredomMeterWidth;
        public int score;

        // input
        public KeyboardState previousKeyboardState;
        public MouseState previousMouseState;

        /// <summary>
        /// Game1 constructor
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            soundEffects = new List<SoundEffect>();
        }

        /// <summary>
        /// Initializes the height and width of the screen for gameplay. Also includes multiple
        /// initializations for gameplay windows, game states, players/enemies, enemy movement logic,
        /// and the actual gameplay loops for the shooter and typing games.
        /// </summary>
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
            enemyTimer = EnemySpawnTime;
            time = 1;

            // important
            currentState = GameState.MainMenu;
            currentDay = GameDay.Day1;
            player = new ArcadePlayer(playerImg, new Rectangle(3 * (screenWidth / 4), screenHeight - 200, 50, 50), screenWidth, playerBulletImg);
            enemyList = new List<ArcadeEnemy>();
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            typingGame = new TypingGame(typingWindow, 1);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);
            swapRow = true;
            shooterGame = new ShooterGame(_graphics, playerImg, enemyImg, playerBulletImg, this);
        }
        /// <summary>
        /// Loads the assets to be used in the game, sprites and fonts are found here
        /// </summary>
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
            whitePixel = Content.Load<Texture2D>("WhitePixel");
            spaceBackground = Content.Load<Texture2D>("NewSpaceBackground");
            music = Content.Load<Song>(musicFile);
            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            soundEffects.Add(Content.Load<SoundEffect>(die)); //0
            soundEffects.Add(Content.Load<SoundEffect>(enemyShot)); //1
        }
        /// <summary>
        /// Contains a switch for the basic gameplay loop / finite state machine. Has multiple
        /// sub-methods for updating certain parts of the game, such as menus and gameplay loops
        /// </summary>
        /// <param name="gameTime">required GameTime object for update methods</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
            // write code below
            if (MediaPlayer.State == MediaState.Stopped) MediaPlayer.Play(music);
            else if (MediaPlayer.State == MediaState.Paused) MediaPlayer.Resume();

            // update input states
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            // do something based on the current state
            switch (currentState)
            {
                // main menu
                case GameState.MainMenu:
                    UpdateMainMenu(currentKeyboardState);
                    break;

                // settings screen
                case GameState.Settings:
                    UpdateSettings(currentKeyboardState);
                    break;

                // start of day screen
                case GameState.DayStart:
                    UpdateDayStart(currentKeyboardState);
                    break;

                // pre day message
                case GameState.DayLetter:
                    UpdateDayLetter(currentKeyboardState);
                    break;

                //day gameplay
                case GameState.Day:
                    UpdateDay(gameTime, currentKeyboardState);
                    break;

                //endless gameplay
                
                case GameState.Endless:
                    UpdateEndless(currentKeyboardState, gameTime);
                    break;
                
                // game over screen
                case GameState.GameOver:
                    UpdateGameOver(currentKeyboardState);
                    break;

                //leaderboard screen
                case GameState.LeaderBoard:
                    UpdateLeaderBoard(currentKeyboardState);
                    break;
                case GameState.Pause:
                    UpdatePause(currentKeyboardState);
                    break;
            }

            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // write code above
            base.Update(gameTime);
        }
        /// <summary>
        /// Draw method also contains a switch statement for the fsm, only drawing what's in that certain state
        /// also contains a toggle for player godmode, which draws when it's active
        /// </summary>
        /// <param name="gameTime">required GameTime object for draw methods</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);
            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);
            // write code below

            // message for when god mode is enabled
            if (player.GodMode)
            {
                _spriteBatch.DrawString(typingFont, "God Mode Enabled (Restart Game to Disable)", new Vector2(20, 20), Color.White);
            }
            //FSM managing GameStates
            switch (currentState)
            {
                // main menu
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;

                // settings screen
                case GameState.Settings:
                    DrawSettings();
                    break;
                
                // start of day screen
                case GameState.DayStart:
                    DrawDayStart(_spriteBatch);
                    break;
                
                // pre day message
                case GameState.DayLetter:
                    DrawDayLetter();
                    break;

                // gameplay
                case GameState.Day:
                    DrawDay();
                    break;

                // endless
                
                case GameState.Endless:
                    DrawEndless();
                    break;
                

                // game over screen
                case GameState.GameOver:
                    DrawGameOver();
                    break;

                // leaderboard screen
                case GameState.LeaderBoard:
                    DrawLeaderBoard();
                    break;

                case GameState.Pause:
                    DrawPause();
                    break;
            }

            // write code above
            ShapeBatch.End();
            _spriteBatch.End();
            base.Draw(gameTime);
        }


        // UPDATE METHODS

        /// <summary>
        /// Updates the main menu based on a key press to take you to other states
        /// </summary>
        /// <param name="currentKeyboardState">Used for user input (menu switching)</param>
        public void UpdateMainMenu(KeyboardState currentKeyboardState)
        {
            if(SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.DayStart;
                ResetGame();
            }
            if (SingleKeyPress(currentKeyboardState, Keys.LeftShift))
            {
                time = 0;
                currentState = GameState.Endless;
                ResetGame();
            }
            if (SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.Settings;
            }

        }
        /// <summary>
        /// Updates the settings menu of the game
        /// </summary>
        /// <param name="currentKeyboardState">Used for user input (menu switching) or godmode toggle</param>
        public void UpdateSettings(KeyboardState currentKeyboardState)
        {
            if (SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.MainMenu;
            }
            if (SingleKeyPress(currentKeyboardState, Keys.Up))
            {
                MediaPlayer.Volume += 0.1f;
            }
            if (SingleKeyPress(currentKeyboardState, Keys.Down))
            {
                MediaPlayer.Volume -= 0.1f;
            }
        }

        /// <summary>
        /// Updates the pause menu of the game
        /// </summary>
        /// <param name="currentKeyboardState"></param>
        public void UpdatePause(KeyboardState currentKeyboardState)
        {
            if (SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.Day;
            }

            if (SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.MainMenu;
            }
        }

        public void UpdateDayStart(KeyboardState currentKeyboardState)
        {
            if(SingleKeyPress(currentKeyboardState, Keys.Space))
            {
                currentState = GameState.DayLetter;
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
        /// <summary>
        /// Updates the current letter of the typing game
        /// </summary>
        /// <param name="currentKeyboardState">Used for user input (menu switching)</param>
        public void UpdateDayLetter(KeyboardState currentKeyboardState)
        {
            currentState = typingGame.UpdateDayLetter(currentState);

            // debug
            if(SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                currentState = GameState.Day;
                typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            }
        }
        /// <summary>
        /// Updates the gameplay loop for a day in the game
        /// </summary>
        /// <param name="gameTime">GameTime object used for updating the clock/shooter</param>
        public void UpdateDay(GameTime gameTime, KeyboardState currentKeyboardState)
        {
            shooterGame.UpdateShooter(gameTime, currentDay, soundEffects);
            shooterGame.UpdateClock(gameTime);
            typingGame.UpdateDay();

            if (SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.Pause;
            }
        }

        /// <summary>
        /// Updates the entire gameplay loop for endless 
        /// </summary>
        /// <param name="currentKeyboardState">Checks for user input (debug purposes)</param>
        /// <param name="gameTime">GameTime object used for timer logic</param>

        public void UpdateEndless(KeyboardState currentKeyboardState, GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime.TotalSeconds;
            shooterGame.UpdateShooter(gameTime, currentDay, soundEffects);
            typingGame.UpdateDay();

            if (SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.Pause;
            }

        }
        
        /// <summary>
        /// Update for the game over screen
        /// </summary>
        /// <param name="currentKeyboardState">Used for user input (menu switching)</param>
        public void UpdateGameOver(KeyboardState currentKeyboardState)
        {
            if(SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.MainMenu;
            }
            if (SingleKeyPress(currentKeyboardState, Keys.Tab))
            {
                int currentScore = shooterGame.score;
                currentScore = currentScore * (int)time;
                if (currentScore > score1)
                {
                    score3 = score2;
                    score2 = score1;
                    score1 = currentScore;
                }
                else if (currentScore > score2 && currentScore <= score1)
                {
                    score3 = score2;
                    score2 = currentScore;
                }
                else if (currentScore > score3 && currentScore < score1 && currentScore <= score2)
                {
                    score3 = currentScore;
                }
                currentState = GameState.LeaderBoard;
            }
        }
        /// <summary>
        /// Updates the leaderboard menu
        /// </summary>
        /// <param name="currentKeyboardState">Used for user input (menu switching)</param>
        public void UpdateLeaderBoard(KeyboardState currentKeyboardState)
        {
            if (SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                currentState = GameState.MainMenu;
            }
        }


        // DRAW METHODS
        /// <summary>
        /// Draws out the various icons / instructions for the main menu
        /// </summary>
        public void DrawMainMenu()
        { 
            _spriteBatch.DrawString(menuFont, "multitasking", new Vector2((screenWidth / 2) - (menuFont.MeasureString("multitasking").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, "by.....................omni_absence", new Vector2((screenWidth / 2) - (typingFont.MeasureString("by.....................omni_absence").X / 2), 400), backgroundColor);
            _spriteBatch.DrawString(typingFont, "ENTER.........................start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.........................start").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "LEFT SHIFT.............endless mode", new Vector2((screenWidth / 2) - (typingFont.MeasureString("LEFT SHIFT.............endless mode").X / 2), 550), Color.White);
            _spriteBatch.DrawString(typingFont, "TAB........................settings", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB........................settings").X / 2), 600), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 650), Color.White);
            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString("by.....................omni_absence").X / 2 - 8, 400, (int)typingFont.MeasureString("by.....................omni_absence").X + 16, 35), new Color(255, 255, 255));

        }
        /// <summary>
        /// Draws out the various icons/instructions for the settings
        /// </summary>
        public void DrawSettings()
        {
            _spriteBatch.DrawString(menuFont, "settings", new Vector2((screenWidth / 2) - (menuFont.MeasureString("settings").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, "TILDE(`)....................GodMode", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TILDE(`)....................GodMode").X / 2), 450), Color.White);
            _spriteBatch.DrawString(typingFont, "UP ARROW..................VOLUME UP", new Vector2((screenWidth / 2) - (typingFont.MeasureString("UP ARROW..................VOLUME UP").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "DOWN ARROW..............VOLUME DOWN", new Vector2((screenWidth / 2) - (typingFont.MeasureString("DOWN ARROW..............VOLUME DOWN").X / 2), 550), Color.White);
            _spriteBatch.DrawString(typingFont, "TAB............................menu", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB............................menu").X / 2), 600), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 650), Color.White);
        }

        public void DrawPause()
        {
            _spriteBatch.DrawString(menuFont, "paused", new Vector2((screenWidth / 2) - (menuFont.MeasureString("paused").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, "Enter........................resume", new Vector2((screenWidth / 2) - (typingFont.MeasureString("Enter........................resume").X / 2), 450), Color.White);
            _spriteBatch.DrawString(typingFont, "TAB............................menu", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB............................menu").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 550), Color.White);
        }

        public void DrawDayStart(SpriteBatch spriteBatch)
        {
            switch(currentDay)
            {
                case GameDay.Day1:
                    DrawDayStartManager(1, "12.20.1999", "post payday blues");
                    break;
                case GameDay.Day2:
                    DrawDayStartManager(2, "12.21.1999", "???");
                    break;
                case GameDay.Day3:
                    DrawDayStartManager(3, "12.22.1999", "update day");
                    break;
                case GameDay.Day4:
                    DrawDayStartManager(4, "12.23.1999", "millenium bug");
                    break;
                case GameDay.Day5:
                    DrawDayStartManager(5, "12.24.1999", "holiday party");
                    break;
                case GameDay.Day6:
                    DrawDayStartManager(6, "12.25.1999", "a blue screen of death christmas");
                    break;
                case GameDay.Day7:
                    DrawDayStartManager(7, "12.27.1999", "prod notes");
                    break;
                case GameDay.Day8:
                    DrawDayStartManager(8, "12.28.1999", "CAPS LOCK");
                    break;
                case GameDay.Day9:
                    DrawDayStartManager(9, "12.29.1999", "???");
                    break;
                case GameDay.Day10:
                    DrawDayStartManager(10, "12.30.1999", "???");
                    break;
                case GameDay.Day11:
                    DrawDayStartManager(11, "12.31.1999", "payday");
                    break;
            }
        }
        /// <summary>
        /// Draws the menu for selecting a day (level select)
        /// </summary>
        /// <param name="dayNumber">Number used in the selected day</param>
        /// <param name="date">Flavor text for use in the selected day</param>
        /// <param name="dayName">Name of the day that is selected</param>
        public void DrawDayStartManager(int dayNumber, String date, String dayName)
        {
            _spriteBatch.DrawString(menuFont, "day " + dayNumber, new Vector2((screenWidth / 2) - (menuFont.MeasureString("day " + dayNumber).X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, dayName, new Vector2((screenWidth / 2) - (typingFont.MeasureString(dayName).X / 2), 400), backgroundColor);
            _spriteBatch.DrawString(typingFont, date, new Vector2((screenWidth / 2) - (typingFont.MeasureString(date).X / 2), 450), Color.White);
            _spriteBatch.DrawString(typingFont, "SPACE.........................start", new Vector2((screenWidth / 2) - (typingFont.MeasureString("SPACE.........................start").X / 2), 550), Color.White);
            _spriteBatch.DrawString(typingFont, "RIGHT_ALT............debug_skip_day", new Vector2((screenWidth / 2) - (typingFont.MeasureString("RIGHT_ALT............debug_skip_day").X / 2), 600), Color.White);
            _spriteBatch.DrawString(typingFont, "LEFT_ALT.........debug_previous_day", new Vector2((screenWidth / 2) - (typingFont.MeasureString("LEFT_ALT.........debug_previous_day").X / 2), 650), Color.White);
            ShapeBatch.Box(new Rectangle((screenWidth / 2) - (int)typingFont.MeasureString(dayName).X / 2 - 8, 400, (int)typingFont.MeasureString(dayName).X + 16, 35), new Color(255, 255, 255));
        }

        /// <summary>
        /// Draws the letter for typing in the day
        /// </summary>
        public void DrawDayLetter()
        {
            typingGame.DrawDayLetter(_spriteBatch, typingFont, typingFontBold, whitePixel, screenWidth, screenHeight);
        }

        /// <summary>
        /// Draws the different windows and indicators for a day, as well as the shooter game
        /// </summary>
        public void DrawDay()
        {
            //Reset the windows to the right sizes because the demo messes them up otherwise
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);


            //Temp code to visualize the two game window
            ShapeBatch.Box(shooterWindow, Color.White);

            _spriteBatch.DrawString(typingFont, shooterGame.PrintClock(), new Vector2(75, 100), Color.Yellow);

            shooterGame.DrawShooterGame(_spriteBatch, typingFont);
            typingGame.DrawDay(_spriteBatch, typingFont, typingFontBold, whitePixel);
        }
        /// <summary>
        /// Draws the endless gamemode using the different windows and indicators for a day
        /// Also draws out many ui features for the game
        /// </summary>

        public void DrawEndless()
        {
            //Reset the windows to the right sizes because the demo messes them up otherwise
            typingWindow = new Rectangle(200, 100, screenWidth / 2, screenHeight - 200);
            shooterWindow = new Rectangle(screenWidth / 2, 100, screenWidth / 2 - 200, screenHeight - 200);

            //time
            timeFinal = (int) time;
            _spriteBatch.DrawString(typingFont, $"Time: {timeFinal}", new Vector2(75, 100), Color.Yellow);

            //Temp code to visualize the two game window
            ShapeBatch.Box(shooterWindow, Color.White);

            //_spriteBatch.DrawString(typingFont, shooterGame.PrintClock(), new Vector2(75, 100), Color.Yellow);

            shooterGame.DrawShooterGame(_spriteBatch, typingFont);
            typingGame.DrawDay(_spriteBatch, typingFont, typingFontBold, whitePixel);
        }
        
        /// <summary>
        /// Draws the game over screen + instructions for where the player can go
        /// </summary>
        public void DrawGameOver()
        {
            _spriteBatch.DrawString(menuFont, "game over", new Vector2((screenWidth / 2) - (menuFont.MeasureString("game over").X / 2), 300), Color.White);
            _spriteBatch.DrawString(typingFont, shooterGame.loseMessage, new Vector2((screenWidth / 2) - (typingFont.MeasureString(shooterGame.loseMessage).X / 2), 400), Color.White);
            _spriteBatch.DrawString(typingFont, "ENTER..........................menu", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ENTER.....................main menu").X / 2), 500), Color.White);
            _spriteBatch.DrawString(typingFont, "TAB.....................leaderboard", new Vector2((screenWidth / 2) - (typingFont.MeasureString("TAB.....................leaderboard").X / 2), 550), Color.White);
            _spriteBatch.DrawString(typingFont, "ESC............................quit", new Vector2((screenWidth / 2) - (typingFont.MeasureString("ESC............................quit").X / 2), 600), Color.White);
        }

        
        public void DrawLeaderBoard()
        {
           
            
            
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
            shooterGame.Reset();
            typingGame = new TypingGame(typingWindow, 1);
        }
    }
}
