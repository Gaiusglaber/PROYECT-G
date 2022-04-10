using UnityEngine;

using Pathfinders.Toolbox.Lerpers;

using TMPro;


namespace Pathfinders.Toolbox.Utils.UI
{
    public class LerpedTMP : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TextMeshProUGUI txtTarget = null;
        [SerializeField] private float time = 0.0f;
        [SerializeField] private FloatLerper.SMOOTH_TYPE mode = default;
        [SerializeField] private bool roundToInt = false;
        #endregion

        #region PRIVATE_FIELDS
        private FloatLerper lerper = null;
        private float target = 0.0f;
        #endregion

        #region UNITY_CALLS
        private void Awake()
        {
            if (lerper == null)
            {
                lerper = new FloatLerper(time, mode);
            }
            
        }

        private void Update()
        {
            if (lerper.On)
            {
                lerper.Update();
                txtTarget.text = ((int)lerper.CurrentValue).ToString();
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void UpdateTarget(float amount)
        {
            if (lerper == null)
            {
                lerper = new FloatLerper(time, mode);
            }

            target = amount;
            lerper.SetValues(lerper.CurrentValue, target, true);
        }

        public void UpdateTargetInstant(float amount)
        {
            lerper.SwitchState(false);

            target = amount;
            txtTarget.text = target.ToString();
        }
        #endregion
    }
}
