using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;

namespace ProyectG.Gameplay.Controllers
{
    public class CameraController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private PlayerController target = null;
        [SerializeField] private float followSpeed = 0f;
        [SerializeField] private float offsetCamera = 0f;
        [SerializeField] private bool initialized = false;
        #endregion

        #region PRIVATE_FIELDS
        private Vector3 targetPosition = default;
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if (target == null)
                return;

            targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);

            if(target.Velocity.x != 0 && target.Velocity.x > 1f)
            {
                targetPosition = new Vector3(targetPosition.x + offsetCamera, targetPosition.y, targetPosition.z);
            }

            if (target.Velocity.x != 0 && target.Velocity.x < 1f)
            {
                targetPosition = new Vector3(targetPosition.x - offsetCamera, targetPosition.y, targetPosition.z);
            }


            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(PlayerController target)
        {
            this.target = target;
        }
        #endregion
    }
}