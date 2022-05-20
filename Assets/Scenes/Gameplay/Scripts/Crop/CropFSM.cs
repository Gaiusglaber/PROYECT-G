using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;

public class CropFSM : MonoBehaviour, IHittable
{
    [SerializeField] private float timeFirstStage;
    [SerializeField] private float timeSecondStage;
    [SerializeField] private float timeThirdStage;
    [SerializeField] private WorldItem cropPrefab;
    [SerializeField] private List<Sprite> spriteCycle = new List<Sprite>();
    //[SerializeField] private CropSlot cropSlot;

    private InventoryController InventoryController;
    private SpriteRenderer spriteRenderer;
    private int amountCrops = 0;
    private enum CropState
    {
        first,
        second,
        third
    };
    private CropState state;
    private float timerCropFSM;
    private bool isStarted;
    void Start()
    {
        timerCropFSM = 0.0f;
        InventoryController = FindObjectOfType<InventoryController>();
        //hacer esto mismo, que instancie un prefab de item world en la posicion del cultivo
        WorldItem worldItem = Instantiate(cropPrefab);
        worldItem.SetOnItemTaked(InventoryController.GenerateItem);
        SetCycle(true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        amountCrops = 1;
        state = CropState.first;
        //cropSlot.ActivatedSlot += SetCycle;
    }

    private void OnDisable()
    {
        //cropSlot.ActivatedSlot -= SetCycle;
    }

    void Update()
    {
        if (amountCrops <= 0)
        {
            SetCycle(true);
            NextStage(CropState.first);
        }
        StartCylce();
        Debug.Log("timer: " + timerCropFSM.ToString("F0"));
        Debug.Log("estado del bool isStarted: " + isStarted);
        Debug.Log("amount crops: " + amountCrops);
    }

    private void SetCycle(bool state)
    {
        isStarted = state;
    }

    private void StartCylce()
    {
        if (isStarted)
        {
            timerCropFSM += Time.deltaTime;
            switch (state)
            {
                case CropState.first:
                    if (timerCropFSM >= timeSecondStage)
                    {
                        NextStage(CropState.second);
                    }
                    break;
                case CropState.second:
                    if (timerCropFSM >= timeThirdStage)
                    {
                        NextStage(CropState.third);
                    }
                    break;
                case CropState.third:
                    SetCycle(false);
                    timerCropFSM = 0.0f;
                    break;
                default:
                    break;
            }
        }
    }

    private void NextStage(CropState stage)
    {
        state = stage;
        spriteRenderer.sprite = spriteCycle[(int)stage];
        //amountCrops++;
    }

    public void OnHit()
    {
        if (state != CropState.third)
            return;
        if(amountCrops <= 0)
        {
            Debug.Log("Comienza nuevo ciclo!");
            SetCycle(true);
            timerCropFSM = 0.0f;
            amountCrops = 1;
            NextStage(CropState.first);
            
        }else
        {
            amountCrops--;
            WorldItem crop = Instantiate(cropPrefab, transform.position, Quaternion.identity);
            crop.SetOnItemTaked(InventoryController.GenerateItem);
        }
        //if (amountCrops > 0)
        //{
        //   
        //}
        //else
        //{
        //    Debug.Log("Comienza nuevo ciclo!");
        //    SetCycle(true);
        //    corpInteraction.enabled = false;
        //    timerCropFSM = 0.0f;
        //    NextStage(0);
        //    return;
        //}
    }
}
