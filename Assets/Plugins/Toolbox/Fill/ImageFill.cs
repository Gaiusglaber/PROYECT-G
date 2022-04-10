using UnityEngine;
using UnityEngine.UI;

using Pathfinders.Toolbox.Fill;

namespace Pathfinders.Toolbox.Utils.UI
{
    public class ImageFill : AbstractFill
    {
        #region EXPOSED_FIELDS        
        [SerializeField] private Image image = null;
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if(floatLerper == null)
            {
                return;
            }

            if(floatLerper.On)
            {
                image.fillAmount = floatLerper.CurrentValue;

                if(useColorLerper)
                {
                    image.color = gradient.Evaluate(floatLerper.CurrentValue);
                }
            }
        }
        #endregion

        #region INIT
        public override void Init()
        {
        }
        #endregion

        #region OVERRIDED_METHODS
        public override void UpdateValue(float amount, bool instant = false)
        {
            float aux = amount / finalValue;

            if (!instant)
            {
                floatLerper.SetValues(image.fillAmount, aux, true);
            }
            else
            {
                image.fillAmount = aux;

                if (useColorLerper)
                {
                    image.color = gradient.Evaluate(floatLerper.CurrentValue);
                }
            }
        }

        public override void SetValues(float newFinalValue, float startValue = 0)
        {           
            finalValue = newFinalValue;

            if (startValue > finalValue)
            {
                image.fillAmount = 1;
            }
            else if(startValue < 0)
            {
                image.fillAmount = 0;
            }
            else
            {
                float aux = startValue / finalValue;
                image.fillAmount = aux;
            }

            if (useColorLerper)
            {
                image.color = gradient.Evaluate(image.fillAmount);
            }
        }
        #endregion
    }
}