using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace Multitasking
{

     //   Additional Improvements
     // - Translucent bar highlighting your current line
     // - Animations for correctly/incorrectly typing a key
     // - Character spacing and line spacing needs to be improved
     //

    internal class TypePrompt
    {
        // holds last frame's KeyboardState
        private KeyboardState previousKeyBoardState;

        // a list of the current prompts taken from a .txt file
        private List<String> prompts;

        private int currentLine;
        private int currentChar;
        private bool staticPrompt;
        private const int lineSpacing = 60;
        private const int verticalOffset = 150;
        
        
        public TypePrompt(String filename, bool staticPrompt)
        {
            String filepath = "../../../../resources/prompts/" + filename;


            prompts = new List<String>();

            currentLine = 0;
            currentChar = 0;
            this.staticPrompt = staticPrompt;

            ScrapeFile(filepath);
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if(currentLine != -1)
            {
                if(GetPressedLetterCount(keyboardState) <= 2 && 
                   keyboardState.IsKeyDown(GetKeyFromChar(prompts[currentLine].Substring(currentChar, 1))) && 
                   previousKeyBoardState != keyboardState)
                {
                    Debug.WriteLine("You pressed the right key! (key: " + prompts[currentLine].Substring(currentChar, 1) + ")");

                    if(currentChar == prompts[currentLine].Length - 1)
                    {
                        if(currentLine == prompts.Count - 1)
                        {
                            currentLine = -1;
                        }
                        else
                        {
                            currentLine += 1;
                            currentChar = 0;
                        }
                    }
                    else
                    {
                        currentChar += 1;

                        while(prompts[currentLine].Substring(currentChar, 1) == " ")
                        {
                            currentChar += 1;
                        }
                    }
                }
            }

            previousKeyBoardState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
        {
            if(staticPrompt)
            {
                for(int i = 0; i < prompts.Count; i++)
                {
                    if(i < currentLine)
                    {
                        spriteBatch.DrawString(font, prompts[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
                    }
                    else if(i > currentLine)
                    {
                        spriteBatch.DrawString(font, prompts[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, prompts[i].Substring(0, prompts[i].Length), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                        spriteBatch.DrawString(font, prompts[i].Substring(0, currentChar), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                        ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, screenWidth, 35), new Color(255, 255, 255));
                    }
                    
                }
            }
            else
            {

            }
        }

        public void ScrapeFile(String filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            String currentLine = reader.ReadLine();

            while (currentLine != null)
            {
                String lineToAdd = "";

                for (int i = 0; i < currentLine.Length; i++)
                {
                    lineToAdd += currentLine[i];
                }

                prompts.Add(lineToAdd);
                currentLine = reader.ReadLine();
            }

            reader.Close();
        }

        public Keys GetKeyFromChar(String character)
        {
            character = character.ToLower();

            switch(character)
            {
                case "a":
                    return Keys.A;
                case "b":
                    return Keys.B;
                case "c":
                    return Keys.C;
                case "d":
                    return Keys.D;
                case "e":
                    return Keys.E;
                case "f":
                    return Keys.F;
                case "g":
                    return Keys.G;
                case "h":
                    return Keys.H;
                case "i":
                    return Keys.I;
                case "j":
                    return Keys.J;
                case "k":
                    return Keys.K;
                case "l":
                    return Keys.L;
                case "m":
                    return Keys.M;
                case "n":
                    return Keys.N;
                case "o":
                    return Keys.O;
                case "p":
                    return Keys.P;
                case "q":
                    return Keys.Q;
                case "r":
                    return Keys.R;
                case "s":
                    return Keys.S;
                case "t":
                    return Keys.T;
                case "u":
                    return Keys.U;
                case "v":
                    return Keys.V;
                case "w":
                    return Keys.W;
                case "x":
                    return Keys.X;
                case "y":
                    return Keys.Y;
                case "z":
                    return Keys.Z;
            }

            return Keys.None;
        }

        public int GetPressedLetterCount(KeyboardState keyboardState)
        {
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            int numOfNonLetterKeysPressed = 0;
            
            for(int i = 0; i < pressedKeys.Length; i++)
            {
                if(pressedKeys[i] == Keys.A || pressedKeys[i] == Keys.B || pressedKeys[i] == Keys.C || pressedKeys[i] == Keys.D ||
                   pressedKeys[i] == Keys.E || pressedKeys[i] == Keys.F || pressedKeys[i] == Keys.G || pressedKeys[i] == Keys.H ||
                   pressedKeys[i] == Keys.I || pressedKeys[i] == Keys.J || pressedKeys[i] == Keys.K || pressedKeys[i] == Keys.L ||
                   pressedKeys[i] == Keys.M || pressedKeys[i] == Keys.N || pressedKeys[i] == Keys.O || pressedKeys[i] == Keys.P ||
                   pressedKeys[i] == Keys.Q || pressedKeys[i] == Keys.R || pressedKeys[i] == Keys.S || pressedKeys[i] == Keys.T ||
                   pressedKeys[i] == Keys.U || pressedKeys[i] == Keys.V || pressedKeys[i] == Keys.W || pressedKeys[i] == Keys.X ||
                   pressedKeys[i] == Keys.Y || pressedKeys[i] == Keys.Z)
                {
                    numOfNonLetterKeysPressed++;
                }
            }

            return numOfNonLetterKeysPressed;
        }
    }
}
