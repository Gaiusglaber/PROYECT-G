using UnityEngine;

using System.Collections.Generic;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Minigames.Common.Toolbox
{
    public class ColorFlasherBase : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("Main Settings")]
        [SerializeField] protected bool startActive = false;
        [SerializeField] protected Color currentUsedColor = Color.white;
        [SerializeField] protected bool loop = false;
        [SerializeField] protected bool includeStartColor = false;

        [Header("Multiple Colors Config")]
        [SerializeField] protected Color[] staticColorsToLerp = null;
        [SerializeField] protected float transitionTime = 1.0f;
        #endregion

        #region PROTECTED_FIELDS
        protected ColorLerper colorLerper = null;
        protected Color originalColorUsed = Color.white;
        protected Color targetColor = Color.white;
        protected List<Color> colorsToLerp = null;
        protected int currentColorIndex = 0;
        protected bool canStartLerp = false;
        #endregion

        #region PROPERTIES
        public Color CurrentUsedColor
        {
            get => currentUsedColor;
            set => currentUsedColor = value;
        }
        public Color OriginalColorUsed
        {
            get => originalColorUsed;
            set => originalColorUsed = value;
        }
        #endregion

        #region UNITY_CALLS
        protected virtual void Awake()
        {
            //originalColor = targetRenderer.color;
            colorLerper = new ColorLerper(transitionTime, AbstractLerper<Color>.SMOOTH_TYPE.STEP_SMOOTHER);
            if (startActive)
            {
                TriggerCustomColors(default, null);
            }
        }

        protected virtual void Update()
        {
            if (colorLerper.On)
            {
                colorLerper.Update();
                if (colorLerper.Reached)
                {
                    if (colorLerper.CurrentValue != OriginalColorUsed)
                    {
                        CheckMultipleColorsLerp(OriginalColorUsed);
                    }
                    else
                    {
                        if (loop)
                        {
                            CheckMultipleColorsLerp(targetColor);
                        }
                        else
                        {
                            CheckMultipleColorsLerp();
                        }
                    }
                }

                currentUsedColor = colorLerper.CurrentValue;
            }
        }
        #endregion

        #region PROTECTED_METHODS
        protected void CheckMultipleColorsLerp(Color targetColorSelected = default)
        {
            if (canStartLerp)
            {
                currentColorIndex++;
                if (currentColorIndex >= colorsToLerp.Count)
                {
                    canStartLerp = loop;
                    colorLerper.SetValues(colorsToLerp[currentColorIndex - 1], colorsToLerp[0], true);
                    currentColorIndex = 0;
                }
                else
                {
                    colorLerper.SetValues(colorsToLerp[currentColorIndex - 1], colorsToLerp[currentColorIndex], true);
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void TriggerCustomColors(Color newOriginalColor, params Color[] colors)
        {
            if (newOriginalColor != default)
            {
                OriginalColorUsed = newOriginalColor;
            }

            colorsToLerp = new List<Color>();

            Color[] colorArrayToAdd = (colors == null || colors.Length <= 0) ? staticColorsToLerp : colors;

            if (includeStartColor)
            {
                colorsToLerp.Add(OriginalColorUsed);
            }

            for (int i = 0; i < colorArrayToAdd.Length; i++)
            {
                colorsToLerp.Add(colorArrayToAdd[i]);
            }

            currentColorIndex = 0;
            canStartLerp = true;
            colorLerper = new ColorLerper(transitionTime, AbstractLerper<Color>.SMOOTH_TYPE.STEP_SMOOTHER);
            colorLerper.SetValues(OriginalColorUsed, colorsToLerp[currentColorIndex], true);
        }

        public void SetLoopStatus(bool status)
        {
            loop = status;
        }
        #endregion
    }
}