using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        /// <param name="texture">Texture used for the projectile</param> 
        /// <param name="position">Position to be used in later methods (draw/update)</param> 
        public ArcadeProjectile (Texture2D texture, Rectangle position)
            : base(texture, position)
        {
            active = true;
            startingPosition = position;
        }

        /// <summary>
        /// get/set for whether a projectile is active
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// get/set for a check on whether a projectile
        /// is a player projectile or not
        /// </summary>
        public bool PlayerProjectile
        {
            get { return playerProjectile; }
            set { playerProjectile = value; }
        }

        /// <summary>
        /// Checks for collision between projectile and a GameObject (i.e enemies)
        /// Also contains logic for checking God Mode
        /// </summary>
        /// <param name="check">Can be a player/enemy, used in intersects method</param> 
        /// <returns>Returns whether a projectile has hit an enemy/player</returns> 
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
        /// <summary>
        /// Update method for updating a projectile. Checks for what is shooting a projectile (player/enemy)
        /// and makes it go down/up based on position
        /// </summary>
        /// <param name="gameTime">Required GameTime object for update methods</param> 
        public override void Update(GameTime gameTime)
        {
            //Determines if the player or enemy is shooting based on starting position
            //If the player is shooting
            
            if (playerProjectile)
            {
                if (position.Y > 105)
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
                if (position.Y < 910)
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
        /// <summary>
        /// Overridden Draw method, draws the projectile that has been fired
        /// </summary>
        /// <param name="sb">Required SpriteBatch object for draw methods</param> 
        /// <param name="color">Color of the projectile (could change b/c of who shot it)</param> 
        public override void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(texture, position, color);
        }
    }
}
