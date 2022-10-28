using UnityEngine;

using System;

namespace ProyectG.Common.UI.Dialogs
{
    public class DialogPlayer : MonoBehaviour
    {
        #region PRIVATE_FIELDS
        private Action<string> triggerDialog;
        #endregion

        #region EXPOSED_METHODS
        public void PlayDialog(string COnversationID)
        {
            triggerDialog?.Invoke(COnversationID);
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetAction(Action<string> dialogEvent)
        {
            triggerDialog += dialogEvent;
        }

        public void ClearAction(Action<string> dialogEvent)
        {
            triggerDialog -= dialogEvent;
        }
        #endregion
    }
}