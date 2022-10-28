using UnityEngine;
using UnityEngine.UI;

using ProyectG.Minigames.Common.Toolbox;

namespace ProyectG.Common.UI.Dialogs
{
    public class CarotArrow : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private ColorFlasherBase flasher = null;
        #endregion

        #region PRIVATE_FIELDS
        private Image carotImage = null;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            carotImage = GetComponent<Image>();
        }
        private void Update()
        {
            if (flasher == null)
                return;

            carotImage.color = flasher.CurrentUsedColor;
        }
        #endregion
    }
}