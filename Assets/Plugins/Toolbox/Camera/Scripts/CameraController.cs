using UnityEngine;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Toolbox.Camera
{
    public class CameraController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Vector3LerperMono positionLerper = null;
        [SerializeField] private FloatLerperMono zoomLerper = null;
        #endregion

        #region PRIVATE_FIELDS
        private UnityEngine.Camera cam = null;
        #endregion

        #region UNITY_CALLS
        protected virtual void Awake()
        {
            cam = GetComponent<UnityEngine.Camera>();
        }

        protected virtual void Update()
        {
            UpdatePosition();
            UpdateZoom();
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetPosition(Vector3 pos, bool instant = false)
        {
            if (instant)
            {
                positionLerper.SwitchState(false);
                transform.position = pos;
            }
            else
            {
                positionLerper.SetValues(transform.position, pos, true);
            }
        }

        public void SetZoom(float amount, bool instant = false)
        {
            if (instant)
            {
                zoomLerper.SwitchState(false);
                cam.orthographicSize = amount;
            }
            else
            {
                zoomLerper.SetValues(cam.orthographicSize, amount, true);
            }
        }
        #endregion

        #region PROTECTED_METHODS
        protected virtual void UpdatePosition()
        {
            if (positionLerper.On)
            {
                transform.position = positionLerper.CurrentValue;
            }
        }

        protected virtual void UpdateZoom()
        {
            if (zoomLerper.On)
            {
                cam.orthographicSize = zoomLerper.CurrentValue;
            }
        }
        #endregion
    }
}
