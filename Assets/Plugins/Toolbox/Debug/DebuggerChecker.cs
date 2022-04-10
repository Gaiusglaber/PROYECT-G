using UnityEngine;
using UnityEngine.SceneManagement;

using Pathfinders.Toolbox.Utils.SceneUtils;

namespace Pathfinders.Toolbox.Debugger
{
    public class DebuggerChecker : MonoBehaviour
    {
        #region PRIVATE_FIELDS
        private const string sceneName = "Debugger";
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            if (SceneUtils.DoesSceneExist(sceneName) && !SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            Destroy(gameObject);
        }
        #endregion
    }
}
