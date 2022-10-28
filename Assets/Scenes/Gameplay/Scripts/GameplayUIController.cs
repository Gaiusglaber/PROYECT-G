using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ProyectG.Toolbox.Lerpers;
using ProyectG.Gameplay.Objects;
using UnityEngine.SceneManagement;
using ProyectG.Common.UI.Dialogs;
using System;

namespace ProyectG.Gameplay.UI
{
    public class GameplayUIController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Machine machine = null;
        [Space(10)]
        [Header("UI Data")]
        [SerializeField] private float panelLerperSpeed = 0;
        [SerializeField] private Vector2 posToLerpPanel = Vector2.zero;
        [SerializeField] private GameObject panel = null;
        [SerializeField] private Volume volume = null;
        [SerializeField] private GameObject gameOverPanel;

        [Header("TEMPORAL")]
        [SerializeField] private GameObject panelControlls = null;
        [SerializeField] private DialogManager dialogManager = null;
        [SerializeField] private Animator dialogPanel = null;
        [SerializeField] private Button onOpenControlls = null;
        [SerializeField] private Button onCloseControlls = null;
        [SerializeField] private Vector2 panelOpenDestination = default;
        [SerializeField] private Vector2 panelCloseDestination = default;
        [SerializeField] private DialogConversationSO[] conversationSOs = null;
        [SerializeField] private NPCHandler npcHandler = null;
        #endregion
        #region PRIVATE_FIELDS
        private DepthOfField dof = null;
        private bool panelHidden = true;
        private Vector2Lerper panelLerper = null;
        private Vector2 initialPos = Vector2.zero;
        private float initialdof = 0;
        private Action OnDialogStart = null;
        private Action OnDialogEnd = null;
        #endregion
        #region UNTIY_CALLS
        private void Start()
        {
            machine.OnInteract += ShowPanel;
            volume.profile.TryGet(out dof);
            initialPos = panel.GetComponent<RectTransform>().anchoredPosition;
            initialdof = dof.focusDistance.value;
            panelLerper = new Vector2Lerper(panelLerperSpeed, AbstractLerper<Vector2>.SMOOTH_TYPE.EASE_OUT);

            onOpenControlls.onClick.AddListener(ShowControlls);
            onCloseControlls.onClick.AddListener(HideControlls);
            onCloseControlls.gameObject.SetActive(false);
            EnergyHandler.Withoutenergy += ShowGameOverPanel;
            npcHandler.Init(dialogManager.LoadDialogue);
            dialogManager.Init();
            dialogManager.InitAllDialogPlayers(OpenPanel, OnDialogStart, OnDialogEnd); 
            dialogManager.SetConversations(conversationSOs);
            dialogManager.OnDialogEnd += ClosePanel;
        }

        private void OnDisable()
        {
            EnergyHandler.Withoutenergy -= ShowGameOverPanel;
        }

        #endregion
        #region PUBLIC_METHODS
        public void ClosePanel()
        {
            dialogPanel.SetBool("IsOpen", false);
        }

        public void OpenPanel()
        {
            dialogPanel.SetBool("IsOpen", true);
        }
        public void ShowPanel()
        {
            panelHidden = !panelHidden;
            if (!panelHidden)
            {
                StartCoroutine(LerpPanel(posToLerpPanel));
                StartCoroutine(LerpVolumeAttribute(dof, 0));
            }
            else
            {
                StartCoroutine(LerpPanel(initialPos));
                StartCoroutine(LerpVolumeAttribute(dof, initialdof));
            }
        }

        public void OnClickRetry(string scene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion
        #region PRIVATE_METHODS
        private void ShowControlls()
        {
            StartCoroutine(LerpPanel(panelControlls, panelOpenDestination));
        }
        private void HideControlls()
        {
            StartCoroutine(LerpPanel(panelControlls, panelCloseDestination));
        }

        private void ShowGameOverPanel(bool state)
        {
            gameOverPanel.SetActive(state);
        }
        #endregion
        #region PUBLIC_CORROUTINES
        #endregion
        #region PRIVATE_CORROUTINES
        private IEnumerator LerpPanel(GameObject panel, Vector2 destPos)
        {
            RectTransform panelPosition = panel.GetComponent<RectTransform>();
            panelLerper.SetValues(panelPosition.anchoredPosition, destPos, true);
            while (panelLerper.On)
            {
                panelLerper.Update();
                panelPosition.anchoredPosition = panelLerper.CurrentValue;
                yield return new WaitForEndOfFrame();
            }
        }
        private IEnumerator LerpPanel(Vector2 destPos)
        {
            RectTransform panelPosition = panel.GetComponent<RectTransform>();
            panelLerper.SetValues(panelPosition.anchoredPosition, destPos, true);
            while (panelLerper.On)
            {
                panelLerper.Update();
                panelPosition.anchoredPosition = panelLerper.CurrentValue;
                yield return new WaitForEndOfFrame();
            }
        }
        private IEnumerator LerpVolumeAttribute(DepthOfField component,float destIntensity)
        {
            FloatLerper lerper = new FloatLerper(2, AbstractLerper<float>.SMOOTH_TYPE.STEP_SMOOTHER);
            lerper.SetValues(component.focusDistance.value, destIntensity, true);
            while (lerper.On)
            {
                lerper.Update();
                component.focusDistance.value = lerper.CurrentValue;
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}
