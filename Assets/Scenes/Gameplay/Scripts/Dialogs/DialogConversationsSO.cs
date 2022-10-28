using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Common.UI.Dialogs
{
    /// <summary>
    /// Stores all dialogs data
    /// </summary>
    [CreateAssetMenu(fileName = "DialogConversations", menuName = "ScriptableObjects/Dialog/DialogConversations", order = 1)]
    public class DialogConversationsSO : ScriptableObject
    {
        public string id = string.Empty;
        public List<DialogConversationSO> conversations = null;
    }
}
