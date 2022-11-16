using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.View;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Objects.Inventory.Data;

using ProyectG.Gameplay.UI;

public class UICombinator : BaseView
{
    #region EXPOSED_FIELDS
    [Header("COMBINATOR VIEW")]
    [SerializeField] private MachineSlotView inputSlot_Left;
    [SerializeField] private MachineSlotView inputSlot_Right;
    [SerializeField] private MachineSlotView resultSlot;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private ItemView prefabItemView = null;
    [SerializeField] private GameObject panelCombinator = null;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Button btnCombine;
    #endregion

    #region PRIVATE_FIELDS
    private bool isInitialized = false;
    private List<ItemModel> possibleItemsResults = new List<ItemModel>();
    #endregion

    #region CONSTANTS
    private const int maxInputItems = 2;
    #endregion

    private void Start()
    {
        Init();
    }

    #region PUBLIC_METHODS
    public override void Init()
    {
        inputSlot_Left.Init(mainCanvas, inventoryController.OnHoverSelection);
        inputSlot_Right.Init(mainCanvas, inventoryController.OnHoverSelection);
        resultSlot.Init(mainCanvas, inventoryController.OnHoverSelection);

        inventoryController.OnInteractionChange += inputSlot_Left.SetOnInteractionInventoryChange;
        inventoryController.OnInteractionChange += inputSlot_Right.SetOnInteractionInventoryChange;
        inventoryController.OnInteractionChange += resultSlot.SetOnInteractionInventoryChange;

        possibleItemsResults = inventoryController.AllItemsAviable.Where(item => item.itemsFrom.Count >= maxInputItems).ToList();

        btnCombine.onClick.AddListener(ProcessAndGenerateResult);
    }

    public void ProcessAndGenerateResult()
    {
        ItemModel leftItem = null;
        ItemModel rightItem = null;

        if (inventoryController.StackTake)
        {
            if (inputSlot_Right.StackOfItems.Stack.Count > 0)
            {
                rightItem = inventoryController.GetItemModelFromId(inputSlot_Right.StackOfItems.Stack[0].ItemType);
            }
            if (inputSlot_Left.StackOfItems.Stack.Count > 0)
            {
                leftItem = inventoryController.GetItemModelFromId(inputSlot_Left.StackOfItems.Stack[0].ItemType);
            }
        }
        else
        {
            if (inputSlot_Right.ObjectsAttach.Count > 0)
            {
                rightItem = inventoryController.GetItemModelFromId(inputSlot_Right.ObjectsAttach[0].ItemType);
            }
            if (inputSlot_Left.ObjectsAttach.Count > 0)
            {
                leftItem = inventoryController.GetItemModelFromId(inputSlot_Left.ObjectsAttach[0].ItemType);
            }
        }

        if(leftItem == null || rightItem == null)
        {
            Debug.LogWarning("Some of the inputs slots are empty.");
            return;
        }

        List<ItemModel> inputItems = new List<ItemModel> { leftItem, rightItem };
        ItemModel itemResult = null;

        bool leftValidated = false;
        bool rightValidated = false;

        for (int i = 0; i < possibleItemsResults.Count; i++)
        {
            if (possibleItemsResults[i] != null)
            {
                for (int j = 0; j < possibleItemsResults[i].itemsFrom.Count; j++)
                {
                    if(!leftValidated)
                    {
                        leftValidated = possibleItemsResults[i].itemsFrom[j].itemId == leftItem.itemId;
                    }

                    if(!rightValidated)
                    {
                        rightValidated = possibleItemsResults[i].itemsFrom[j].itemId == rightItem.itemId;
                    }

                    if (leftValidated && rightValidated)
                    {
                        itemResult = possibleItemsResults[i];
                        break;
                    }
                }
            }
        }

        if(itemResult == null)
        {
            Debug.LogWarning("The items placed on the inputs field doesn't give anything on exchange.");
            return;
        }

        ItemView leftItemToRemove = null;
        ItemView rightItemToRemove = null;

        if (inventoryController.StackTake)
        {
            leftItemToRemove = inputSlot_Left.StackOfItems.Stack[0];
            rightItemToRemove = inputSlot_Right.StackOfItems.Stack[0];
            inputSlot_Left.RemoveItemFromStack(leftItemToRemove);
            inputSlot_Right.RemoveItemFromStack(rightItemToRemove);
        }
        else
        {
            leftItemToRemove = inputSlot_Left.ObjectsAttach[0];
            rightItemToRemove = inputSlot_Right.ObjectsAttach[0];
            inputSlot_Left.RemoveItemFromStack(leftItemToRemove);
            inputSlot_Right.RemoveItemFromStack(rightItemToRemove);
        }

        ItemView newViewResult = Instantiate(prefabItemView, resultSlot.SlotPosition, Quaternion.identity, resultSlot.transform);
        newViewResult.GenerateItem(mainCanvas, null, resultSlot, itemResult, inventoryController.Model.SiwtchItemsOnSlots, inventoryController.OnRemoveItems, inventoryController.OnAddItems);

        if (inventoryController.StackTake)
        {
            resultSlot.AddItemToStack(newViewResult);
        }
        else
        {
            resultSlot.AddItemToSlot(newViewResult);
        }
    }
    #endregion
}
