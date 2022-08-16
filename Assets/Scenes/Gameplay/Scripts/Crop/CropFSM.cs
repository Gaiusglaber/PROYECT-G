using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;

public class CropFSM : MonoBehaviour, IHittable
{
    #region ENUMS
    private enum CropState
    {
        first,
        second,
        third
    };
    #endregion

    #region EXPOSED_FIELDS
    [SerializeField] private float timeFirstStage;
    [SerializeField] private float timeSecondStage;
    [SerializeField] private float timeThirdStage;
    [SerializeField] private float heightOffset = 0;
    [SerializeField] private WorldItem cropPrefab;
    [SerializeField] private List<Sprite> spriteCycle = new List<Sprite>();
    #endregion

    #region PRIVATE_FIELDS
    private InventoryController InventoryController;
    private SpriteRenderer spriteRenderer;
    private int amountCrops = 0;
    private CropState state;
    private float timerCropFSM;
    private bool isStarted;
    #endregion

    #region UNITY_CALLS
    void Start()
    {
        timerCropFSM = 0.0f;
        InventoryController = FindObjectOfType<InventoryController>();
        SetCycle(true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        amountCrops = 1;
        state = CropState.first;
    }

    void Update()
    {
        if (amountCrops <= 0)
        {
            SetCycle(true);
            timerCropFSM = 0.0f;
            amountCrops = 1;
            NextStage(CropState.first);
        }
        StartCylce();
    }
    #endregion

    #region PRIVATE_METHODS

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
    }
    #endregion

    #region INTERFACES
    public void OnHit()
    {
        if (state != CropState.third)
            return;
        if(amountCrops <= 0)
        {
            SetCycle(true);
            timerCropFSM = 0.0f;
            amountCrops = 1;
            NextStage(CropState.first);
            
        }else
        {
            amountCrops--;
            WorldItem crop = Instantiate(cropPrefab, transform.position + (Vector3.up * heightOffset), Quaternion.identity);
            crop.SetOnItemTaked(InventoryController.GenerateItem);
        }
    }
    #endregion
}
