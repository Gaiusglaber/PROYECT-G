using UnityEngine;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Toolbox.Fill
{
    public abstract class AbstractFill : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] protected FloatLerperMono floatLerper = null;
        [SerializeField] protected bool useColorLerper = false;
        [SerializeField] protected Gradient gradient = null;
        #endregion

        #region PROTECTED_FIELDS
        protected float finalValue = 0f;
        #endregion

        #region PUBLIC_METHODS
        public abstract void Init();
        public abstract void SetValues(float newFinalValue, float startValue = 0);
        public abstract void UpdateValue(float amount,bool instant = false);
        #endregion
    }
}