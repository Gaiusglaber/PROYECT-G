using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Gameplay.UI
{
    public class EnergyGenerator : MonoBehaviour
    {
        //Temp
        [SerializeField] private EnergyHandler energyHandler;
        [SerializeField] private int generateEnergyValue = 10;
        int energyToGenerate = 0;
        private bool isCollidingWithPlayer;

        void Start()
        {
            isCollidingWithPlayer = false;
        }

        private void Update()
        {
            if (isCollidingWithPlayer)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    energyToGenerate += generateEnergyValue;
                    energyHandler.UpdateEnergy(energyToGenerate);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            isCollidingWithPlayer = false;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Esta dentro del collider!");
                isCollidingWithPlayer = true;
            }
        }
    }
}