using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjetoMulti
{
    class DemoScreen
    {
        private Letter letterBuffer;
        private List<Letter> outputString;
        private Vector2 scale;
        private Color letterColor;

        private bool clearButtonPressed;

        public DemoScreen()
        {
            outputString = new List<Letter>();
            outputString.Add(new Letter("T", 0.5, 0.5, 1000));
            outputString.Add(new Letter("_", 0.5, 0.5, 1000));
            outputString.Add(new Letter("T", 0.5, 0.5, 1000));
        }

        public void UpdateLetter()
        {
            if (letterBuffer != null)
            {
                outputString.Add(letterBuffer);
                letterBuffer = null;
            }
        }

        public void AddLetter(string character, double scaleX, double scaleY, double duration)
        {
            letterBuffer = new Letter(character, scaleX, scaleY, duration);
        }


        public void Clear()
        {
            clearButtonPressed = true;
        }

        public bool IsClearButtonPressed()
        {
            return clearButtonPressed;
        }

        public void ResetString()
        {
            outputString = new List<Letter>();
        }

        public List<Letter> GetOutputString()
        {
            return outputString;
        }
    }
}
