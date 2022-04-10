using UnityEngine;

using TMPro;

namespace Pathfinders.Toolbox.Utils.FpsCounter
{
    public class FpsCounter : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TextMeshProUGUI txtMinFps = null;
        [SerializeField] private TextMeshProUGUI txtMaxFps = null;
        [SerializeField] private TextMeshProUGUI txtAvgFps = null;
        [SerializeField] private TextMeshProUGUI txtCurrFps = null;
        [SerializeField] private float refreshRate = 1.0f;
        #endregion

        #region PRIVATE_FIELDS
        private float timer = 0.0f;

        private int totalFrames = 0;
        private int totalFps = 0;
        private int minFps = 0;
        private int maxFps = 0;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            timer = 0.0f;
        }

        private void Update()
        {
            if (timer >= refreshRate)
            {
                int fps = (int)(1.0f / Time.unscaledDeltaTime);
                totalFrames++;
                totalFps += fps;

                if (fps > maxFps)
                {
                    txtMaxFps.text = "MAX " + fps.ToString();
                    maxFps = fps;
                }

                if (fps < minFps)
                {
                    txtMinFps.text = "MIN " + fps.ToString();
                    minFps = fps;
                }

                txtCurrFps.text = "CUR " + fps.ToString();
                txtAvgFps.text = "AVG " + (totalFps / totalFrames).ToString();
                timer = 0.0f;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        #endregion
    }
}
