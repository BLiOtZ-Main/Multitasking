﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Multitasking
{
    internal class ArcadeProjectile : ArcadeGameObject
    {
        

        // field declaration
        private bool active;
        private Rectangle startingPosition;
        private bool playerProjectile = false;
        private Random rng = new Random();
 

        /// <summary>
        /// public parameterized constructor for projectiles
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public ArcadeProjectile (Texture2D texture, Rectangle position)
            : base(texture, position)
        {
            active = true;
            startingPosition = position;
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public bool PlayerProjectile
        {
            get { return playerProjectile; }
            set { playerProjectile = value; }
        }

        /// <summary>
        /// Checks for collision between projectile and a GameObject (i.e enemies)
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool CheckCollision (ArcadeGameObject check)
        {
            if (active)
            {
                //GodMode Logic
                if(check is ArcadePlayer)
                {
                    ArcadePlayer player = check as ArcadePlayer;

                    if (player.GodMode)
                    {
                        return false;
                    }
                }

                    return base.position.Intersects(check.position);

            }
            else
            {
                return false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Determines if the player or enemy is shooting based on starting position
            //If the player is shooting
            
            if (playerProjectile)
            {
                if (position.Y > 100)
                {
                    //The projectile goes up
                    position.Y -= rng.Next(4,10);
                }
                else
                {
                    active = false;
                }
            }
            //if the enemy is shooting
            else
            {
                if (position.Y < 950)
                {
                    //The projectile goes down
                    position.Y += rng.Next(4,10);
                }
                else
                {
                    active = false;
                }
            }
            
        }

        public override void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(texture, position, color);
        }
    }
}
