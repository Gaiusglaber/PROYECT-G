using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects.Inventory.Data;

public class Furance : MonoBehaviour
{
    [SerializeField] private float maxTimeToBurn;
    [SerializeField] private UIFurance uiFurance;

    private List<ItemModel> furanceInventory = new List<ItemModel>();

    private float timerBurn;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            uiFurance.ShowPanel(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiFurance.ShowPanel(true);
        }
    }

    public void CheckInputSlot()
    {
        //if (uiFurance.inputSlot)
    }

    public void BurnFuel()
    {
        if (timerBurn <= maxTimeToBurn)
        {
            timerBurn += Time.deltaTime;
            //llenar la barra indicadora de quema de combustible
        }
    }
}
