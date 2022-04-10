using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinders.Toolbox.Lerpers.Movement;

namespace Pathfinders.Toolbox.Camera
{
    public class OpenableCamera : MonoBehaviour, IVector2Openable
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Vector3Opener opener = null;
        #endregion

        #region PUBLIC_METHODS
        public void ToggleVisibility(bool status, bool instant = false)
        {
            opener.UpdateStatus(status, instant);
        }
        #endregion
    }
}
