using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multitasking
{
    internal class ArcadePlayer : ArcadeGameObject
    {
        // Constant for speed of player
        private const int Speed = 8;

        // Constant for how often a projectile is shot
        private const double TimePerShot = 0.5;

        // Field Declaration
        private int windowWidth;
        private double timer;
        private List<ArcadeProjectile> projectiles;

        /// <summary>
        /// public parameterized constructor that creates an Arcade Player object
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        public ArcadePlayer(Texture2D texture, Rectangle position, int windowWidth)
            : base(texture, position)
        {
            this.windowWidth = windowWidth;
            timer = TimePerShot;
            projectiles = new List<ArcadeProjectile>();
        }

        public List<ArcadeProjectile> Projectiles
        {
            get { return projectiles; }
        }

        public override void Update(GameTime gameTime)
        {
            // Allows player to move left and right using
            // left and right arrow keys

            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Left))
            {
                position.X -= Speed;
                if (position.X < windowWidth / 2)
                {
                    position.X = windowWidth / 2;
                }
            }

            if (kbState.IsKeyDown(Keys.Right))
            {
                position.X += Speed;
                if (position.X > windowWidth - 300)
                {
                    position.X = windowWidth - 300;
                }
            }
            
            // Tracks time elapsed to know when to shoot another projectile
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if(timer < 0)
            {
                projectiles.Add(new ArcadeProjectile(texture, position)); // Need to add actual texture later, just using placeholder to compile
            }
            
        }


    }
}
