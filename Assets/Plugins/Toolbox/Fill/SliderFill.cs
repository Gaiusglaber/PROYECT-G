using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Pathfinders.Toolbox.Fill;

namespace Pathfinders.Toolbox.Utils.UI
{    
    public class SliderFill : AbstractFill
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Slider slider = null;
        #endregion

        #region PRIVATE_FIELDS
        private ColorBlock colorBlock = new ColorBlock();
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if (floatLerper == null)
            {
                return;
            }

            if (floatLerper.On)
            {
                slider.value = floatLerper.CurrentValue;

                if(useColorLerper)
                {
                    colorBlock.normalColor = gradient.Evaluate(floatLerper.CurrentValue / finalValue);
                    slider.colors = colorBlock;
                }
            }
        }
        #endregion

        #region INIT
        public override void Init()
        {
            colorBlock = slider.colors;
        }
        #endregion

        #region PUBLIC_METHODS
        public override void UpdateValue(float amount, bool instant = false)
        {
            if(!instant)
            {
                floatLerper.SetValues(slider.value, amount, true);
            }
            else
            {
                slider.value = amount;

                if (useColorLerper)
                {
                    colorBlock.normalColor = gradient.Evaluate(amount / finalValue);
                    slider.colors = colorBlock;
                }
            }
        }

        public override void SetValues(float newFinalValue, float startValue = 0)
        {
            finalValue = newFinalValue;

            slider.minValue = 0f;
            slider.maxValue = finalValue;

            if (startValue > finalValue)
            {
                slider.value = slider.maxValue;
            }
            else if (startValue < 0)
            {
                slider.value = slider.minValue;
            }
            else
            {
                float aux = startValue / finalValue;
                slider.value = aux;
            }

            colorBlock.normalColor = gradient.Evaluate(finalValue);
            slider.colors = colorBlock;
        }

        public void OnValueChanged(UnityAction<float> onValueChanged)
        {
            slider.onValueChanged.AddListener(onValueChanged);
        }
        #endregion
    }
}