using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using ProyectG.Gameplay.Objects.Inventory.Data;
using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Toolbox.Lerpers;

namespace ProyectG.Gameplay.Objects.Inventory.Controller
{
    public class InventoryController : MonoBehaviour   //Controlador del inventario
    {
        #region EXPOSED_FIELDS
        [Header("MAIN SETTINGS")]
        [SerializeField, Tooltip("This is a grid size, so the amount of slots will be rows per column [ Note->(x * y) ].")]
        private Vector2Int bagSlots = default;
        [SerializeField, Tooltip("The logic spacing between slots.")] private Vector2 slotsSize = default;
        [Header("REFERENCES")]
        [SerializeField] private Transform viewParent = default;
        [SerializeField] private InventoryView inventoryView = null;
        [SerializeField] private Volume volume = null;
        [Header("ITEM DATABASE")]
        [SerializeField] private List<ItemModel> allItemsAviable = null;
        #endregion

        #region PRIVATE_FIELDS
        private InventoryModel inventoryModel = null;
        private DepthOfField dof = null;
        private bool stackTake = false;

        private float initialdof = 0;
        #endregion

        #region PROPERTIES
        public bool StackTake { get { return stackTake; } }
        public InventoryModel Model => inventoryModel;
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            volume.profile.TryGet(out dof);
            initialdof = dof.focusDistance.value;

            InitilizeMVC();

            Vector2 initialPos = new Vector2(viewParent.position.x, viewParent.position.y);

            //Main inits
            inventoryModel.Init(bagSlots, slotsSize); //data del inventario
            inventoryView.Init(inventoryModel, viewParent, inventoryModel.SiwtchItemsOnSlots, inventoryModel.SiwtchStackOfItemsOnSlots); //visual del inventario

            //Set actions
            inventoryView.SetOnHandleInventory(BlendBackground);
            inventoryModel.SetOnSomeItemAdded(inventoryView.UpdateInventoryView);
            inventoryModel.SetOnGetItemModelFromDatabae(GetItemModelFromId);

            if (!inventoryView.IsOpen)
            {
                stackTake = true;

                inventoryView.OnChangeInteractionType(stackTake);
            }
        }

        public List<SlotInventoryModel> GetExtraSlotsFromInventory()
        {
            return inventoryModel.ExtraGridSlots;
        }

        public SlotInventoryModel GetSlotFromGridPosition(Vector2Int gridPosition)
        {
            return inventoryModel.ExtraGridSlots.Find(t => t.GridPosition == gridPosition);
        }

        public void ExtendInventoryWithExtraSlots(int fromX, int toX, int fromY, int toY, List<SlotInventoryView> extraSlotsView, ref List<Vector2Int> positionsIntegrated)
        {
            inventoryModel.SetExtraSlots(fromX, toX, fromY, toY, ref positionsIntegrated);

            inventoryView.SetExtraViewSlots(extraSlotsView);
        }

        public void ClearExtraSlotsInventory()
        {
            inventoryModel.ClearExtraSlots();
            inventoryView.ClearExtraSlots();
        }

        public void GenerateItem(string idItem)
        {
            GenerateItems(idItem, 1);
        }

        public void GenerateItem(string idItem, Vector2Int slotPosition)
        {
            GenerateItems(idItem, 1, slotPosition);
        }

        public void OnDeleteItem()
        {
            Vector2Int pos = new Vector2Int(0, 0);
            RemoveItems(pos, 1);
        }

        public void ToggleInventory()
        {
            inventoryView.ToggleInventory();

            if(!inventoryView.IsOpen)
            {
                DisableStackMode();
            }
        }

        public void CheckState()
        {
            if (inventoryView == null) return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                inventoryView.ToggleInventory();

                if(!inventoryView.IsOpen)
                {
                    DisableStackMode();
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                stackTake = false;

                inventoryView.OnChangeInteractionType(stackTake);

                return;
            }

            stackTake = true;
            inventoryView.OnChangeInteractionType(stackTake);
        }

        public void GenerateItems(string idItem, int amount, Vector2Int gridPosition = default)
        {
            ItemModel itemToAttach = allItemsAviable.Find(t => t.itemId == idItem);
            List<ItemModel> newStackOfItems = new List<ItemModel>();

            for (int i = 0; i < amount; i++)
            {
                if (itemToAttach!=null)
                newStackOfItems.Add(itemToAttach);
            }

            inventoryModel.AttachItemsToSlot(newStackOfItems, gridPosition);
        }

        public void RemoveItems(Vector2Int gridPosition, int amount= 0, bool allItems = true)
        {
            inventoryModel.DeattachItemsFromSlot(gridPosition, amount, allItems);
        }

        public ItemModel GetItemModelFromId(string idItem)
        {
            ItemModel resultItem = null;

            for (int i = 0; i < allItemsAviable.Count; i++)
            {
                if (allItemsAviable[i].itemId == idItem)
                {
                    resultItem = allItemsAviable[i];
                    break;
                }
            }

            if (resultItem == null)
                Debug.LogWarning("That item id is not in the database of items aviables.");

            return resultItem;
        }

        public ItemModel GetItemModelFromView(ItemView viewItem)
        {
            ItemModel resultItem = null;

            for (int i = 0; i < allItemsAviable.Count; i++)
            {
                if (allItemsAviable[i].itemId == viewItem.ItemType)
                {
                    resultItem = allItemsAviable[i];
                    break;
                }
            }

            if (resultItem == null)
                Debug.LogWarning("That item id is not in the database of items aviables.");

            return resultItem;
        }
        #endregion

        #region PRIVATE_METHODS
        private void DisableStackMode()
        {
            stackTake = false;
            inventoryView.OnChangeInteractionType(stackTake);
        }

        private void InitilizeMVC()
        {
            inventoryModel = new InventoryModel();
        }

        private void BlendBackground(bool state)
        {
            if(state)
            {
                StartCoroutine(LerpVolumeAttribute(dof, initialdof));
            }
            else
            {
                StartCoroutine(LerpVolumeAttribute(dof, 0));
            }
        }
        #endregion

        #region CORUTINES
        private IEnumerator LerpVolumeAttribute(DepthOfField component, float destIntensity)
        {
            FloatLerper lerper = new FloatLerper(.5f, AbstractLerper<float>.SMOOTH_TYPE.STEP_SMOOTHER);
            lerper.SetValues(component.focusDistance.value, destIntensity, true);
            while (lerper.On)
            {
                lerper.Update();
                component.focusDistance.value = lerper.CurrentValue;
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}