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

        /// <summary>
        /// returns whether the player is currently typing the letter,
        /// prompt one, prompt two, or prompt three.
        /// </summary>
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

        /// <summary>
        /// Sole constructor for the typing simulator. 
        /// </summary>
        /// <param name="window">the space in the current window that the typing simulator will be displayed</param>
        /// <param name="day">the current day of gameplay</param>
        public TypingGame(Rectangle window, int day)
        {
            // initializing variables
            letterPrompt = new List<String>();
            taskOnePrompt = new List<String>();
            taskTwoPrompt = new List<String>();
            taskThreePrompt = new List<String>();
            currentState = new GameState();
            skippedCharacters = new List<String>() { " ", ".", "!", "?", "'", ",", "^", "$" };
            letterKeys = new List<Keys>() { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                                            Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z};
            stringToKeys = new Dictionary<string, Keys>() { { "a", Keys.A }, { "b", Keys.B }, { "c", Keys.C }, { "d", Keys.D }, { "e", Keys.E }, { "f", Keys.F },
                                                            { "g", Keys.G }, { "h", Keys.H }, { "i", Keys.I }, { "j", Keys.J }, { "k", Keys.K }, { "l", Keys.L },
                                                            { "m", Keys.M }, { "n", Keys.N }, { "o", Keys.O }, { "p", Keys.P }, { "q", Keys.Q }, { "r", Keys.R },
                                                            { "s", Keys.S }, { "t", Keys.T }, { "u", Keys.U }, { "v", Keys.V }, { "w", Keys.W }, { "x", Keys.X },
                                                            { "y", Keys.Y }, { "z", Keys.Z }};
            typingWindow = new Rectangle(window.X, window.Y, window.Width, window.Height);
            currentPromptIndex = -1; // sets the current prompt to the letter
            currentLineIndex = 0;
            currentCharIndex = 0;
            this.day = day;

            // scrape files for prompts
            ScrapeFile("../../../../resources/prompts/day" + day + ".txt", day);
            MoveToNextAvailableChar();
        }


        // CORE GAME METHODS

        /// <summary>
        /// frame by frame updates to the typing simulator. mainly checks
        /// if the user is pressing the right keys or not, and progresses
        /// the current prompt accordingly.
        /// </summary>
        /// <param name="currentState">the current game state</param>
        /// <returns>the new (or same) game state</returns>
        public GameState UpdateDayLetter(GameState currentState)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            this.currentState = currentState;

            // conditions to make sure the player is typing the right characters and not abusing the system
            bool endOfPrompt;
            bool exceededKeysPressed;
            bool pressingCurrentKey;
            bool changedKeyboardState;

            // checking if the prompt has ended
            if(currentLineIndex == -1)
            {
                // if it has, move on to the first task prompt and reset the other variables accordingly
                currentPromptIndex = 0;
                currentLineIndex = 0;
                currentCharIndex = 0;
                previousKeyBoardState = keyboardState;
                return GameState.Day; // makes sure Game1 knows that we are no longer in the letter state
            }
            // this means we are in the middle of the prompt, not the end
            else
            {
                // updates conditionals to ensure player is typing the right character and not abusing the simulator
                endOfPrompt = currentLineIndex != -1;
                exceededKeysPressed = GetPressedLetterCount(keyboardState) <= 2;
                pressingCurrentKey = keyboardState.IsKeyDown(GetKeyFromChar(letterPrompt[currentLineIndex].Substring(currentCharIndex, 1)));
                changedKeyboardState = previousKeyBoardState != keyboardState;

                // checks if they pass every conditional
                if(endOfPrompt && exceededKeysPressed && pressingCurrentKey && changedKeyboardState)
                {
                    // "type" the current character
                    TypeChar();
                }

                previousKeyBoardState = keyboardState;
                return GameState.DayLetter; // makes sure Game1 knows we are still in the letter state, the day has NOT started
            }   
        }

        /// <summary>
        /// frame by frame updates to the task prompts, essentially
        /// the actual "non tutorial" gameplay.
        /// </summary>
        public void UpdateDay()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // conditions to make sure the player is typing the right characters and not abusing the system
            bool endOfPrompt;
            bool exceededKeysPressed;
            bool pressingCurrentKey;
            bool changedKeyboardState;

            // checking if the prompt has ended (unfortunately hard coded)
            if(currentLineIndex == -1)
            {
                // if it has, check if this is the first task prompt
                if(currentPromptIndex == 0)
                {
                    // if it is, move to the second task prompt
                    currentPromptIndex = 1;
                }
                // otherwise check if this is the second task prompt and that the day has more than just 2 prompts
                else if(currentPromptIndex == 1 && day > 5)
                {
                    // if it is, moves to the third task prompt
                    currentPromptIndex = 2;
                }
                currentLineIndex = 0;
                currentCharIndex = 0;
            }
            // this means we are in the middle of the prompt, not the end
            else
            {
                // updates conditionals to ensure player is typing the right character and not abusing the simulator
                endOfPrompt = currentLineIndex != -1;
                exceededKeysPressed = GetPressedLetterCount(keyboardState) <= 2;
                pressingCurrentKey = keyboardState.IsKeyDown(GetKeyFromChar(CurrentPrompt[currentLineIndex].Substring(currentCharIndex, 1)));
                changedKeyboardState = previousKeyBoardState != keyboardState;

                // checks if they pass every conditional
                if(endOfPrompt && exceededKeysPressed && pressingCurrentKey && changedKeyboardState)
                {
                    // "type" the current character
                    TypeChar();
                }
            }

            previousKeyBoardState = keyboardState;
        }

        /// <summary>
        /// draws the letter prompt frame by frame.
        /// </summary>
        /// <param name="spriteBatch">the SpriteBatch the method is going to draw with</param>
        /// <param name="font">the font the method is going to use to draw</param>
        /// <param name="fontBold">the bold version of the font that the method is going to use to draw</param>
        /// <param name="whitePixel">an image of a white pixel, which is used to draw some things that ShapeBatch can't</param>
        /// <param name="screenWidth">the width of the entire screen</param>
        /// <param name="screenHeight">the height of the entire screen</param>
        public void DrawDayLetter(SpriteBatch spriteBatch, SpriteFont font, SpriteFont fontBold, Texture2D whitePixel, int screenWidth, int screenHeight)
        {
            // goes through each line in the letter prompt
            for(int i = 0; i < letterPrompt.Count; i++)
            {
                // if the line has been typed
                if(i < currentLineIndex)
                {
                    // draw the line as yellow text
                    spriteBatch.DrawString(font, letterPrompt[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
                }
                // if the line is still to come
                else if(i > currentLineIndex)
                {
                    // draws the line as gray text
                    spriteBatch.DrawString(font, letterPrompt[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                }
                // this means we are on this line
                else
                {
                    // draws untyped text on this line as blue, typed text as white, a highlight for the current line, and another box on top to ensure the text is clean
                    spriteBatch.DrawString(font, letterPrompt[i].Substring(0, letterPrompt[i].Length), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                    spriteBatch.DrawString(fontBold, letterPrompt[i].Substring(0, currentCharIndex), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                    spriteBatch.Draw(whitePixel, new Rectangle(0, lineSpacing * i + verticalOffset, (int)font.MeasureString(letterPrompt[i].Substring(0, currentCharIndex)).X + 550, 35), Color.White);
                    ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, screenWidth, 35), new Color(255, 255, 255));
                }

            }
        }

        /// <summary>
        /// draws the task prompts frame by frame. uses the
        /// typing window parameters provided when the typing
        /// simulator was initialized, rather than the full
        /// screen.
        /// </summary>
        /// <param name="spriteBatch">the SpriteBatch the method is going to draw with</param>
        /// <param name="font">the font the method is going to use to draw</param>
        /// <param name="fontBold">the bold version of the font that the method is going to use to draw</param>
        /// <param name="whitePixel">an image of a white pixel, which is used to draw some things that ShapeBatch can't</param>
        public void DrawDay(SpriteBatch spriteBatch, SpriteFont font, SpriteFont fontBold, Texture2D whitePixel)
        {
            // goes through each line in the letter prompt
            for(int i = 0; i < CurrentPrompt.Count; i++)
            {
                // if the line has been typed
                if(i < currentLineIndex)
                {
                    // draw the line as yellow text
                    spriteBatch.DrawString(font, CurrentPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
                }
                // if the line is still to come
                else if(i > currentLineIndex)
                {
                    // draws the line as gray text
                    spriteBatch.DrawString(font, CurrentPrompt[i], new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                }
                // this means we are on this line
                else
                {
                    // draws untyped text on this line as blue, typed text as white, a highlight for the current line, and another box on top to ensure the text is clean
                    spriteBatch.DrawString(font, CurrentPrompt[i].Substring(0, CurrentPrompt[i].Length), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                    spriteBatch.DrawString(fontBold, CurrentPrompt[i].Substring(0, currentCharIndex), new Vector2((typingWindow.Width / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                    spriteBatch.Draw(whitePixel, new Rectangle(0, lineSpacing * i + verticalOffset, (int)font.MeasureString(CurrentPrompt[i].Substring(0, currentCharIndex)).X + 75, 35), Color.White);
                    ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, typingWindow.Width, 35), new Color(255, 255, 255));
                }

            }
        }





        // UTILITY METHODS

        /// <summary>
        /// scrapes a text file and puts the prompts contained
        /// inside of it into a given list.
        /// </summary>
        /// <param name="filepath">the path to the text file</param>
        /// <param name="day">the current day</param>
        public void ScrapeFile(String filepath, int day)
        {
            // starts the StreamReader
            StreamReader reader = new StreamReader(filepath);
            reader.ReadLine();
            String currentScrapedLine = reader.ReadLine();

            // scrapes the letter
            while(!currentScrapedLine.Contains("##"))
            {
                letterPrompt.Add(currentScrapedLine);
                currentScrapedLine = reader.ReadLine();
            }

            // scrapes task one
            currentScrapedLine = reader.ReadLine();
            while(!currentScrapedLine.Contains("##"))
            {
                taskOnePrompt.Add(currentScrapedLine);
                currentScrapedLine = reader.ReadLine();
            }

            // scrapes task two
            currentScrapedLine = reader.ReadLine();
            while(!currentScrapedLine.Contains("##"))
            {
                taskTwoPrompt.Add(currentScrapedLine);
                currentScrapedLine = reader.ReadLine();
            }

            // scrapes task three (if it exists)
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

        /// <summary>
        /// gets the number of letters on the player's
        /// keyboard that was pressed this frame.
        /// </summary>
        /// <param name="keyboardState">the player's current KeyboardState</param>
        /// <returns>the number of letters the player pressed this frame</returns>
        public int GetPressedLetterCount(KeyboardState keyboardState)
        {
            // gathers every key the player pressed
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            int counter = 0;

            // goes through every key the player pressed
            for(int i = 0; i < pressedKeys.Length; i++)
            {
                // if its a letter, incrememnt the count by one
                if(letterKeys.Contains(pressedKeys[i]))
                    counter++;
            }

            // return the count
            return counter;
        }

        /// <summary>
        /// takes in a character and returns the key
        /// associated with it.
        /// </summary>
        /// <param name="character">a character</param>
        /// <returns>the key associated with the character</returns>
        public Keys GetKeyFromChar(String character)
        {
            character = character.ToLower();

            if(stringToKeys.ContainsKey(character))
                return stringToKeys[character];
            else
                return Keys.None;
        }

        /// <summary>
        /// types a character in the typing simulator
        /// </summary>
        public void TypeChar()
        {
            // checks if we are actually within a prompt
            if(currentLineIndex != -1)
            {
                // if we are, increments the character positon by one and moves to the next typeable character
                currentCharIndex++;
                MoveToNextAvailableChar();
            }
        }

        /// <summary>
        /// moves the current character to the next one
        /// that the player is allowed to type. in practice
        /// this means ignoring spaces, punctuation, and 
        /// new lines.
        /// </summary>
        public void MoveToNextAvailableChar()
        {
            // checks if we are actually within a prompt
            if(currentLineIndex != -1)
            {
                // if we are, checks if that was the last character
                if(currentCharIndex == CurrentPrompt[currentLineIndex].Length)
                {
                    // if it was, move to the next line
                    MoveToNextLine();
                }

                // checks if we are now at the end of the prompt
                if(currentLineIndex != -1)
                {
                    // if we are, skips characters until we arrive at a typeable one
                    while(skippedCharacters.Contains(CurrentPrompt[currentLineIndex][currentCharIndex].ToString()))
                    {
                        currentCharIndex++;

                        // if we hit the end of a line, moves to the next line
                        if(currentCharIndex == CurrentPrompt[currentLineIndex].Length)
                        {
                            MoveToNextLine();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// moves the current character to a new line
        /// and resets all variables accordingly.
        /// </summary>
        public void MoveToNextLine()
        {
            // checks to make sure we are still in the prompt
            if(currentLineIndex != -1)
            {
                // if we are, reset the char index
                currentCharIndex = 0;

                // if this is the end of the prompt, update the line index accordingly
                if(currentLineIndex == CurrentPrompt.Count - 1)
                {
                    currentLineIndex = -1;
                }
                // otherwise just increment the line index by one and move to the next typeable character
                else
                {
                    currentLineIndex++;
                    MoveToNextAvailableChar();
                }

                // if we are not at the end of the prompt
                if(currentLineIndex != -1)
                {
                    // skips passed any empty lines
                    while(CurrentPrompt[currentLineIndex] == "")
                    {
                        // if we hit the end of the prompt, update the line index accordingly
                        if(currentLineIndex == CurrentPrompt.Count)
                        {
                            currentLineIndex = -1;
                        }
                        // otherwise, just increment the line index
                        else
                        {
                            Debug.WriteLine("========NEW LINE========== (empty line)");
                            currentLineIndex++;
                            MoveToNextAvailableChar();
                        }
                    }
                }
            }
        }
    }
}