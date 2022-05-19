using System;
using UnityEngine;

namespace ProyectG.Gameplay.Objects{

	#region CLASSES
	public class Machine : MonoBehaviour
	{

		#region EXPOSED_FIELDS
		#endregion

		#region PRIVATE_FIELDS
		public Action OnInteract = null;
        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.E))
            {
                OnInteract?.Invoke();
            }*/
        }
        #endregion

        #region PUBLIC_METHODS
        #endregion

        #region PRIVATE_METHODS
        #endregion

        #region PUBLIC_CORROUTINES
        #endregion

        #region PRIVATE_CORROUTINES
        #endregion

    }
	#endregion

}