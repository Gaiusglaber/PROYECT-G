using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ProyectG.Toolbox.Lerpers;
using ProyectG.Gameplay.Objects;

namespace ProyectG.Gameplay.UI
{
    public class GameplayUIController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Machine machine = null;
        [Space(10)]
        [Header("UI Data")]
        [SerializeField] private TMPro.TMP_Text jewel0value = null;
        [SerializeField] private TMPro.TMP_Text jewel1value = null;
        [SerializeField] private TMPro.TMP_Text jewel2value = null;
        [SerializeField] private TMPro.TMP_Text jewel3value = null;
        [SerializeField] private TMPro.TMP_Text jewel4value = null;
        [SerializeField] private TMPro.TMP_Text coinvalue = null;
        [SerializeField] private float panelLerperSpeed = 0;
        [SerializeField] private Vector2 posToLerpPanel = Vector2.zero;
        [SerializeField] private GameObject panel = null;
        [SerializeField] private Volume volume = null;
        #endregion
        #region PRIVATE_FIELDS
        private DepthOfField dof = null;
        private bool panelHidden = true;
        private Vector2Lerper panelLerper = null;
        private Vector2 initialPos = Vector2.zero;
        private float initialdof = 0;
        #endregion
        #region UNTIY_CALLS
        private void Start()
        {
            machine.OnInteract += ShowPanel;
            volume.profile.TryGet(out dof);
            initialPos = panel.GetComponent<RectTransform>().anchoredPosition;
            initialdof = dof.focusDistance.value;
            panelLerper = new Vector2Lerper(panelLerperSpeed, AbstractLerper<Vector2>.SMOOTH_TYPE.EASE_OUT);
        }
        #endregion
        #region PUBLIC_METHODS
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
        #endregion
        #region PRIVATE_METHODS
        #endregion
        #region PUBLIC_CORROUTINES
        #endregion
        #region PRIVATE_CORROUTINES
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
