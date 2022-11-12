using ProyectG.Gameplay.Objects.Inventory.Data;
using UnityEngine;

namespace ProyectG.Common.UI.Dialogs
{
    /// <summary>
    /// Stores a line information.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogLine", menuName = "ScriptableObjects/Dialog/DialogLine", order = 3)]
    public class DialogLineSO : ScriptableObject
    {
        public string id = string.Empty;
        public string actorId = string.Empty;
        public string emotion = string.Empty;
        public string lookingActorId = string.Empty;
        public ItemModel item = null;
        public int quantity = 0;
        public bool give = false;
        public bool recieve = false;

        [TextArea(10,20)]
        public string line = string.Empty;
        public bool loopOnBadAnswerws = false;
        public DialogAnswer[] answers = null;
    }
}
