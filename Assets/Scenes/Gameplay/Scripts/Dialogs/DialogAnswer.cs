using UnityEngine;

namespace ProyectG.Common.UI.Dialogs
{
    /// <summary>
    /// Stores an answer information.
    /// </summary>
    [System.Serializable]
    public class DialogAnswer 
    {
        public string id = string.Empty;
        [TextArea(5, 10)]
        public string feedback = string.Empty;
        public string answer = string.Empty;
        public bool isCorrect = false;
        public int scoreValue = 0;
    }
}