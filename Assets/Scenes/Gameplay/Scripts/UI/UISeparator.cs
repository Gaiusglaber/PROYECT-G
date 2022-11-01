using System;

using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.UI
{
    public class UISeparator : BaseView
    {
        #region EXPOSED_FIELDS
        [Header("SEPARATOR VIEW")]
        [SerializeField] private MachineSlotView inputSlot;
        [SerializeField] private MachineSlotView output1;
        [SerializeField] private MachineSlotView output2;

        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private ItemView prefabItemView = null;
        [SerializeField] private GameObject panelSeparator = null;
        [SerializeField] private Canvas mainCanvas = null;
        [SerializeField] private Image progressFillProcess = null;
        public Action<ItemModel> OnProcessMaterial;
        public Action onCancelProcess = null;
        #endregion

        #region PRIVATE_FIELDS
        private Func<bool> isSeparatorProcessing = null;
        private float durationProcess = 0.0f;
        #endregion

        #region PROPERTIES
        public InventoryController InverntoryController { get { return inventoryController; } }
        public Func<bool> IsSeparatorProcessing { set { isSeparatorProcessing = value; } get { return isSeparatorProcessing; } }
        #endregion

        #region PUBLIC_METHODS
        public override void Init()
        {
            inputSlot.Init(mainCanvas, inventoryController.OnHoverSelection);
            output1.Init(mainCanvas, inventoryController.OnHoverSelection);
            output2.Init(mainCanvas, inventoryController.OnHoverSelection);

            inventoryController.OnInteractionChange += inputSlot.SetOnInteractionInventoryChange;
            inventoryController.OnInteractionChange += output1.SetOnInteractionInventoryChange;
            inventoryController.OnInteractionChange += output2.SetOnInteractionInventoryChange;

            onCancelProcess += StopFill;

            base.Init();
        }

        public void SetDurationProcess(float timeToSeparate)
        {
            durationProcess = timeToSeparate;
        }

        public void GenerateProcessedItems(ItemModel itemFrom1)
        {
            ItemModel finalItem = itemFrom1.itemResults[1];
            ItemModel finalIte2 = itemFrom1.itemResults[2];

            if (output1 == null)
            {
                Debug.LogWarning("Failed to generat result for the procesed item, the outpu1 slot is NULL");
                return;
            }

            if (output2 == null)
            {
                Debug.LogWarning("Failed to generate result for the procesed item, the output2 slot is NULL");
                return;
            }

            ItemView newHalftItem1 = Instantiate(prefabItemView, output1.SlotPosition, Quaternion.identity, output1.transform);
            newHalftItem1.GenerateItem(mainCanvas, null, output1, finalItem, inventoryController.Model.SiwtchItemsOnSlots, inventoryController.OnRemoveItems, inventoryController.OnAddItems);

            ItemView newHalftItem2 = Instantiate(prefabItemView, output2.SlotPosition, Quaternion.identity, output2.transform);
            newHalftItem2.GenerateItem(mainCanvas, null, output2, finalIte2, inventoryController.Model.SiwtchItemsOnSlots, inventoryController.OnRemoveItems, inventoryController.OnAddItems);

            if (inventoryController.StackTake)
            {
                output1.AddItemToStack(newHalftItem1);
                output2.AddItemToStack(newHalftItem2);
            }
            else
            {
                output1.AddItemToSlot(newHalftItem1);
                output2.AddItemToSlot(newHalftItem2);
            }
        }

        public void TogglePanel()
        {
            panelSeparator.SetActive(!panelSeparator.activeSelf);
            inventoryController.ToggleInventory();
        }

        public void OnEndProcess()
        {
            ItemView itemToRemove = null;

            if (inventoryController.StackTake)
            {
                itemToRemove = inputSlot.StackOfItems.Stack[0];
                inputSlot.RemoveItemFromStack(itemToRemove);
            }
            else
            {
                itemToRemove = inputSlot.ObjectsAttach[0];
                inputSlot.RemoveItemFromSlot(itemToRemove);
            }
        }

        public void UpdateProgressFill(float actualTime)
        {
            float fillValue = (actualTime * 100.0f) / durationProcess;
            fillValue = fillValue / 100.0f;

            progressFillProcess.fillAmount = Mathf.MoveTowards(progressFillProcess.fillAmount, fillValue, Time.deltaTime);
        }

        public void ProcessMaterials()
        {
            if (!IsSeparatorProcessing.Invoke())
            {
                if (inputSlot != null)
                {
                    ItemModel firstItem = null;

                    if (inventoryController.StackTake)
                    {
                        if (inputSlot.StackOfItems.Stack.Count > 0)
                        {
                            firstItem = inventoryController.GetItemModelFromId(inputSlot.StackOfItems.Stack[0].ItemType);
                        }
                    }
                    else
                    {
                        if (inputSlot.ObjectsAttach.Count > 0)
                        {
                            firstItem = inventoryController.GetItemModelFromId(inputSlot.ObjectsAttach[0].ItemType);
                        }
                    }

                    GenerateProcessedItems(firstItem);

                    OnEndProcess();

                    //Mandarle dos items al action OnProcessMaterial sirve para el combinador.
                    //OnProcessMaterial?.Invoke(firstItem);
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void StopFill()
        {
            progressFillProcess.fillAmount = 0;
        }
        #endregion
    }
}