using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Multitasking
{
    internal class TypingGame
    {
        // controls
        private KeyboardState previousKeyBoardState;

        // prompts
        private List<String> letterPrompt;
        private List<String> taskOnePrompt;
        private List<String> taskTwoPrompt;
        private List<String> taskThreePrompt;

        // gameplay
        private GameState currentState;
        private Dictionary<String, Keys> stringToKeys;
        private List<String> skippedCharacters;
        private List<Keys> letterKeys;
        private int currentLineIndex;
        private int currentCharIndex;
        private int currentPromptIndex;
        private int day;

        // aesthetics
        private Rectangle typingWindow;
        private const int lineSpacing = 60;
        private const int verticalOffset = 150;

        private List<String> CurrentPrompt
        {
            get
            {
                if(currentPromptIndex == -1)
                    return letterPrompt;
                else if(currentPromptIndex == 0)
                    return taskOnePrompt;
                else if(currentPromptIndex == 1)
                    return taskTwoPrompt;
                else
                    return taskThreePrompt;
            }
        }

        public TypingGame(Rectangle window, int day)
        {
            // initializing variables
            letterPrompt = new List<String>();
            taskOnePrompt = new List<String>();
            taskTwoPrompt = new List<String>();
            taskThreePrompt = new List<String>();
            currentState = new GameState();
            skippedCharacters = new List<String>() { " ", ".", "!", "?", "'", ",", "^" };
            letterKeys = new List<Keys>() { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                                            Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z};
            stringToKeys = new Dictionary<string, Keys>() { { "a", Keys.A }, { "b", Keys.B }, { "c", Keys.C }, { "d", Keys.D }, { "e", Keys.E }, { "f", Keys.F },
                                                            { "g", Keys.G }, { "h", Keys.H }, { "i", Keys.I }, { "j", Keys.J }, { "k", Keys.K }, { "l", Keys.L },
                                                            { "m", Keys.M }, { "n", Keys.N }, { "o", Keys.O }, { "p", Keys.P }, { "q", Keys.Q }, { "r", Keys.R },
                                                            { "s", Keys.S }, { "t", Keys.T }, { "u", Keys.U }, { "v", Keys.V }, { "w", Keys.W }, { "x", Keys.X },
                                                            { "y", Keys.Y }, { "z", Keys.Z }};
            typingWindow = new Rectangle(window.X, window.Y, window.Width, window.Height);
            currentPromptIndex = -1;
            currentLineIndex = 0;
            currentCharIndex = 0;
            this.day = day;

            // scrape files for prompts
            ScrapeFile("../../../../resources/prompts/day" + day + ".txt", day);
            MoveToNextAvailableChar();
        }


        // CORE GAME METHODS

        public GameState UpdateDayLetter(GameState currentState)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            this.currentState = currentState;
            bool endOfPrompt;
            bool exceededKeysPressed;
            bool pressingCurrentKey;
            bool changedKeyboardState;

            if(currentLineIndex == -1)
            {
                currentPromptIndex = 0;
                currentLineIndex = 0;
                currentCharIndex = 0;
                previousKeyBoardState = keyboardState;
                return GameState.Day;
            }
            else
            {
                endOfPrompt = currentLineIndex != -1;
                exceededKeysPressed = GetPressedLetterCount(keyboardState) <= 2;
                pressingCurrentKey = keyboardState.IsKeyDown(GetKeyFromChar(letterPrompt[currentLineIndex].Substring(currentCharIndex, 1)));
                changedKeyboardState = previousKeyBoardState != keyboardState;

                if(endOfPrompt && exceededKeysPressed && pressingCurrentKey && changedKeyboardState)
                {
                    TypeChar();
                }

                previousKeyBoardState = keyboardState;
                return GameState.DayLetter;
            }   
        }

        public void UpdateDay()
        {
            Debug.WriteLine("currentPromptIndex: " + currentPromptIndex);
            KeyboardState keyboardState = Keyboard.GetState();
            bool endOfPrompt;
            bool exceededKeysPressed;
            bool pressingCurrentKey;
            bool changedKeyboardState;

            if(currentLineIndex == -1)
            {
                if(currentPromptIndex == 0)
                {
                    currentPromptIndex = 1;
                }
                else if(currentPromptIndex == 1 && day > 5)
                {
                    currentPromptIndex = 2;
                }
                currentLineIndex = 0;
                currentCharIndex = 0;
            }
            else
            {
                endOfPrompt = currentLineIndex != -1;
                exceededKeysPressed = GetPressedLetterCount(keyboardState) <= 2;
                pressingCurrentKey = keyboardState.IsKeyDown(GetKeyFromChar(CurrentPrompt[currentLineIndex].Substring(currentCharIndex, 1)));
                changedKeyboardState = previousKeyBoardState != keyboardState;

                if(endOfPrompt && exceededKeysPressed && pressingCurrentKey && changedKeyboardState)
                {
                    TypeChar();
                }
            }

            previousKeyBoardState = keyboardState;
        }

        public void DrawDayLetter(SpriteBatch spriteBatch, SpriteFont font, SpriteFont fontBold, Texture2D whitePixel, int screenWidth, int screenHeight)
        {
            for(int i = 0; i < letterPrompt.Count; i++)
            {

                if(i < currentLineIndex)
                {
                    spriteBatch.DrawString(font, letterPrompt[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
                }
                else if(i > currentLineIndex)
                {
                    letterPrompt[i] = letterPrompt[i].TrimEnd();
                    bool flag = true;

                    for(int j = 0; j < letterPrompt[i].Length; j++)
                    {
                        if(!font.Characters.Contains(letterPrompt[i][j]))
                        {
                            flag = false;
                        }
                    }

                    if(flag)
                    { 
                        spriteBatch.DrawString(font, letterPrompt[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                    }
                }
                else
                {
                    spriteBatch.DrawString(font, letterPrompt[i].Substring(0, letterPrompt[i].Length), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                    spriteBatch.DrawString(fontBold, letterPrompt[i].Substring(0, currentCharIndex), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                    spriteBatch.Draw(whitePixel, new Rectangle(0, lineSpacing * i + verticalOffset, (int)font.MeasureString(letterPrompt[i].Substring(0, currentCharIndex)).X + 550, 35), Color.White);
                    ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, screenWidth, 35), new Color(255, 255, 255));
                }

            }
        }

        public void DrawDay(SpriteBatch spriteBatch, SpriteFont font, SpriteFont fontBold, Texture2D whitePixel)
        {
            for(int i = 0; i < CurrentPrompt.Count; i++)
            {
                if(i < currentLineIndex)
                {
                    spriteBatch.DrawString(font, CurrentPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
                }
                else if(i > currentLineIndex)
                {
                    CurrentPrompt[i] = CurrentPrompt[i].TrimEnd();
                    bool flag = true;

                    for(int j = 0; j < CurrentPrompt[i].Length; j++)
                    {
                        if(!font.Characters.Contains(CurrentPrompt[i][j]))
                        {
                            flag = false;
                        }
                    }

                    if(flag)
                    {
                        spriteBatch.DrawString(font, CurrentPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                    }
                }
                else
                {
                    spriteBatch.DrawString(font, CurrentPrompt[i].Substring(0, CurrentPrompt[i].Length), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                    spriteBatch.DrawString(fontBold, CurrentPrompt[i].Substring(0, currentCharIndex), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                    spriteBatch.Draw(whitePixel, new Rectangle(0, lineSpacing * i + verticalOffset, (int)font.MeasureString(CurrentPrompt[i].Substring(0, currentCharIndex)).X + 75, 35), Color.White);
                    ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, typingWindow.Width, 35), new Color(255, 255, 255));
                }

            }
        }





        // UTILITY METHODS

        public void ScrapeFile(String filepath, int day)
        {
            StreamReader reader = new StreamReader(filepath);
            reader.ReadLine();
            String currentScrapedLine = reader.ReadLine();

            while(!currentScrapedLine.Contains("##"))
            {
                letterPrompt.Add(currentScrapedLine);
                currentScrapedLine = reader.ReadLine();
            }

            currentScrapedLine = reader.ReadLine();
            while(!currentScrapedLine.Contains("##"))
            {
                taskOnePrompt.Add(currentScrapedLine);
                currentScrapedLine = reader.ReadLine();
            }

            currentScrapedLine = reader.ReadLine();
            while(!currentScrapedLine.Contains("##"))
            {
                taskTwoPrompt.Add(currentScrapedLine);
                currentScrapedLine = reader.ReadLine();
            }

            if(day > 5)
            {
                reader.ReadLine();
                while(!currentScrapedLine.Contains("##"))
                {
                    taskThreePrompt.Add(currentScrapedLine);
                    currentScrapedLine = reader.ReadLine();
                }
            }

            reader.Close();
        }

        public int GetPressedLetterCount(KeyboardState keyboardState)
        {
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            int counter = 0;

            for(int i = 0; i < pressedKeys.Length; i++)
            {
                if(letterKeys.Contains(pressedKeys[i]))
                    counter++;
            }

            return counter;
        }

        public Keys GetKeyFromChar(String character)
        {
            character = character.ToLower();

            if(stringToKeys.ContainsKey(character))
                return stringToKeys[character];
            else
                return Keys.None;
        }

        public void TypeChar()
        {
            if(currentLineIndex != -1)
            {
                currentCharIndex++;
                MoveToNextAvailableChar();
            }
        }

        public void MoveToNextAvailableChar()
        {
            if(currentLineIndex != -1)
            {
                if(currentCharIndex == CurrentPrompt[currentLineIndex].Length)
                {
                    MoveToNextLine();
                }

                if(currentLineIndex != -1)
                {
                    while(skippedCharacters.Contains(CurrentPrompt[currentLineIndex][currentCharIndex].ToString()))
                    {
                        currentCharIndex++;

                        if(currentCharIndex == CurrentPrompt[currentLineIndex].Length)
                        {
                            MoveToNextLine();
                        }
                    }
                }
            }
        }

        public void MoveToNextLine()
        {
            if(currentLineIndex != -1)
            {
                currentCharIndex = 0;

                if(currentLineIndex == letterPrompt.Count - 1)
                {
                    currentLineIndex = -1;
                }
                else
                {
                    currentLineIndex++;
                    MoveToNextAvailableChar();
                }

                if(currentLineIndex != -1)
                {
                    while(letterPrompt[currentLineIndex] == "")
                    {
                        if(currentLineIndex == letterPrompt.Count)
                        {
                            currentLineIndex = -1;
                        }
                        else
                        {
                            currentLineIndex++;
                        }
                    }
                }
            }
        }
    }
}