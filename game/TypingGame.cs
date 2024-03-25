using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Multitasking
{
    enum PromptState { Body, LastLine, End }
    enum LineState { Body, LastChar, End }

    internal class TypingGame
    {
        // controls
        private KeyboardState previousKeyBoardState;

        // prompts
        private List<String> tutorialPrompts;
        private List<String> gamePrompts;

        // gameplay
        private bool inTutorial;
        private int currentLineIndex;
        private int currentCharIndex;

        // aesthetics
        private const int lineSpacing = 60;
        private const int verticalOffset = 150;

        private List<String> CurrentPrompt
        {
            get
            {
                if(inTutorial) 
                    return tutorialPrompts;
                else 
                    return gamePrompts;
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
                if(currentCharIndex < CurrentPrompt[currentLineIndex].Length - 1) 
                    return LineState.Body;
                else if(currentCharIndex == CurrentPrompt[currentLineIndex].Length - 1) 
                    return LineState.LastChar;
                else 
                    return LineState.End;
            }
        }


        public TypingGame()
        {
            // initializing variables
            tutorialPrompts = new List<String>();
            gamePrompts = new List<String>();
            inTutorial = true;
            currentLineIndex = 0;
            currentCharIndex = 0;

            // scrape files for prompts
            ScrapeFile(tutorialPrompts, "../../../../resources/prompts/tutorialPrompts.txt");
            ScrapeFile(gamePrompts, "../../../../resources/prompts/gamePrompts.txt");
        }


        // CORE GAME METHODS

        public void UpdateTypingTutorial()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            bool notEndOfPrompt = CurrentPromptState != PromptState.End;
            bool fewEnoughLettersPressed = GetPressedLetterCount(keyboardState) <= 2;
            bool pressingCurrentLetter = keyboardState.IsKeyDown(GetKeyFromChar(tutorialPrompts[currentLineIndex].Substring(currentCharIndex, 1)));
            bool differentKeyboardState = previousKeyBoardState != keyboardState;
            bool newLine = false;

            if(notEndOfPrompt && fewEnoughLettersPressed && pressingCurrentLetter && differentKeyboardState)
            {
                if(CurrentLineState == LineState.Body)
                    currentCharIndex++;

                if(CurrentLineState == LineState.LastChar)
                {
                    currentCharIndex = 0;
                    newLine = true;
                }
                    
            }

            while(tutorialPrompts[currentLineIndex][currentCharIndex].ToString() == " " ||
                  tutorialPrompts[currentLineIndex][currentCharIndex].ToString() == "." ||
                  tutorialPrompts[currentLineIndex][currentCharIndex].ToString() == "!" ||
                  tutorialPrompts[currentLineIndex][currentCharIndex].ToString() == "'" ||
                  tutorialPrompts[currentLineIndex][currentCharIndex].ToString() == "," )
            {
                if(CurrentLineState == LineState.Body)
                    currentCharIndex++;

                if(CurrentLineState == LineState.LastChar)
                {
                    currentCharIndex = 0;
                    newLine = true;
                }
                    
            }

            if(newLine)
            {
                newLine = false;

                if(CurrentPromptState == PromptState.LastLine)
                    currentLineIndex = -1;
                else
                    currentLineIndex++;
            }

            previousKeyBoardState = keyboardState;
        }

        public void UpdateTypingGame()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            // typing game code
            previousKeyBoardState = keyboardState;
        }

        public void DrawTypingTutorial(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
        {
            for(int i = 0; i < tutorialPrompts.Count; i++)
            {
                if(i < currentLineIndex)
                {
                    spriteBatch.DrawString(font, tutorialPrompts[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Yellow);
                }
                else if(i > currentLineIndex)
                {
                    spriteBatch.DrawString(font, tutorialPrompts[i], new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.Gray);
                }
                else
                {
                    spriteBatch.DrawString(font, tutorialPrompts[i].Substring(0, tutorialPrompts[i].Length), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), new Color(31, 0, 171));
                    spriteBatch.DrawString(font, tutorialPrompts[i].Substring(0, currentCharIndex), new Vector2((screenWidth / 2) - 400, lineSpacing * i + verticalOffset), Color.White);
                    ShapeBatch.Box(new Rectangle(0, lineSpacing * i + verticalOffset, screenWidth, 35), new Color(255, 255, 255));
                }

            }
        }

        public void DrawTypingGame(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
        {
            // draw typing game
        }





        // UTILITY METHODS

        public void ScrapeFile(List<String> prompts, String filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            String currentLineIndex= reader.ReadLine();

            while(currentLineIndex!= null)
            {
                String lineToAdd = "";

                for(int i = 0; i < currentLineIndex.Length; i++)
                {
                    lineToAdd += currentLineIndex[i];
                }

                prompts.Add(lineToAdd);
                currentLineIndex= reader.ReadLine();
            }

            reader.Close();
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

        public Keys GetKeyFromChar(String character)
        {
            character = character.ToLower();

            switch(character)
            {
                case "a": return Keys.A; case "b": return Keys.B; case "c": return Keys.C; case "d": return Keys.D;
                case "e": return Keys.E; case "f": return Keys.F; case "g": return Keys.G; case "h": return Keys.H;
                case "i": return Keys.I; case "j": return Keys.J; case "k": return Keys.K; case "l": return Keys.L;
                case "m": return Keys.M; case "n": return Keys.N; case "o": return Keys.O; case "p": return Keys.P;
                case "q": return Keys.Q; case "r": return Keys.R; case "s": return Keys.S; case "t": return Keys.T;
                case "u": return Keys.U; case "v": return Keys.V; case "w": return Keys.W; case "x": return Keys.X;
                case "y": return Keys.Y; case "z": return Keys.Z;
            }

            return Keys.None;
        }
    }
}