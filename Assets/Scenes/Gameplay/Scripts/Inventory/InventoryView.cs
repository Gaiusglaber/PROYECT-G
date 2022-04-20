using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.Objects.Inventory.View
{
    public class InventoryView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private SlotInventoryView prefabSlots = null;
        [SerializeField] private Animator animator = null;
        #endregion

        #region PRIVATE_FIELDS
        private Transform parentView = null;

        private SlotInventoryView[,] slotsView = null;
        #endregion

        #region PROPERTIES
        public bool IsOpen { set; get; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(InventoryModel model, Transform parentTarget)
        {
            parentView = parentTarget;

            InitializeSlotsView(model.GridRows,model.GridCols);

            for (int x = 0; x < model.GridRows; x++)
            {
                for (int y = 0; y < model.GridCols; y++)
                {
                    Vector2Int gPos = new Vector2Int(x, y);
                    Vector2 finalWorldPosition = new Vector2((parentView.position.x - (model.GridCols * 0.5f)), 
                        (parentView.position.y + (model.GridRows * 0.5f))) + model.GetSlot(gPos).SlotPosition;

                    SlotInventoryView newSlotInv = Instantiate(prefabSlots, finalWorldPosition, Quaternion.identity, parentView);
                    model.GetSlot(gPos).SetPosition(finalWorldPosition);
                    slotsView[x, y] = newSlotInv;
                }
            }

            IsOpen = true;
            OpenInventory();
        }

        public void OpenInventory()
        {
            if (animator == null) return;

            IsOpen = !IsOpen;
            animator.SetBool("IsOpen", IsOpen);
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitializeSlotsView(int rows, int cols)
        {
            slotsView = new SlotInventoryView[rows, cols];
        }
        #endregion
    }
}