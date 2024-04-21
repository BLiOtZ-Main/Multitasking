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
        public const double EnemySpawnTime = 4;
        public const double BoredomTime = 0.5;
        public const int maxBoredom = 500;
        public int boredomMeterWidth;
        public int score;

        //Gameplay
        private ArcadePlayer player;
        private List<ArcadeEnemy> enemyList;
        bool swapRow;

        //Visuals
        public Rectangle boredomMeterBorder;
        public Rectangle boredomMeter;
        public Texture2D playerBulletImg;
        public Texture2D enemyImg;
        public Texture2D playerImg;

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

                    if(boredomTimer <= 0)
                    {
                        boredomMeterWidth += 3;
                        boredomTimer = BoredomTime;
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

                    if (boredomTimer <= 0)
                    {
                        boredomMeterWidth += 3;
                        boredomTimer = BoredomTime;
                    }
                    break;

            }

            //GameObject Updates
            
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

            //Temp code to swap to GameOver
            if (SingleKeyPress(currentKeyboardState, Keys.Enter))
            {
                game1.currentState = GameState.GameOver;
                loseMessage = "(You're debugging right now aren't you?)";
            }

            //If the player dies game over
            if (!player.IsAlive)
            {
                boredomMeterWidth += 100;
                DeathReset();
            }

            // If boredom meter fills game over
            if (boredomMeterWidth >= maxBoredom)
            {
                game1.currentState = GameState.GameOver;
                loseMessage = "(You resigned from your job)";
            }

        }

        public void DrawShooterGame(SpriteBatch _spriteBatch, SpriteFont typingFont)
        {
            //Ui Drawing

            //Creates/Resets the Boredom meter
            boredomMeterBorder = new Rectangle((screenWidth / 2) + 125, 60, maxBoredom, 30);
            boredomMeter = new Rectangle(boredomMeterBorder.X, boredomMeterBorder.Y, boredomMeterWidth, boredomMeterBorder.Height);

            //Draws The Boredom meter
            ShapeBatch.BoxOutline(boredomMeterBorder, Color.White);
            ShapeBatch.Box(boredomMeter, Color.White);

            //Draws Ui Labels
            _spriteBatch.DrawString(typingFont, "Space Game Again TM", new Vector2(1200, 100), Color.Black);
            _spriteBatch.DrawString(typingFont, $"Score: {score}", new Vector2(1000, 100), Color.Blue);
            _spriteBatch.DrawString(typingFont, "boredom", new Vector2(boredomMeterBorder.X + (float)(0.5 * boredomMeterBorder.Width) - (typingFont.MeasureString("boredom").X / 2), 30), Color.White);
            
            
            //Gameplay Drawing
            
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

        public void Reset()
        {
            player.IsAlive = true;
            enemyList.Clear();
            player.Projectiles.Clear();
            enemyTimer = EnemySpawnTime;
            score = 0;
            swapRow = true;
            boredomMeterWidth = 250;
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
    }
}
