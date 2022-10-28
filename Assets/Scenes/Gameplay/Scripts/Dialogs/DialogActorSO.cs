using UnityEngine;

namespace ProyectG.Common.UI.Dialogs
{
    /// <summary>
    /// Stores all actor information.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogActor", menuName = "ScriptableObjects/Dialog/DialogActor", order = 2)]
    public class DialogActorSO : ScriptableObject
    {
        public string id = string.Empty;
        public string _name = string.Empty;
        public GameObject GO = null;
    }
}
