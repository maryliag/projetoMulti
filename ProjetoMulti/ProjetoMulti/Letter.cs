using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace ProjetoMulti
{
    class Letter
    {
        private static float MAX_DURATION = 2500;
        private static Vector3 BASE_COLOR = new Vector3(0, 51, 102);
        private string character;
        private Vector2 scale;
        private Color letterColor;


        public Letter(string character, double scaleX, double scaleY, double duration)
        {
            scale = new Vector2();
            this.character = character;
            scale.X = (float)scaleX;
            scale.Y = (float)scaleY;
            letterColor = new Color(new Vector4(BASE_COLOR, extractAlpha(duration)));
        }

        private int extractAlpha(double duration)
        {
            int durationScaled = (int)(duration * 100 / MAX_DURATION);

            if (durationScaled > 100)
            {
                durationScaled = 100;
            }
            return 100 - durationScaled;
        }

        public Vector2 getScale()
        {
            return scale;
        }

        public string getText()
        {
            return character;
        }

        public Color getColor()
        {
            return letterColor;
        }
    }
}
