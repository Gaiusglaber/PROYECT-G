using System;
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

        public InventoryModel Model => inventoryModel;

        #region PUBLIC_METHODS
        public void Init()
        {
            volume.profile.TryGet(out dof);
            initialdof = dof.focusDistance.value;

            InitilizeMVC();

            Vector2 initialPos = new Vector2(viewParent.position.x, viewParent.position.y);

            //Main inits
            inventoryModel.Init(bagSlots, slotsSize); //data del inventario
            inventoryView.Init(inventoryModel, viewParent, inventoryModel.SiwtchItemsOnSlots); //visual del inventario
            
            //Set actions
            inventoryView.SetOnHandleInventory(BlendBackground);
            inventoryModel.SetOnSomeItemAdded(inventoryView.UpdateInventoryView);
            inventoryModel.SetOnGetItemModelFromDatabae(GetItemModelFromId);

            //Testing
            //GenerateItems(allItemsAviable[0].itemId, 5);
            for (int i = 0; i < allItemsAviable.Count; i++)
            {
                GenerateItems(allItemsAviable[i].itemId, 5);
            }
        }

        public void OnCreateSomeItems()
        {
            for (int i = 0; i < allItemsAviable.Count; i++)
            {
                GenerateItems(allItemsAviable[i].itemId, 5);
            }
        }

        public void OnDeleteItems()
        {
            Vector2Int pos = new Vector2Int(0, 0);
            RemoveItems(pos, 1);
        }

        public void UpdateInventory()
        {
            if (inventoryView == null)
                return;

            inventoryView.UpdateSlots(stackTake);
        }

        public void CheckState()
        {
            if (inventoryView == null) return;

            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryView.OpenInventory();
            }

            if (!inventoryView.IsOpen)
                return;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                stackTake = true;
            }
            else
            {
                stackTake = false;
            }
        }

        public void GenerateItems(string idItem, int amount, Vector2Int gridPosition = default)
        {
            ItemModel itemToAttach = allItemsAviable.Find(t => t.itemId == idItem);
            List<ItemModel> newStackOfItems = new List<ItemModel>();

            for (int i = 0; i < amount; i++)
            {
                newStackOfItems.Add(itemToAttach);
            }

            inventoryModel.AttachItemsToSlot(newStackOfItems, gridPosition);
        }

        public void RemoveItems(Vector2Int gridPosition, int amount= 0, bool allItems = true)
        {
            inventoryModel.DeattachItemsFromSlot(gridPosition, amount, allItems);
        }
        #endregion

        #region PRIVATE_METHODS
        private ItemModel GetItemModelFromId(string idItem)
        {
            ItemModel resultItem = null;

            for (int i = 0; i < allItemsAviable.Count; i++)
            {
                if(allItemsAviable[i].itemId == idItem)
                {
                    resultItem = allItemsAviable[i];
                    break;
                }
            }

            if (resultItem == null)
                Debug.LogWarning("That item id is not in the database of items aviables.");

            return resultItem;
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
            FloatLerper lerper = new FloatLerper(2, AbstractLerper<float>.SMOOTH_TYPE.STEP_SMOOTHER);
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