using System;
using System.Collections.Generic;

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
        [SerializeField] private SlotInventoryView inputSlot;
        [SerializeField] private SlotInventoryView outputSlot1;
        [SerializeField] private SlotInventoryView outputSlot2;
        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private GameObject prefabItemView = null;
        [SerializeField] private GameObject panelSeparator = null;
        [SerializeField] private Canvas mainCanvas = null;
        [SerializeField] private Image progressFillProcess = null;
        public Action<ItemModel> OnProcessMaterial;
        public Action onCancelProcess = null;
        #endregion

        #region PRIVATE_FIELDS
        private Func<bool> isSeparatorProcessing = null;

        private float durationProcess = 0.0f;
        private bool extraPositionsCreated = false;

        private List<SlotInventoryView> thisUiSlotsView = new List<SlotInventoryView>();

        private List<Vector2Int> savedSlotPositons = new List<Vector2Int>();

        private Vector2Int invalidPosition = new Vector2Int(-1, -1);
        #endregion

        #region PROPERTIES
        public InventoryController InverntoryController { get { return inventoryController; } }
        public Func<bool> IsSeparatorProcessing { set { isSeparatorProcessing = value; } get { return isSeparatorProcessing; } }
        #endregion

        #region UNITY_CALLS
        void Update()
        {
            if(!initialized)
            {
                return;
            }

            inputSlot.SetOnInteractionInventoryChange(inventoryController.StackTake);
            outputSlot1.SetOnInteractionInventoryChange(inventoryController.StackTake);
            outputSlot2.SetOnInteractionInventoryChange(inventoryController.StackTake);
        }
        #endregion

        #region PUBLIC_METHODS
        public override void Init()
        {
            inputSlot.Init(prefabItemView, mainCanvas, invalidPosition, false);
            outputSlot1.Init(prefabItemView, mainCanvas, invalidPosition, false);
            outputSlot2.Init(prefabItemView, mainCanvas, invalidPosition, false);

            thisUiSlotsView.Add(inputSlot);
            thisUiSlotsView.Add(outputSlot1);
            thisUiSlotsView.Add(outputSlot2);

            extraPositionsCreated = false;

            onCancelProcess += StopFill;

            base.Init();
        }

        public void SetDurationProcess(float timeToSeparate)
        {
            durationProcess = timeToSeparate;
        }

        public void GenerateProcessedItems(ItemModel itemFrom1)
        {
            ItemModel finalItem = itemFrom1.itemResults[1]; //posicion 1 de la lista devuelve table item
            inventoryController.GenerateItem(finalItem.itemId, outputSlot1.GridPosition);

            ItemModel finalItem2 = itemFrom1.itemResults[2]; //posicion 2 de la lista devuelve salvia item
            inventoryController.GenerateItem(finalItem2.itemId, outputSlot2.GridPosition);
        }

        public void TogglePanel()
        {
            panelSeparator.SetActive(!panelSeparator.activeSelf);
            inventoryController.ToggleInventory();

            ThisIsAwfulButNeeded();
        }

        public void OnEndProcess()
        {
            //progressFillProcess.fillAmount = 0;

            inventoryController.RemoveItems(inputSlot.GridPosition, 1);

            inputSlot.UpdateTextOutStack();
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
                if (inventoryController.Model.GetSlot(inputSlot.GridPosition) == null)
                    return;

                if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems.Count > 0 && inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0].itemId == "Wood")
                {
                    if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0].itemResults.Count < 1)
                        return;

                    Debug.Log("Item send to separate");

                    ItemModel firstItem = inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems[0];

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

        private void ThisIsAwfulButNeeded()
        {
            if (panelSeparator.activeSelf)
            {
                if (extraPositionsCreated)
                    return;

                //Ojo esta parte! Esto es muy feo pero tuve que hacerlo asi porque no encontraba forma de resolverlo
                //literalmente estoy "extendiendo" el inventario para poder interactuar con la UI. Eventualmente esto
                //tendriamos que cambiarlo u encontrar algo mas optimo. Btw por ahora me sirve. :)
                Debug.Log("Creado de posiciones extra del inventario");

                inventoryController.ExtendInventoryWithExtraSlots(100, 101, 100, 103, thisUiSlotsView, ref savedSlotPositons);    //Puse 80/83 como para que sean slots imposibles de usar realmente

                inputSlot.SetSlotGridPosition(inventoryController.GetSlotFromGridPosition(savedSlotPositons[0]).GridPosition);
                outputSlot1.SetSlotGridPosition(inventoryController.GetSlotFromGridPosition(savedSlotPositons[1]).GridPosition);
                outputSlot2.SetSlotGridPosition(inventoryController.GetSlotFromGridPosition(savedSlotPositons[2]).GridPosition);

                Debug.Log("extra slots del separador 1: " + inventoryController.GetSlotFromGridPosition(savedSlotPositons[0]));
                Debug.Log("extra slots del separador 2: " + inventoryController.GetSlotFromGridPosition(savedSlotPositons[1]));
                Debug.Log("extra slots del separador 3: " + inventoryController.GetSlotFromGridPosition(savedSlotPositons[2]));


                extraPositionsCreated = true;
            }
            /*else
            {
                if (inventoryController.Model.GetSlot(inputSlot.GridPosition) == null ||
                    inventoryController.Model.GetSlot(outputSlot1.GridPosition) == null ||
                    inventoryController.Model.GetSlot(outputSlot2.GridPosition) == null)
                    return;


                if (inventoryController.Model.GetSlot(inputSlot.GridPosition).StackOfItems.Count > 0 ||
                    inventoryController.Model.GetSlot(outputSlot1.GridPosition).StackOfItems.Count > 0 ||
                    inventoryController.Model.GetSlot(outputSlot2.GridPosition).StackOfItems.Count > 0)
                    return;

                Debug.Log("Limpiado de posiciones extra del inventario");

                inputSlot.SetSlotGridPosition(invalidPosition);
                outputSlot1.SetSlotGridPosition(invalidPosition);
                outputSlot2.SetSlotGridPosition(invalidPosition);

                extraPositionsCreated = false;
            }*/
        }
        #endregion
    }
}