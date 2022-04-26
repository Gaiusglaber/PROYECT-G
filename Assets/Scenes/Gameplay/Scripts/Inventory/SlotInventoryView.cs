using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class SlotInventoryView : MonoBehaviour, IPointerDownHandler
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Image slotFrame = null;
        [SerializeField] private Image iconItemAttach = null;
        [SerializeField] private BoxCollider2D colliderSprite = null;
        #endregion

        #region PRIVATE_FIELDS
        private UnityAction atInteract = null;
        #endregion

        #region UNITY_CALLS
        
        #endregion

        #region PROPERTIE
        public UnityAction OnInteract
        {
            get
            {
                return atInteract;
            }
            set
            {
                atInteract = value;
            }
        }
        public Vector2 SlotPosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDraggable dragObject))
            {
                Debug.Log("Hay un draggable encima o");

                dragObject.AttachToSlot(SlotPosition, transform);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnInteract?.Invoke();
        }
        #endregion
    }
}