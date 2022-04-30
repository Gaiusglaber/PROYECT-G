using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;
using ProyectG.Gameplay.Objects.Inventory.Data;

using TMPro;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class SlotInventoryView : MonoBehaviour, IPointerDownHandler
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Image slotFrame = null;
        [SerializeField] private Image iconItemAttach = null;
        [SerializeField] private BoxCollider2D colliderSprite = null;
        [SerializeField] private TextMeshProUGUI slotStack = null;
        #endregion

        #region PRIVATE_FIELDS
        private UnityAction atInteract = null;

        private Transform nextSlotFromThis = default;

        private List<ItemBase> objectsAttach = new List<ItemBase>();
        #endregion

        #region UNITY_CALLS
        
        #endregion

        #region PROPERTIES
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

        public Transform NextSlotFromThis
        {
            get
            {
                return nextSlotFromThis;
            }
            set
            {
                nextSlotFromThis = value;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Transform nextStlotFromThis)
        {
            nextSlotFromThis = nextStlotFromThis;
            slotStack.text = string.Empty;
        }

        public void UpdateSlot()
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, colliderSprite.size, 0, transform.forward);

            if(!IsStackUpdated(hits))
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.TryGetComponent(out ItemBase newItem))
                    {
                        AddItemToSlotStack(newItem);
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnInteract?.Invoke();
        }
        #endregion

        #region GIZMOS
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, colliderSprite.size);
        }
        #endregion

        #region PRIVATE_METHODS
        private bool IsStackUpdated(RaycastHit2D [] actualItemsInsideSlot)
        {
            //Debug.Log(actualItemsInsideSlot.Length-1);

            if(objectsAttach.Count == (actualItemsInsideSlot.Length - 1))   //Sin contar el mismo slot que tiene su collider
            {
                return true;
            }
            else
            {
                int amountToRemove = objectsAttach.Count - (actualItemsInsideSlot.Length-1);

                if (amountToRemove < 0)
                    return false;

                for (int i = 0; i < amountToRemove; i++)
                {
                    RemoveItemFromSlotStack(objectsAttach[objectsAttach.Count - 1]);
                }
                return false;
            }
        }
        private void AddItemToSlotStack(ItemBase item)
        {
            if(!objectsAttach.Contains(item))
            {
                objectsAttach.Add(item);
                Debug.Log("Item added to list");
                slotStack.text = objectsAttach.Count.ToString();
            }
        }
        private void RemoveItemFromSlotStack(ItemBase item)
        {
            if (objectsAttach.Contains(item))
            {
                objectsAttach.Remove(item);
                Debug.Log("Item removed from list");
                slotStack.text = objectsAttach.Count.ToString();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (objectsAttach.Count > 0)
            {
                if(collision.TryGetComponent(out ItemBase newItem))
                {
                    if(newItem.ItemType == objectsAttach[0].ItemType)
                    {
                        newItem.AttachToSlot(SlotPosition, transform);
                    }
                    else
                    {
                        if(newItem.AttachToSlot(NextSlotFromThis.position, NextSlotFromThis))
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                if (collision.TryGetComponent(out ItemBase item))
                {
                    item.AttachToSlot(SlotPosition, transform);
                }                
            }
        }
        #endregion
    }
}