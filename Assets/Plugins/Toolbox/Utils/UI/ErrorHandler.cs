using UnityEngine;

using Pathfinders.Toolbox.Singletons;

using TMPro;

namespace Pathfinders.Toolbox.Utils.UI
{
    public class ErrorHandler : GenericSingleton<ErrorHandler>
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject holder = null;
        [SerializeField] private TextMeshProUGUI txtError = null;
        [SerializeField] private float screenTime = 2f;
        #endregion

        #region PRIVATE_FIELDS
        private float timer = 0f;
        private bool showing = false;
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if (showing)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    showing = false;
                    timer = 0f;
                    holder.SetActive(false);
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void ShowError(string error)
        {
            txtError.text = error;
            holder.SetActive(true);

            timer = screenTime;
            showing = true;
        }
        #endregion
    }
}
