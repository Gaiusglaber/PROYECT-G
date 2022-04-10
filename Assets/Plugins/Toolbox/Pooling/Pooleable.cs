using UnityEngine;

namespace Pathfinders.Toolbox.Pooling
{
    public abstract class Pooleable : MonoBehaviour
    {
        #region ABSTRACT
        public abstract void Get();
        public abstract void Release();
        #endregion
    }
}
