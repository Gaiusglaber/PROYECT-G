using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace ProyectG.MainMenu.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Button btnPlay = null;
        [SerializeField] private Button btnExit = null;
        [Header("MENU SETTINGS")]
        [SerializeField] private float delayToSHowTitle = 0.5f;

        #endregion

        #region UNITY_CALLS
        #endregion

        #region PUBLIC_METHODS
        public void Init(Action onPlayPressed, Action onExitPressed)
        {
            btnPlay.onClick.AddListener(() => { onPlayPressed?.Invoke(); });
            btnExit.onClick.AddListener(() => { onExitPressed?.Invoke(); });
        }

        public void DeInit()
        {
            btnPlay.onClick.RemoveAllListeners();
            btnExit.onClick.RemoveAllListeners();
        }
        #endregion

        #region PRIVATE_METHODS
        #endregion
    }
}