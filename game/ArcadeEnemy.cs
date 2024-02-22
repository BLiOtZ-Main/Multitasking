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
        /// Update placeholder
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
