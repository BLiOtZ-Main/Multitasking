using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multitasking
{
    internal class ArcadeProjectile : ArcadeGameObject
    {
        // field declaration
        private bool active;

        /// <summary>
        /// public parameterized constructor for projectiles
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public ArcadeProjectile (Texture2D texture, Rectangle position)
            : base(texture, position)
        {
            active = true;
        }

        /// <summary>
        /// Checks for collision between projectile and a GameObject (i.e enemies)
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool CheckCollision (ArcadeGameObject check)
        {
            return base.position.Intersects(check.position);
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
