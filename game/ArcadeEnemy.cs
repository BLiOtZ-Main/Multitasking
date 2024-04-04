﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multitasking
{
    internal class ArcadeEnemy : ArcadeGameObject
    {
        private int windowHeight;
        private int windowWidth;

        private ArcadePlayer player;

        //Constants for enemy shift and shoot delay
        private const double ShiftDelay = Math.PI;
        private const double ShootDelay = 3;
        private const double MoveDelay = 1;

        //Constant for enemy shift distance
        private const int ShiftDistance = 3;

        //Needed booleans
        private bool isAlive;
        private bool shiftedRight;
        
        //Timer related variables
        private double shootTimer;
        private double shiftTimer;
        private double moveTimer;

        //Texture for Projectiles
        private Texture2D projectileTexture;

        //Rng
        private Random rng = new Random();

        //List of all enemies
       

        public List<ArcadeProjectile> projectiles;

        /// <summary>
        /// public parameterized constructor for arcade enemies
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        public ArcadeEnemy (Texture2D texture, Rectangle position, int windowHeight, int windowWidth, ArcadePlayer player, Texture2D projectileTexture)
            : base(texture, position)
        {
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            
            shiftedRight = false;
            shiftTimer = ShiftDelay;
            shootTimer = ShootDelay;
            moveTimer = MoveDelay;
            isAlive = true;

            //Adds the new enemy to the enemy list

            projectiles = new List<ArcadeProjectile>();
            
            //Stores a reference to the player
            this.player = player;

            //Stores projectile texture
            this.projectileTexture = projectileTexture;
            
        }

        /// <summary>
        /// Property to tell if an enemy is alive or dead
        /// </summary>
        public bool IsAlive { get { return isAlive; } set {  isAlive = value;  } }

        /// <summary>
        /// Property to get the list of enemies
        /// </summary>



        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //Checks if a projectile collides
            foreach (ArcadeProjectile p in projectiles)
            {
                if (p.CheckCollision(player))
                {
                    player.IsAlive = false;
                }

            }

            //Tracks time to know when to shift
            shiftTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (shiftTimer < 0)
            {
                //If the enemy shifted right last shift
                if(shiftedRight)
                {
                    //Shift Left
                    position.X -= 20;
                    shiftedRight = false;
                    shiftTimer = ShiftDelay;
                }
                //Otherwise
                else
                {
                    //Shift right
                    position.X += 20;
                    shiftedRight = true;
                    shiftTimer = ShiftDelay;
                }
            }

            //The enemy constantly moves downward
            moveTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if(moveTimer < 0)
            {
                position.Y += 15;
                moveTimer = MoveDelay;
            }

            
            //Tracks time to know when to shoot
            shootTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (shootTimer < 0)
            {   
                //randomizes the shooting a bit
                if (rng.Next(100) > 98)
                {
                    projectiles.Add(new ArcadeProjectile(projectileTexture, new Rectangle(position.X + texture.Width/2, position.Y + texture.Height, 17, 33)));
                    shootTimer = ShootDelay;
                }
                
            }
        }
    }
}
