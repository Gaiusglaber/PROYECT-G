using UnityEngine;

namespace Pathfinders.Common.UI.Dialogs
{
    public class UI_PanelHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        public void DisableObject()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}