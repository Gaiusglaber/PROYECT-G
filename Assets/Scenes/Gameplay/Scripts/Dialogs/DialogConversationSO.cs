using Pathfinders.Common.UI.Dialogs;
using UnityEngine;

namespace ProyectG.Common.UI.Dialogs
{
    /// <summary>
    /// Stores a dialog to be display in the scene.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogConversation", menuName = "ScriptableObjects/Dialog/DialogConversation", order = 1)]
    public class DialogConversationSO : ScriptableObject
    {
        public string id = string.Empty;
        public DialogActorSO[] actors = null;
        public DialogLineSO[] lines = null;
    }
}
