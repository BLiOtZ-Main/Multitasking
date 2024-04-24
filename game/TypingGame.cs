using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Multitasking
{
    enum PromptState { Body, LastLine, End }
    enum LineState { Body, LastChar, End }

    internal class TypingGame
    {
        // controls
        private KeyboardState previousKeyBoardState;

        // prompts
        private List<String> allPrompts;
        private List<String> letterPrompt;
        private List<List<String>> taskPrompts;

        // gameplay
        private GameState currentState;
        private Dictionary<String, Keys> stringToKeys;
        private List<String> skippedCharacters;
        private List<Keys> letterKeys;
        private bool inTutorial;
        private int currentLineIndex;
        private int currentCharIndex;

        // aesthetics
        private Rectangle typingWindow;
        private const int lineSpacing = 60;
        private const int verticalOffset = 150;

        private List<String> CurrentPrompt
        {
            get
            {
                if(inTutorial)
                    return letterPrompt;

                // CHANGE THIS vvv (currently hardcoded, it should go the current task prompt)
                return taskPrompts[0];
            }
        }

        private PromptState CurrentPromptState
        {
            get
            {
                if(currentLineIndex < CurrentPrompt.Count - 1) 
                    return PromptState.Body;
                else if (currentLineIndex == CurrentPrompt.Count - 1) 
                    return PromptState.LastLine;
                else 
                    return PromptState.End;
            }
        }

        private LineState CurrentLineState
        {
            get
            {
                if(currentCharIndex < CurrentPrompt[currentLineIndex].Length) 
                    return LineState.Body;
                else if(currentCharIndex == CurrentPrompt[currentLineIndex].Length) 
                    return LineState.LastChar;
                else 
                    return LineState.End;
            }
        }


        public TypingGame(Rectangle window, int day)
        {
            // initializing variables
            allPrompts = new List<String>();
            letterPrompt = new List<String>();
            taskPrompts = new List<List<String>>();
            currentState = new GameState();
            skippedCharacters = new List<String>() { " ", ".", "!", "?", "'", ",", "^" };
            letterKeys = new List<Keys>() { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                                            Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z};
            stringToKeys = new Dictionary<string, Keys>() { { "a", Keys.A }, { "b", Keys.B }, { "c", Keys.C }, { "d", Keys.D }, { "e", Keys.E }, { "f", Keys.F },
                                                            { "g", Keys.G }, { "h", Keys.H }, { "i", Keys.I }, { "j", Keys.J }, { "k", Keys.K }, { "l", Keys.L },
                                                            { "m", Keys.M }, { "n", Keys.N }, { "o", Keys.O }, { "p", Keys.P }, { "q", Keys.Q }, { "r", Keys.R },
                                                            { "s", Keys.S }, { "t", Keys.T }, { "u", Keys.U }, { "v", Keys.V }, { "w", Keys.W }, { "x", Keys.X },
                                                            { "y", Keys.Y }, { "z", Keys.Z }};
            inTutorial = true;
            typingWindow = new Rectangle(window.X, window.Y, window.Width, window.Height);
            currentLineIndex = 0;
            currentCharIndex = 0;

            // scrape files for prompts
            ScrapeFile("../../../../resources/prompts/day" + day + ".txt");
            MoveToNextAvailableChar();
            Debug.WriteLine("letterPromptCount: " + letterPrompt.Count);
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
            KeyboardState keyboardState = Keyboard.GetState();

            bool endOfPrompt;
            bool exceededKeysPressed;
            bool pressingCurrentKey;
            bool changedKeyboardState;

            if(currentLineIndex == -1)
            {
                previousKeyBoardState = keyboardState;
                // move to next document
            }
            else
            {
                while(letterPrompt[currentLineIndex].Length == 0)
                {
                    MoveToNextLine();
                }

                while(skippedCharacters.Contains(letterPrompt[currentLineIndex][currentCharIndex].ToString()))
                {
                    if(CurrentLineState == LineState.Body)
                        currentCharIndex++;

                    if(CurrentLineState == LineState.LastChar)
                    {
                        MoveToNextLine();
                    }
                }

                endOfPrompt = CurrentPromptState != PromptState.End;
                exceededKeysPressed = GetPressedLetterCount(keyboardState) <= 2;
                pressingCurrentKey = keyboardState.IsKeyDown(GetKeyFromChar(letterPrompt[currentLineIndex].Substring(currentCharIndex, 1)));
                changedKeyboardState = previousKeyBoardState != keyboardState;

                if(endOfPrompt && exceededKeysPressed && pressingCurrentKey && changedKeyboardState)
                {
                    if(CurrentLineState == LineState.Body)
                        currentCharIndex++;

                    if(CurrentLineState == LineState.LastChar)
                    {
                        currentCharIndex = 0;

                        if(CurrentPromptState == PromptState.LastLine)
                            currentLineIndex = -1;
                        else
                            currentLineIndex++;
                    }

                }

                previousKeyBoardState = keyboardState;


                previousKeyBoardState = keyboardState;
            }
        }

        public void DrawDayLetter(SpriteBatch spriteBatch, SpriteFont font, SpriteFont fontBold, Texture2D whitePixel)
        {
            for(int i = 0; i < letterPrompt.Count; i++)
            {

                if(i < currentLineIndex)
                {
                    spriteBatch.DrawString(font, letterPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
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
                        spriteBatch.DrawString(font, letterPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                    }
                }
                else
                {
                    spriteBatch.DrawString(font, letterPrompt[i].Substring(0, letterPrompt[i].Length), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                    spriteBatch.DrawString(fontBold, letterPrompt[i].Substring(0, currentCharIndex), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                    spriteBatch.Draw(whitePixel, new Rectangle(0, lineSpacing * i + verticalOffset, (int)font.MeasureString(letterPrompt[i].Substring(0, currentCharIndex)).X + 550, 35), Color.White);
                    ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, typingWindow.Width, 35), new Color(255, 255, 255));
                }

            }
        }

        public void DrawDay(SpriteBatch spriteBatch, SpriteFont font, SpriteFont fontBold, Texture2D whitePixel)
        {
            for(int i = 0; i < letterPrompt.Count; i++)
            {
                if(i < currentLineIndex)
                {
                    spriteBatch.DrawString(font, letterPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
                }
                else if(i > currentLineIndex)
                {
                    spriteBatch.DrawString(font, letterPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                }
                else
                {
                    spriteBatch.DrawString(font, letterPrompt[i].Substring(0, letterPrompt[i].Length), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                    spriteBatch.DrawString(fontBold, letterPrompt[i].Substring(0, currentCharIndex), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                    spriteBatch.Draw(whitePixel, new Rectangle(0, lineSpacing * i + verticalOffset, (int)font.MeasureString(letterPrompt[i].Substring(0, currentCharIndex)).X + 550, 35), Color.White);
                    ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, typingWindow.Width, 35), new Color(255, 255, 255));
                }

            }
        }





        // UTILITY METHODS

        public void ScrapeFile(String filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            reader.ReadLine();
            String currentScrapedLine = reader.ReadLine();

            while(!currentScrapedLine.Contains("##"))
            {
                letterPrompt.Add(currentScrapedLine);
                currentScrapedLine = reader.ReadLine();
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
            Debug.WriteLine("currentLineIndex: " + currentLineIndex);
            if(currentLineIndex != -1)
            {
                if(currentCharIndex == letterPrompt[currentLineIndex].Length)
                {
                    MoveToNextLine();
                }

                if(currentLineIndex != -1)
                {
                    while(skippedCharacters.Contains(letterPrompt[currentLineIndex][currentCharIndex].ToString()))
                    {
                        currentCharIndex++;

                        if(currentCharIndex == letterPrompt[currentLineIndex].Length)
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
                Debug.WriteLine("SPECIAL currentLineIndex: " + currentLineIndex);
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