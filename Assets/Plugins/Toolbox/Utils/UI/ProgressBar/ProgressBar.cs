using UnityEngine;

using TMPro;

namespace Pathfinders.Toolbox.Utils.UI.ProgressBar
{
    public class ProgressBar : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] protected ImageFill progressBar = null;
        [SerializeField] protected TMP_Text currentProgressText = null;
        #endregion

        #region PROTECTED_FIELDS
        protected int maxProgress = 0;
        protected int currentProgress = 0;
        #endregion

        #region PROTECTED_METHODS
        protected void UpdateProgressText(string newText)
        {
            if (currentProgressText != null)
            {
                currentProgressText.text = newText;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public virtual void SetupProgressBar(int newMaxProgress)
        {
            maxProgress = newMaxProgress;
            currentProgress = 0;

            UpdateProgressText("0%");

            progressBar.SetValues(maxProgress);
        }

        public virtual void UpdateProgressBar(int newProgress)
        {
            currentProgress = newProgress;
            progressBar.UpdateValue(currentProgress);

            if (currentProgress != 0)
            {
                float progressInPercentage = (float)currentProgress / (float)maxProgress;
                progressInPercentage *= 100;

                UpdateProgressText((int)progressInPercentage + "%");
            }
            else
            {
                UpdateProgressText("0%");
            }
        }

        public (int,int) GetCurrentAndMaxProgress()
        {
            return (currentProgress, maxProgress);
        }
        #endregion
    }
}