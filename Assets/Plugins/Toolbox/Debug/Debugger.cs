using UnityEngine;

using Pathfinders.Toolbox.Utils.FpsCounter;
using Pathfinders.Toolbox.Utils.UI;

namespace Pathfinders.Toolbox.Debugger
{
    public class Debugger : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private bool fpsEnabled = false;
        [SerializeField] private bool errorHandlerEnabled = false;
        [SerializeField] private FpsCounter fpsCounter = null;
        [SerializeField] private ErrorHandler errorHandler = null;
        #endregion

        #region UNITY_CALLS
        private void Awake()
        {
            fpsCounter.gameObject.SetActive(fpsEnabled);
            errorHandler.gameObject.SetActive(errorHandlerEnabled);
        }
        #endregion
    }
}
