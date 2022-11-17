using UnityEngine;

namespace ProyectG.Gameplay.UI
{
    public class BaseView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("BASE VIEW")]
        [SerializeField] private GameObject holder;
        [SerializeField] private string idView = null;
        #endregion

        #region PRIVATE_FIELDS
        protected bool initialized = false;
        #endregion

        #region PROPERTIES
        public string IdView { get { return idView; } }
        public bool Initialized { get { return initialized; } }
        #endregion

        #region UNITY_CALLS
        private void OnDestroy()
        {
            initialized = false;
        }
        #endregion

        #region PUBLIC_METHODS
        public virtual void Init()
        {
            initialized = true;
        }

        public virtual void TogglePanel(){ }

        public virtual void ToggleView(bool state)
        {
            holder.SetActive(state);
        }
        #endregion
    }
}