using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;
using ProyectG.Gameplay.Controllers;
using ProyectG.Gameplay.Objects.Inventory.Controller;

using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.UI;
using ProyectG.Gameplay.RoomSystem.Handler;
using ProyectG.Common.Modules.Audio.Channels.Sound;
using ProyectG.Common.Modules.Audio.Channels.Music;
using ProyectG.Common.UI.Dialogs;
using System;
using ProyectG.Gameplay.Objects.Inventory.Data;

namespace ProjectG.Gameplay.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("Settings")]
        [SerializeField] private Transform initialPosition = null;

        [Header("Main References")]
        [SerializeField] private PlayerController playerPrefab = null;
        [SerializeField] private InventoryController inventory = null;
        [SerializeField] private EnergyHandler energyHandler = null;
        [SerializeField] private CameraController cameraHandler = null;
        [SerializeField] private MouseController mouseController = null;
        [SerializeField] private RoombuilderHandler roomSystem = null;
        [SerializeField] private SoundHandlerChannel soundHandlerChannel = null;
        [SerializeField] private MusicHandlerChannel musicHandlerChannel = null;
        [SerializeField] private DialogManager dialogManager = null;
        [SerializeField] private DialogConversationSO[] conversationSOs = null;
        [SerializeField] private Animator dialogPanel = null;
        [SerializeField] private NPCHandler npcHandler = null;

        [Header("FOR TESTING")]
        [SerializeField] private List<ItemModel> testItems = null;
        #endregion

        #region PRIVATE_FIELDS
        private PlayerController player = null;

        private Action OnDialogStart = null;
        private Action OnDialogEnd = null;
        #endregion

        #region UNITY_CALLS
        private void Awake()
        {
            InitializePlayer();

            inventory.Init();
            cameraHandler.Init(player);
            mouseController.Init();

            if (dialogManager != null)
            {
                dialogManager.Init(TryDeleteItem, TryGiveItem);
                dialogManager.InitAllDialogPlayers(OpenPanel, OnDialogStart, OnDialogEnd);
                dialogManager.SetConversations(conversationSOs);
                dialogManager.OnDialogEnd += OnEndDialog;
            }
        }

        private void Start()
        {
            Time.timeScale = 1;

            roomSystem.Init(player, cameraHandler, inventory);

            soundHandlerChannel.OnPlaySound?.Invoke("CalmForest");
            musicHandlerChannel.OnPlayMusic?.Invoke("CalmMusic");

            inventory.GenerateItem("Wood", 50);
            inventory.GenerateItem("Coal", 50);
            //inventory.GenerateItem("Salvia", 25);
            inventory.GenerateItem("fruit", 50);
            inventory.GenerateItem("pollu", 1);
            inventory.GenerateItem("gashroom", 1);
            /*inventory.GenerateItem("saturnita_fuel", 25);
            inventory.GenerateItem("residue_toxic", 25);
            inventory.GenerateItem("CommonSeed", 25);
            inventory.GenerateItem("Rocks", 25);
            inventory.GenerateItem("StoragedUnstability", 25);
            inventory.GenerateItem("juice_fruit_toxic", 25);
            inventory.GenerateItem("gashroom", 25);
            inventory.GenerateItem("saturnita_core", 25);*/
        }

        private void Update()
        {
            InventoryUpdate();
        }

        private void OnDisable()
        {
            //EnergyHandler.Withoutenergy -= StopGame;
            //UIUpgradeTable.TradeSuccessfully -= StopGame;
        }

        #endregion

        #region PUBLIC_METHODS

        #endregion

        #region PRIVATE_METHODS
        public void OnEndDialog(bool toggle, string itemId)
        {
            dialogPanel.SetBool("IsOpen", false);
        }

        private bool TryDeleteItem(ItemModel item, int quantity)
        {
            bool result = inventory.Model.ConsumeItems(item, quantity);
            npcHandler.OnConversationEnd(dialogManager.ActualDialog.id, result);
            return result;
        }

        private void TryGiveItem(ItemModel item, int quantity)
        {
            inventory.GenerateItem(item.itemId, quantity);
            npcHandler.OnConversationEnd(dialogManager.ActualDialog.id, true);
        }

        public void OpenPanel()
        {
            dialogPanel.SetBool("IsOpen", true);
        }

        private void InitializePlayer()
        {
            if(initialPosition != null)
            {
                player = Instantiate(playerPrefab, initialPosition.position, initialPosition.rotation);
            }
            else
            {
                player = Instantiate(playerPrefab);
            }

            IEnumerator ActivatePlayerAfterTime()
            {
                yield return new WaitForSeconds(1f);
                
                if (player != null)
                {
                    player.Init(soundHandlerChannel);
                }
            }

            StartCoroutine(ActivatePlayerAfterTime());

            energyHandler.playerController = player;
        }

        private void InventoryUpdate()
        {
            if (inventory == null) return;

            inventory.CheckState();

            //inventory.UpdateInventory();
        }

        private void StopGame(bool stop)
        {
            if(stop)
                Time.timeScale = 0;
        }

        #endregion
    }
}