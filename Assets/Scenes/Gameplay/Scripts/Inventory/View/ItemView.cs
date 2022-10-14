using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ProyectG.Gameplay.Interfaces;
using ProyectG.Gameplay.Objects.Inventory.Data;

using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ItemView : MonoBehaviour, IDraggable
    {
        #region EXPOSED_FIELDS
        [SerializeField] private float followSpeed = 0;
        [SerializeField] private Image itemImage = null;
        [SerializeField] private bool useLerpAnimations = true;
        [SerializeField] private LayerMask [] whereCanBePlaced = null;
        #endregion

        #region PRIVATE_FIELDS
        private Canvas canvas = null;
        private Camera mainCamera = null;
        private RectTransform draggingPlane;
        private BoxCollider2D myCollider = null;
        private Vector3Lerper positionLerper = null;
        private PointerEventData eventData = null;

        private float currentLerpTime = 0;
        private float lerpTime = 0;
        private float finalPerc = 0;
        private float timeToGoBackSlot = 0.5f;
        private float time = 0;

        private bool isDragging = false;
        private bool prepareToAttachOnSlot = false;
        private bool isAttachedToSlot = false;
        private bool dragDisable = false;
        private bool itemInitialized = false;

        private Action<Vector2Int, Vector2Int> onEndedChangeOfSlot = null;

        private Action<Vector2Int, int, bool> onMoveItemToExternalSlot = null;
        private Action<string, int, Vector2Int> onAddedSomeItemFromExtraSlot = null;

        private (Vector2, Vector2Int ,Transform) slotPositionAttached = default;
        private Vector2Int slotGridPosition = default;

        private MachineSlotView machineSlot = null;
        private SlotInventoryView mySlot = null;

        private string itemId;

        private bool wasAttachedOnMachine = false;

        public ItemType itemViewType;

        private RectTransform thisRect = null;
        #endregion

        #region PROPERTIES
        public MachineSlotView MachineSlot { get { return machineSlot; } set { machineSlot = value; } }
        public bool WasAttachedOnMachine { get { return wasAttachedOnMachine; } set { wasAttachedOnMachine = value; } }
        public string ItemType { get { return itemId; } }
        public bool Dragged => isDragging;

        public (Vector2, Vector2Int, Transform) SlotPositionAttached { get { return slotPositionAttached; } }
        #endregion

        #region INITIALIZATION
        public void GenerateItem(Canvas mainCanvas, SlotInventoryView slotAttached, MachineSlotView machineSlot,ItemModel itemData = null, Action<Vector2Int,Vector2Int> onEndDrag = null,
            Action<Vector2Int, int, bool> onMoveItemToExternalSlot = null, Action<string, int, Vector2Int> onAddedItemFromOutside = null)
        {
            onEndedChangeOfSlot = onEndDrag;
            this.onMoveItemToExternalSlot = onMoveItemToExternalSlot;
            onAddedSomeItemFromExtraSlot = onAddedItemFromOutside;

            time = 0;
            timeToGoBackSlot = 0.5f;
            lerpTime = followSpeed;


            canvas = mainCanvas;
            myCollider = GetComponent<BoxCollider2D>();
            mainCamera = Camera.main;

            if(itemData != null)
            {
                itemId = itemData.itemId;
                itemImage.sprite = itemData.itemSprite;
                itemViewType = itemData.itemType;
            }
            else
            {
                itemId = "stacker";
            }

            //slotGridPosition = slotAttached.SlotPosition;
            thisRect = gameObject.transform as RectTransform;

            positionLerper = new Vector3Lerper(followSpeed * 0.5f, Vector3Lerper.SMOOTH_TYPE.STEP_SMOOTHER);

            if(slotAttached != null)
            {
                slotPositionAttached.Item1 = slotAttached.SlotPosition;
                slotPositionAttached.Item2 = slotAttached.GridPosition;
                slotPositionAttached.Item3 = slotAttached.transform;

                //Aplica el lerp del nuevo item creado al slot al que fue enviado
                itemInitialized = false;
                isAttachedToSlot = true;

                mySlot = slotAttached;
                AttachToSlot(slotAttached.SlotPosition, slotAttached.GridPosition,slotAttached.gameObject.transform, false);

                wasAttachedOnMachine = false;
            }
            else
            {
                this.machineSlot = machineSlot;

                slotPositionAttached.Item1 = machineSlot.SlotPosition;
                slotPositionAttached.Item2 = new Vector2Int(-1, -1);
                slotPositionAttached.Item3 = machineSlot.transform;

                //Aplica el lerp del nuevo item creado al slot al que fue enviado
                itemInitialized = false;
                isAttachedToSlot = true;

                mySlot = null;
                PlaceInMachineSlot(machineSlot.SlotPosition, machineSlot.transform, machineSlot);

                wasAttachedOnMachine = true;
            }
        }

        public void ClearSlot()
        {
            mySlot = null;
        }
        #endregion

        #region INTERACTION
        /// <summary>
        /// El update termina de desplazar el objeto si este no alcanzo el mouse hasta el final
        /// </summary>
        private void Update()
        {
            if (!Dragged || dragDisable)
                return;

            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float perc = currentLerpTime / lerpTime;
            float smooth = perc;
            smooth = perc * perc;
            perc = smooth;

            finalPerc = perc;

            SetDraggedPosition(eventData);
        }
        #endregion

        #region DRAG_IMPLEMENTATION
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (dragDisable)
                return;

            isDragging = true;

            prepareToAttachOnSlot = false;

            gameObject.transform.SetParent(canvas.transform, false);
            gameObject.transform.SetAsLastSibling();

            draggingPlane = canvas.transform as RectTransform;

            RectTransform objectDragging = gameObject.transform as RectTransform;

            objectDragging.position = eventData.position;

            this.eventData = eventData;

            SetDraggedPosition(eventData);

            OnItemTake();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragDisable)
                return;

            if (!Dragged || gameObject == null)
                return;

            SetDraggedPosition(eventData);
        }

        private void SetDraggedPosition(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
            {
                draggingPlane = eventData.pointerEnter.transform as RectTransform;
            }

            this.eventData = eventData;

            Vector3 globalMouse = default;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMouse))
            {
                thisRect.position = globalMouse;
                thisRect.rotation = draggingPlane.rotation;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.eventData = eventData;
            currentLerpTime = 0;
            isDragging = false;

            prepareToAttachOnSlot = true;

            CheckOverSlot();
        }

        private bool CheckItemType(ItemType[] allowedTypes)
        {
            bool check = false;

            if (allowedTypes.Length < 1)
            {
                return true;
            }
            for(int i = 0; i < allowedTypes.Length; i++)
            {
                if (itemViewType == allowedTypes[i])
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        private void OnItemTake()
        {
            Ray2D rayFromThisItem = new Ray2D(thisRect.position - (Vector3.forward * 15f), thisRect.forward * 50f);

            for (int i = 0; i < whereCanBePlaced.Length; i++)
            {
                RaycastHit2D[] allHits = Physics2D.RaycastAll(rayFromThisItem.origin, rayFromThisItem.direction, 50f, whereCanBePlaced[i]);

                foreach (RaycastHit2D hit in allHits)
                {
                    if (hit.collider.TryGetComponent(out SlotInventoryView slotFromItem))
                    {
                        slotFromItem.RemoveItemFromSlot(this);
                    }
                    else if(hit.collider.TryGetComponent(out MachineSlotView machineSlot))
                    {
                        machineSlot.RemoveItemFromSlot(this);
                    }
                }
            }

            Debug.DrawRay(rayFromThisItem.origin, rayFromThisItem.direction * 50f, Color.green);
        }

        private bool CheckOverSlot()
        {
            Ray2D rayFromThisItem = new Ray2D(thisRect.position - (Vector3.forward * 15f), thisRect.forward * 50f);

            for (int i = 0; i < whereCanBePlaced.Length; i++)
            {
                RaycastHit2D[] allHits = Physics2D.RaycastAll(rayFromThisItem.origin, rayFromThisItem.direction, 50f, whereCanBePlaced[i]);

                foreach (RaycastHit2D hit in allHits)
                {
                    if(hit.collider.TryGetComponent(out SlotInventoryView slotFromItem))
                    {
                        if (wasAttachedOnMachine)
                        {
                            Debug.Log("WAS ON MACHINE");
                        }

                        if (!wasAttachedOnMachine)
                        {
                            if(!slotFromItem.PlaceItemOnSlotInternal(this, true))
                            {
                                if (mySlot != null)
                                {
                                    mySlot.PlaceItemOnSlotInternal(this, false);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if(!slotFromItem.PlaceItemOnSlotInternal(this, false))
                            {
                                if (machineSlot != null)
                                {
                                    machineSlot.AddItemToSlot(this);
                                    return false;
                                }
                            }

                            onAddedSomeItemFromExtraSlot?.Invoke(itemId, 1, slotFromItem.GridPosition);

                            wasAttachedOnMachine = false;
                        }

                        if (mySlot != slotFromItem)
                        {
                            mySlot = slotFromItem;
                        }

                        Debug.Log("Slot " + slotFromItem.GridPosition);
                        
                        return true;
                    }
                    else if(hit.collider.TryGetComponent(out MachineSlotView machineSlot))
                    {
                        machineSlot.AddItemToSlot(this);

                        slotGridPosition = new Vector2Int(-1, -1);
                        slotPositionAttached = (machineSlot.SlotPosition, slotGridPosition, machineSlot.transform);

                        wasAttachedOnMachine = true;

                        Debug.Log("Item moved to slot machine " + machineSlot);
                        return true;
                    }
                }
            }

            if (!wasAttachedOnMachine)
            {
                if (mySlot != null)
                {
                    mySlot.PlaceItemOnSlotInternal(this, false);
                }
            }
            else
            {
                if (machineSlot != null)
                {
                    machineSlot.AddItemToSlot(this);
                }
            }

            Debug.DrawRay(rayFromThisItem.origin, rayFromThisItem.direction * 50f, Color.green);

            return false;
        }

        public void PlaceInMachineSlot(Vector2 positionSlot, Transform parent, MachineSlotView machineSlot,params ItemType[] allowedTypes)
        {
            if (!CheckItemType(allowedTypes))
                return;

            this.machineSlot = machineSlot;

            if (!isDragging)
            {
                Vector2Int positionItemMove = new Vector2Int(-1,-1);

                transform.position = positionSlot;

                if (itemInitialized)
                {
                    onMoveItemToExternalSlot.Invoke(positionItemMove, 1, false);    //Remove item from player inventory
                }

                transform.SetParent(parent);
                myCollider.enabled = true;
                itemInitialized = true;
            }
        }

        public void PlaceOnSlotAgain(Vector2 positionSlot, Vector2Int gridPos, Transform parent, params ItemType[] allowedTypes)
        {
            if (!CheckItemType(allowedTypes))
                return;

            if (!isDragging)
            {
                transform.position = positionSlot;

                if (itemInitialized)
                {
                    onAddedSomeItemFromExtraSlot.Invoke(itemId, 1, gridPos);    //Remove item from player inventory
                }

                transform.SetParent(parent);
                myCollider.enabled = true;
                itemInitialized = true;
            }
        }

        public bool AttachToSlot(Vector2 positionSlot, Vector2Int gridPos, Transform parent, bool swipeItem,params ItemType[] allowedTypes)
        {
            if (!CheckItemType(allowedTypes))
                return false;

            if (!isDragging)
            {
                (Vector2, Vector2Int, Transform) lastSlotPosition = slotPositionAttached;

                if (prepareToAttachOnSlot)
                {
                    isAttachedToSlot = true;
                    slotPositionAttached.Item1 = positionSlot;
                    slotPositionAttached.Item2 = gridPos;
                    slotPositionAttached.Item3 = parent;
                }

                if(useLerpAnimations)
                {
                    StartCoroutine(AttachToPosition(positionSlot, () =>
                    {
                        if(itemInitialized)
                        {
                            if(swipeItem)
                            {
                                onEndedChangeOfSlot.Invoke(lastSlotPosition.Item2, slotPositionAttached.Item2);
                            }
                        }

                        transform.SetParent(parent);
                        myCollider.enabled = true;
                        itemInitialized = true;
                    }));
                }
                else
                {
                    transform.position = positionSlot;

                    if (itemInitialized)
                    {
                        if(swipeItem)
                        {
                            if(transform.parent != parent)
                            {
                                onEndedChangeOfSlot.Invoke(lastSlotPosition.Item2, slotPositionAttached.Item2);
                            }
                        }
                    }

                    transform.SetParent(parent);
                    myCollider.enabled = true;
                    itemInitialized = true;
                }

                return true;
            }

            return false;
        }

        public void UpdateItemSlot((Vector2, Vector2Int, Transform) newSlotData)
        {
            slotPositionAttached = newSlotData;
        }

        public void SwitchStateItem(bool state)
        {
            if(itemId == "stacker")
            {
                dragDisable = false;
                return;
            }

            dragDisable = state;
        }

        public void SwitchStateCollider(bool state)
        {
            myCollider.enabled = state;
            enabled = state;
        }
        #endregion

        #region CORUTINES
        private IEnumerator AttachToPosition(Vector2 targetPosition, Action callbackAtEndPosition = null)
        {
            if (prepareToAttachOnSlot)
            {
                isAttachedToSlot = true;
            }

            positionLerper.SetValues(transform.position, targetPosition, true);

            while (!positionLerper.Reached)
            {
                positionLerper.Update();

                transform.position = positionLerper.CurrentValue;

                yield return null;
            }

            transform.position = positionLerper.CurrentValue;

            callbackAtEndPosition?.Invoke();

            yield return null;
        }
        #endregion
    }
}