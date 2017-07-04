using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VocalisBot
{
    [Serializable]
    public class Hangman
    {
        public List<char> GuessedCharacters = new List<char>();
        public string CurrentPic { get { return _hangmanPics[GuessedCharacters.Count]; } }
        public string CurrentWord { get; private set; }
        public StringBuilder HiddenWord { get { return _hiddenWord; } }

        private StringBuilder _hiddenWord;

        public Hangman()
        {
            var words = new List<string>() { "robot", "chatbot", "azure", "cognitive", "services", "microsoft" };

            var r = new Random();
            var pos = r.Next(words.Count);
            CurrentWord = words[pos];
            
            _hiddenWord = new StringBuilder();
            for(var i = 0; i < CurrentWord.Length; i++)
            {
                _hiddenWord.Append('.');
            }
        }

        public bool IsCorrectGuess(char guess)
        {
            var wordChars = CurrentWord.ToCharArray();
            var result = false;
            for (var i = 0; i < wordChars.Length; i++)
            {
                var wordChar = wordChars[i];
                if (wordChar == guess)
                {
                    _hiddenWord[i] = wordChar;
                    result = true;
                }
            }

            if(result == false)
            {
                GuessedCharacters.Add(guess);
            }

            return result;
        }

        public bool GameWon
        {
            get
            {
                return !_hiddenWord.ToString().Contains('.');
            }
        }

        public bool GameLost
        {
            get
            {
                return GuessedCharacters.Count == 7;
            }
        }



        private List<string> _hangmanPics = new List<string>()
        {
@"```
   ____
  |    |
  |
  |
  |
  |
 _|_
|   |______
```",
@"```
   ____
  |    |
  |    o
  |
  |
  |
 _|_
|   |______
```",
@"```
   ____
  |    |
  |    o
  |    |
  |
  |
 _|_
|   |______
```",
@"```
   ____
  |    |
  |    o
  |   /|
  |
  |
 _|_
|   |______
```",
@"```
   ____
  |    |
  |    o
  |   /|\
  |
  |
 _|_
|   |______
```",
@"```
   ____
  |    |
  |    o
  |   /|\
  |    |
  |
 _|_
|   |______
```",
@"```
   ____
  |    |
  |    o
  |   /|\
  |    |
  |   / 
 _|_
|   |______
```",
@"```
   ____
  |    |
  |    o
  |   /|\
  |    |
  |   / \
 _|_
|   |______
```"
        };
    }
}