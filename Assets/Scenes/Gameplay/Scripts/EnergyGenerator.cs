using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProyectG.Gameplay.UI
{
    public class EnergyGenerator : MonoBehaviour
    {
        //Temp
        [SerializeField] private EnergyHandler energyHandler;
        private const int generateEnergyValue = 10;

        void Start()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    energyHandler.UpdateEnergy(generateEnergyValue);
                }
            }
        }
    }
}