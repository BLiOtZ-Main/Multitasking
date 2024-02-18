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
    internal class TypingGame
    {
        private TypePrompt tutorialPrompt;
        private TypePrompt gamePrompt;

        private bool inTutorial;

        public TypingGame()
        {
            tutorialPrompt = new TypePrompt("tutorial.txt", true);
            gamePrompt = new TypePrompt("game.txt", false);

            inTutorial = true;
        }

        public void Update()
        {
            if(inTutorial)
            {
                tutorialPrompt.Update();
            }
            else
            {
                gamePrompt.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
        {
            if(inTutorial)
            {
                tutorialPrompt.Draw(spriteBatch, font, screenWidth, screenHeight);
            }
            else
            {
                gamePrompt.Draw(spriteBatch, font, screenWidth, screenHeight);
            }
        }
    }
}
