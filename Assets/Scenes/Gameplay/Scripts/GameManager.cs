using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Controller;
using ProyectG.Gameplay.Objects.Inventory;

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
        #endregion

        #region PRIVATE_FIELDS
        private PlayerController player = null;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            InitializePlayer();

            inventory.Init();
        }

        private void Update()
        {
            PlayerUpdate();
            InventoryUpdate();
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

            if (player != null)
            {
                player.Init();
            }
        }

        private void PlayerUpdate()
        {
            if (player == null) return;

            player.UpdatePlayerController();
        }

        private void InventoryUpdate()
        {
            if (inventory == null) return;

            inventory.CheckState();
        }
        #endregion
    }
}