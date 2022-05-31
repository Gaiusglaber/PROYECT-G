using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;
using ProyectG.Gameplay.Controllers;
using ProyectG.Gameplay.Objects.Inventory.Controller;

using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.UI;

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

        [Header("FOR TESTING")]
        [SerializeField] private List<WorldItem> testItems = null;
        #endregion

        #region PRIVATE_FIELDS
        private PlayerController player = null;
        #endregion

        #region UNITY_CALLS
        private void Awake()
        {
            InitializePlayer();

            inventory.Init();
            cameraHandler.Init(player);
            mouseController.Init();

            for (int i = 0; i < testItems.Count; i++)
            {
                if(testItems[i] != null)
                {
                    testItems[i].SetOnItemTaked(inventory.GenerateItem);
                }
            }

            inventory.GenerateItem("TestItem1");
            inventory.GenerateItem("TestItem1");
            inventory.GenerateItem("TestItem1");
        }

        private void Start()
        {
            Time.timeScale = 1;
            EnergyHandler.Withoutenergy += StopGame;
        }

        private void Update()
        {
            InventoryUpdate();
        }

        private void OnDisable()
        {
            EnergyHandler.Withoutenergy -= StopGame;
        }

        #endregion

        #region PUBLIC_METHODS

        #endregion

        #region PRIVATE_METHODS
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
                    player.Init();
                }
            }

            StartCoroutine(ActivatePlayerAfterTime());

            energyHandler.playerController = player;
        }

        private void InventoryUpdate()
        {
            if (inventory == null) return;

            inventory.CheckState();

            inventory.UpdateInventory();
        }

        private void StopGame(bool stop)
        {
            if(stop)
                Time.timeScale = 0;
        }

        #endregion
    }
}