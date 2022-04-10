using UnityEngine.SceneManagement;

namespace Pathfinders.Toolbox.Utils.SceneUtils
{
    public static class SceneUtils
    {
        public static bool DoesSceneExist(string name)
        {
            if (string.IsNullOrEmpty(name)) 
            {
                return false;
            }

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                int lastSlash = scenePath.LastIndexOf("/");
                string sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

                if (string.Compare(name, sceneName, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
