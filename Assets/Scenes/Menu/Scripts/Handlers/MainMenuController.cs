using UnityEngine;

using ProyectG.MainMenu.UI;

namespace ProyectG.MainMenu.Handlers
{
    public class MainMenuController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private UIMainMenu UIMainMenu;
        [SerializeField] private string gameplayScene = null;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            UIMainMenu.Init(PlayGame, CloseGame);
        }
        #endregion

        #region PRIVATE_METHODS
        private void PlayGame()
        {
            SceneLoader.Instance.LoadSceneAsyncWithLoadScreen(gameplayScene);
        }

        private void CloseGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        #endregion
    }
}