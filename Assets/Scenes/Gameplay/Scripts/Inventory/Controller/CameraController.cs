using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;
using System;

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
        private Camera mainCamera = null;
        private bool isAttachedToPlayer = true;
        #endregion

        #region PROPERTIES
        public Camera MainCamera { get => mainCamera; }
        public bool IsAttachedToPlayer { get => isAttachedToPlayer; set => isAttachedToPlayer = value; }
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if (!IsAttachedToPlayer)
                return;

            if (target == null)
                return;

            targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);

            if(!target.CustomAnimator._armature.flipX)
            {
                targetPosition = new Vector3(targetPosition.x + offsetCamera, targetPosition.y, targetPosition.z);
            }

            if(target.CustomAnimator._armature.flipX)
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

            mainCamera = GetComponent<Camera>();
        }

        public void MoveCamera(Vector3 worldPosition, Action onMoveEnded = null)
        {
            StopAllCoroutines();

            StartCoroutine(LerpToPosition(worldPosition, onMoveEnded));
        }

        public void ToggleAttachToPlayer(bool state)
        {
            IsAttachedToPlayer = state;
        }
        #endregion

        #region CORUTINES
        private IEnumerator LerpToPosition(Vector3 worldPosition, Action onLerpEnded = null)
        {
            float minDistance = 0.15f;

            Vector3 targetPosition = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);

            while (Vector3.Distance(transform.position, targetPosition) > minDistance)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

                yield return null;
            }

            onLerpEnded?.Invoke();

            yield break;
        }
        #endregion
    }
}