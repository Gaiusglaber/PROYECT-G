using System;
using UnityEngine;

using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProyectG.Gameplay.Objects.Machines
{
    public class Opposite : Machine
    {
        #region EXPOSED_FILEDS
        [SerializeField] private EnergyHandler energyHandler = null;
        #endregion

        #region PRIVATE_FIELDS
        private bool isProcessing = false;
        private float timerProcess;
        private float timeToProcessObject = 0;
        
        private ItemModel itemPorcessed = null;
        
        private UIOppositeMachine uiOppositeMachine;
        #endregion
        
        #region ACTIONS
        public Action<ItemModel> OnProcessMaterial;
        
        private Action OnItemProcessed = null;
        private Action OnItemConsumed = null;
        #endregion

        #region UNITY_CALLS
        protected override void Update()
        {
            base.Update();

            if (isProcessing)
            {
                if (timerProcess < timeToProcessObject)
                {
                    timerProcess += Time.deltaTime;

                    energyHandler.ConsumeEnergyByProcess();

                    uiOppositeMachine.UpdateProgressFill(timerProcess);

                    Debug.Log("Item processing");
                }
                else
                {
                    uiOppositeMachine.GenerateOppositeItem(itemPorcessed, (state) => 
                    {
                        if(state)
                        {
                            timerProcess = 0;
                            isProcessing = false;

                            itemPorcessed = null;

                            OnItemProcessed?.Invoke();

                            Debug.Log("Item processed successfully");
                        }
                        else
                        {
                            Debug.Log("The machine has in the output a item of different type, remove it and you will can process more items.");
                        }
                    });
                }
            }
        }
        #endregion

        #region OVERRIDE
        public override void Init(BaseView viewAttach)
        {
            base.Init(viewAttach);
            
            uiOppositeMachine = viewAttach as UIOppositeMachine;
            
            uiOppositeMachine.IsOppositeProcessing = IsProcessing;

            uiOppositeMachine.OnProcessItem = SetProcess;
            OnItemProcessed = uiOppositeMachine.OnEndProcess;
            OnItemConsumed = uiOppositeMachine.OnConsumeItem;

            isProcessing = false;
            timerProcess = 0.0f;

            energyHandler = FindObjectOfType<EnergyHandler>();
        }

        #endregion

        #region PRIVATE_METHODS
        private bool IsProcessing()
        {
            return isProcessing;
        }
        
        private void SetProcess(ItemModel item)
        {
            if(item != null)
            {
                if(item.itemResults.Count < 1)
                {
                    Debug.LogWarning("The item model hasn't any result to process");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("The item model to process is null");
                return;
            }
        
            isProcessing = true;

            itemPorcessed = item;

            timeToProcessObject = itemPorcessed.timeToComplete;
            uiOppositeMachine.SetDurationProcess(timeToProcessObject);

            energyHandler.SetCostOfProcessDecrement(itemPorcessed.energyCost, itemPorcessed.costInterval);

            OnItemConsumed?.Invoke();
        }
        #endregion
    }
}