using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class CropSlot : MonoBehaviour
{
    public Action<bool> ActivatedSlot;
    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //obtener el item del inventario del player que sea crop
            ActivatedSlot?.Invoke(true);
        }
    }
    //hacer que el ActivatedSlot se llame cuando detecte que el player planto un recurso

    public void NextStage(int stage)
    {
        //cambiar el item a uno nuevo para la siguiente fase
    }

}
