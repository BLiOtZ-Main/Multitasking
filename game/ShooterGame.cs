using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Multitasking
{
    internal class ShooterGame
    {
        //Very important
        Game1 game1;
        
        //Numbers
        public const int EnemyDist = 70;
        public double enemyTimer;
        public double boredomTimer;
        public double typingTimer;
        public const double EnemySpawnTime = 4;
        public const double BoredomTime = 0.5;
        public const double TypingTimerReset = 4;
        public int clockHour;
        public int clockMinute;
        public const int maxBoredom = 500;
        public int boredomMeterWidth;
        public int score;
        public int loadBarWidth;
        public const int loadBarMax = 500;

        //Gameplay
        private ArcadePlayer player;
        private List<ArcadeEnemy> enemyList;
        bool swapRow;

        //Visuals
        public Rectangle boredomMeterBorder;
        public Rectangle boredomMeter;
        public Rectangle loadMeterBorder;
        public Rectangle loadMeter;
        public Texture2D playerBulletImg;
        public Texture2D enemyImg;
        public Texture2D playerImg;
        public bool drawLoad = false;

        public string loseMessage;
        
        public int screenWidth;
        public int screenHeight;
        
        public KeyboardState previousKeyboardState;


        public ShooterGame(GraphicsDeviceManager _graphics, Texture2D playerImg, Texture2D enemyImg, Texture2D playerBulletImg, Game1 game1)
        {
            this.game1 = game1;
            
            //Initilizes the required graphics
            this.playerImg = playerImg;
            this.enemyImg = enemyImg;
            this.playerBulletImg = playerBulletImg;
            
            // numbers
            screenWidth = _graphics.GraphicsDevice.Viewport.Width;
            screenHeight = _graphics.GraphicsDevice.Viewport.Height;
            enemyTimer = EnemySpawnTime;
            boredomTimer = BoredomTime;
            typingTimer = TypingTimerReset;
            clockHour = 9;
            clockMinute = 0;

            player = new ArcadePlayer(playerImg, new Rectangle(3 * (screenWidth / 4), screenHeight - 200, 50, 50), screenWidth, playerBulletImg);
            enemyList = new List<ArcadeEnemy>();
        }

        public void UpdateShooter(GameTime gameTime, GameDay currentDay)
        {
            //Update input states
            KeyboardState currentKeyboardState = Keyboard.GetState();

            //Gameplay Logic

            //Timer to track when to spawn more enemies
            enemyTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            boredomTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (drawLoad)
            {
                loadBarWidth += 1;

                if (loadBarWidth == loadBarMax)
                {
                    loadBarWidth = 0;
                    drawLoad = false;
                }

                DeathReset();
            }
            else
            {
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
                        else if (enemyTimer <= 0)
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

                            enemyTimer = EnemySpawnTime;
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
                        else if (enemyTimer <= 0)
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

                            enemyTimer = EnemySpawnTime;
                        }

                        break;

                }
            }

            //GameObject Updates

            if (boredomTimer <= 0)
            {
                boredomMeterWidth += 3;
                boredomTimer = BoredomTime;
            }

            //Updates the Player
            player.Update(gameTime);

            //Updates every Enemy
            foreach (ArcadeEnemy e in enemyList)
            {
                if (e.IsAlive)
                {
                    e.Update(gameTime);
                }
            }

            //Updates the player's projectiles
            foreach (ArcadeProjectile projectile in player.Projectiles)
            {
                if (projectile.Active)
                {
                    projectile.Update(gameTime);
                }

            }

            //Collision Checks

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
                            boredomMeterWidth -= 10;
                        }
                    }
                }
            }

            //Player collision check
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

            //Lose Conditions

            //Is enemies reach the player game over
            foreach (ArcadeEnemy e in enemyList)
            {
                if (e.position.Y >= screenHeight - 200)
                {
                    boredomMeterWidth += 200;
                    DeathReset();
                    break;
                }
            }

            //If the player dies game over
            if (!player.IsAlive)
            {
                boredomMeterWidth += 30;
                drawLoad = true;
              
            }

            // If boredom meter fills game over
            if (boredomMeterWidth >= maxBoredom)
            {
                game1.currentState = GameState.GameOver;
                loseMessage = "(You resigned from your job)";
            }

        }

        public void DrawShooterGame(SpriteBatch _spriteBatch, SpriteFont typingFont, GameDay currentDay)
        {
            //Ui Drawing

            //Creates/Resets the Boredom meter
            boredomMeterBorder = new Rectangle((screenWidth / 2) + 125, 60, maxBoredom, 30);
            boredomMeter = new Rectangle(boredomMeterBorder.X, boredomMeterBorder.Y, boredomMeterWidth, boredomMeterBorder.Height);

            //Creates/Resets the load Meter
            loadMeterBorder = new Rectangle((screenWidth / 2) + 125, 450, loadBarMax, 30);
            loadMeter = new Rectangle(loadMeterBorder.X, loadMeterBorder.Y, loadBarWidth, loadMeterBorder.Height);

            //Draws The Boredom meter
            ShapeBatch.BoxOutline(boredomMeterBorder, Color.White);
            ShapeBatch.Box(boredomMeter, Color.White);


            //Draws Ui Labels
            _spriteBatch.DrawString(typingFont, "Space Game Again TM", new Vector2(1200, 100), Color.Black);
            _spriteBatch.DrawString(typingFont, $"Score: {score}", new Vector2(1000, 100), Color.Blue);
            _spriteBatch.DrawString(typingFont, "boredom", new Vector2(boredomMeterBorder.X + (float)(0.5 * boredomMeterBorder.Width) - (typingFont.MeasureString("boredom").X / 2), 30), Color.White);

            //Gameplay Drawing

            //Draws the loadscreen
            if (drawLoad)
            {
                _spriteBatch.DrawString(typingFont, "Loading", new Vector2(loadMeterBorder.X + (float)(0.5 * loadMeterBorder.Width) - (typingFont.MeasureString("Loading").X / 2), 400), Color.White);
                ShapeBatch.BoxOutline(loadMeterBorder, Color.White);
                ShapeBatch.Box(loadMeter, Color.White);
            }
            else
            {
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
   

            

        }

        public void Reset()
        {
            player.IsAlive = true;
            enemyList.Clear();
            player.Projectiles.Clear();
            enemyTimer = EnemySpawnTime;
            score = 0;
            swapRow = true;
            boredomMeterWidth = 250;
            clockHour = 9;
            clockMinute = 0;
            typingTimer = TypingTimerReset;
        }

        public void DeathReset()
        {
            player.IsAlive = true;
            enemyList.Clear();
            player.Projectiles.Clear();
            enemyTimer = EnemySpawnTime;
            score = 0;
            swapRow = true;
        }

        /// <summary>
        /// Updates the clock in the top left
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateClock(GameTime gameTime)
        {
            typingTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (typingTimer <= 0)
            {
                // Only updates clock if it's not 5:00 pm
                if (clockHour != 5)
                {
                    // Checks if time needs to change from 12 - 1
                    if (clockMinute == 5 && clockHour == 12)
                    {
                        clockHour = 1;
                        clockMinute = 0;
                    }
                    // Checks if the hour number needs to change
                    else if (clockMinute == 5)
                    {
                        clockHour++;
                        clockMinute = 0;
                    }
                    // Otherwise just increments minute
                    else
                    {
                        clockMinute++;
                    }

                }
                typingTimer = TypingTimerReset;
            }
        }

        public String PrintClock()
        {
            return String.Format("{0}:{1}0", clockHour, clockMinute);
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

        private void LoadScreen()
        {
  
            loadBarWidth += 1;

            if(loadBarWidth == loadBarMax)
            {
                loadBarWidth = 0;
                DeathReset();
                drawLoad = false;
            }
        }
    }
}
