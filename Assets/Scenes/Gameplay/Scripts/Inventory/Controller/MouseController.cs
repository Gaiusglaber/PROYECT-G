using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Gameplay.Controllers
{
    public class MouseController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private LayerMask[] checkClickHover = null;
        [SerializeField] private float distance = 180f;
        [SerializeField] private float followSpeed = 100f;
        [SerializeField] private bool hideRealCursor = false;
        #endregion

        #region PRIVATE_FIELDS
        private Camera mainCamera = null;
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            Ray mouseCheck = mainCamera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(mouseCheck.origin, mouseCheck.direction * distance, Color.red);
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            mainCamera = Camera.main;

            Cursor.visible = hideRealCursor;
        }
        #endregion
    }
}