using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Common.Utils.Lighting
{
    public class LightFlasher : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField, Range(0.25f, 5f)] private float flickInterval = 0f;
        [SerializeField] private float maxIntensity = 2.15f;
        [SerializeField] private float minIntensity = 1.0f;
        [SerializeField] private FloatLerper.SMOOTH_TYPE smoothType = 0f;
        #endregion

        #region PRIVATE_FIELDS
        private Light2D lightAttach = null;
        private FloatLerper lerper = null;
        #endregion
        
        #region UNITY_CALLS
        private void Start()
        {
            lightAttach = GetComponent<Light2D>();
            
            lerper = new FloatLerper(flickInterval, smoothType);
            
            LerpToMinIntensity();
        }

        private void Update()
        {
            if (lerper.On)
            {
                lerper?.Update();
            }
        }
        #endregion

        #region PRIVATE_FIELDS
        private void LerpToMaxIntensity()
        {
            StartCoroutine(LerpToIntensity(maxIntensity, minIntensity, LerpToMinIntensity));
        }

        private void LerpToMinIntensity()
        {
            StartCoroutine(LerpToIntensity(minIntensity, maxIntensity, LerpToMaxIntensity));
        }
        #endregion

        #region COROUTINES
        private IEnumerator LerpToIntensity(float startValue, float endValue, Action onReached = null)
        {
            if (lerper == null || lightAttach == null)
            {
                yield break;
            }
            
            lerper.SetValues(startValue, endValue, true);

            while (!lerper.Reached)
            {
                lerper.Update();

                lightAttach.intensity = lerper.CurrentValue;
                
                yield return null;
            }
            
            onReached?.Invoke();
        }
        #endregion
    }
}