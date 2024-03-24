using Microsoft.Xna.Framework;
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

        private bool isAlive;
        private bool shiftedRight;
        private int shiftDelay;
        private int shootDelay;
        private Random rng;

        /// <summary>
        /// public parameterized constructor for arcade enemies
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        public ArcadeEnemy (Texture2D texture, Rectangle position, int windowHeight, int windowWidth)
            : base(texture, position)
        {
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
        }

        /// <summary>
        /// Property to tell if an enemy is alive or dead
        /// </summary>
        public bool IsAlive { get { return isAlive; } }

        /// <summary>
        /// Update placeholder
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

            /*

            If Projectile collides with enemy
            {
                isAlive = false;
            }
            
            
            Enemies shift left and right every "shiftDelay" seconds
             
            If Bool "shiftedRight" -> shift left
            {
            position.X -= 5;
            }
            else
            {
            position.X -= 5;
            }
            
            //Enemies constantly move downward no matter what
            position.Y -= 1;
            
            //every 3 seconds % chance to shoot a projectile
            
            If "shootDelay" seconds passed
            {
               if rng.Next() > (whatever the percentage is decided to be
               {
                  create new projectile object
               }
            }
            
             */

        }



    }
}
