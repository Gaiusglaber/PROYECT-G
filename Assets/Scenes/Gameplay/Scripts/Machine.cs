using System;
using UnityEngine;

using ProyectG.Gameplay.UI;

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
        #endregion

        #region PUBLIC_METHODS
        public virtual void Init(BaseView viewAttach)
        {
        }
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