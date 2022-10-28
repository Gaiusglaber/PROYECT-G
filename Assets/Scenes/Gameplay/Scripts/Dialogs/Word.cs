using ProyectG.Toolbox.Lerpers;
using UnityEngine;


namespace Pathfinders.Common.UI.Dialogs
{
    public class Word
    {
        #region PRIVATE_FIELDS
        private string finalResult = string.Empty;
        private char word;
        private int actualSize = 0;
        private int initialSize = 0;
        private int finalSize = 0;
        private FloatLerper sizeLerper = null;
        private ColorLerper colorLerper = null;

        private Color actualColor = Color.white;
        private Color startColor = Color.white;
        private Color endColor = Color.white;

        private bool usingSizeLerper = false;
        private bool usingColorLerper = false;
        #endregion

        #region PRIVATE_METHODS
        private string GetHexFromColor(Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGBA(color);
        }
        #endregion

        #region PUBLIC_METHODS
        public void UpdateWord()
        {
            if (sizeLerper != null && usingSizeLerper)
            {
                sizeLerper.Update();

                if (sizeLerper.On)
                {
                    actualSize = (int)sizeLerper.CurrentValue;
                    finalResult = "<size=" + actualSize.ToString() + ">" + word + "</size>";
                }

                if (sizeLerper.Reached)
                {
                    finalResult = "<size=" + finalSize.ToString() + ">" + word + "</size>";
                    sizeLerper.SwitchState(false);
                }
            }

            if (colorLerper != null && usingColorLerper)
            {
                colorLerper.Update();

                if (colorLerper.On)
                {
                    actualColor = colorLerper.CurrentValue;
                    finalResult = "<color=" + GetHexFromColor(actualColor) + ">" + finalResult + "</color>";
                }

                if (colorLerper.Reached)
                {
                    finalResult = "<color=" + GetHexFromColor(endColor) + ">" + finalResult + "</color>";
                    colorLerper.SwitchState(false);
                }
            }
        }

        public bool IsLerpEnded()
        {
            if (usingSizeLerper && !usingColorLerper)
            {
                return sizeLerper.Reached;
            }
            else if(usingSizeLerper && usingColorLerper)
            {
                return sizeLerper.Reached && colorLerper.Reached;
            }
            else if(usingColorLerper && !usingSizeLerper)
            {
                return colorLerper.Reached;
            }
            else
            {
                return true;
            }
        }

        public void SetColorsWord(Color startColor, Color endColor)
        {
            this.startColor = startColor;
            this.endColor = endColor;

            actualColor = this.startColor;
        }

        public void Init(char word, float sizeText, FloatLerper sizeLerper, ColorLerper colorLerper, bool lerpSize, bool lerpColors)
        {
            this.word = word;

            actualSize = initialSize;
            finalSize = (int)sizeText;

            this.sizeLerper = sizeLerper;
            this.colorLerper = colorLerper;

            usingSizeLerper = lerpSize;
            usingColorLerper = lerpColors;

            if (!usingSizeLerper)
            {
                finalResult = "<size=" + finalSize.ToString("0") + ">" + word + "</size>";
            }
            else
            {
                finalResult = "<size=" + actualSize + ">" + word + "</size>";
            }

            if (this.sizeLerper != null)
            {
                this.sizeLerper.SetValues(initialSize, finalSize, lerpSize);                
            }
            if (this.colorLerper != null)
            {
                this.colorLerper.SetValues(startColor, endColor, lerpColors);
            }
        }

        public char GetWordAttach()
        {
            return word;
        }

        public string GetWordResult()
        {
            return finalResult;
        }
        #endregion
    }
}