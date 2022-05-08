using System;
using UnityEngine;
using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Gameplay.Objects{
	public class WorldItem : MonoBehaviour
	{
		#region EXPOSED_FIELDS
		[SerializeField] protected WorldItemSO data = null;
		[SerializeField] protected float fallSpeed = 0;
		#endregion

		#region PRIVATE_FIELDS
		private Vector2Lerper posLerper = null;
        private Vector2Lerper sizeLerper = null;
        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLS
        protected void Start()
        {
			posLerper = new Vector2Lerper(fallSpeed, AbstractLerper<Vector2>.SMOOTH_TYPE.STEP_SMOOTHER);
        }
        protected void Update()
        {
            UpdateLerpers();
        }
        #endregion

        #region PUBLIC_METHODS
        public virtual void Spawn()
        {

        }
		#endregion

		#region PRIVATE_METHODS
        private void UpdateLerpers()
        {
            if (posLerper.On)
            {
                posLerper.Update();
                transform.position = posLerper.CurrentValue;
            }
            if (sizeLerper.On)
            {
                sizeLerper.Update();
                transform.localScale = sizeLerper.CurrentValue;
            }
        }
		#endregion

		#region PUBLIC_CORROUTINES
		#endregion

		#region PRIVATE_CORROUTINES
		#endregion

	}
}