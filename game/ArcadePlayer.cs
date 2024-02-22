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
        private const int Speed = 4;

        // Field Declaration
        private int windowHeight;
        private int windowWidth;

        /// <summary>
        /// public parameterized constructor that creates an Arcade Player object
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        public ArcadePlayer(Texture2D texture, Rectangle position, int windowHeight, int windowWidth)
            : base(texture, position)
        {
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
        }

        public override void Update(GameTime gameTime)
        {
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
                if (position.X > windowWidth - texture.Width)
                {
                    position.X = windowWidth - texture.Width;
                }
            }
        }


    }
}
