using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;
using UnityEngine.U2D.Animation;


public class Mushroom : Machine, IHittable
{
    private enum MushroomState
    {
        first,
        second,
        third
    }

    //Exposed fields
    [SerializeField] private float timeFirstStage;
    [SerializeField] private float timeSecondStage;
    [SerializeField] private float timeThirdStage;
    [SerializeField] private int amountPerFarm = 3;
    [SerializeField] private int hitsNeededToFarm = 0;
    [SerializeField] private float heightOffset = 0;
    [SerializeField] private WorldItem gashroomPrefab;
    [SerializeField] private List<Sprite> spriteCycle = new List<Sprite>();
    [SerializeField] private SpriteSkin spriteSkin = null;
    [SerializeField] private List<Transform> bones = null;

    //privaste fields
    private InventoryController inventoryController;
    private SpriteRenderer spriteRenderer;
    private int amountGashroom;
    private MushroomState state;
    private float timerMushroomFSM;
    private bool isStarted;

    private int amountHits = 0;
    void Start()
    {
        timerMushroomFSM = 0.0f;
        inventoryController = FindObjectOfType<InventoryController>();
        SetCycle(true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        amountGashroom = 1;
        state = MushroomState.first;
        Init(null);
    }

    protected override void Update()
    {
        base.Update();

        if (amountGashroom <= 0)
        {
            SetCycle(true);
            timerMushroomFSM = 0.0f;
            amountGashroom = 1;
            NextStage(MushroomState.first);
        }
        StartCycle();
    }

    private void SetCycle(bool state)
    {
        isStarted = state;
    }

    private void StartCycle()
    {
        if (isStarted)
        {
            timerMushroomFSM += Time.deltaTime;
            switch (state)
            {
                case MushroomState.first:
                    if (timerMushroomFSM >= timeSecondStage)
                    {
                        NextStage(MushroomState.second);
                    }
                    break;
                case MushroomState.second:
                    if (timerMushroomFSM >= timeThirdStage)
                    {
                        NextStage(MushroomState.third);
                    }
                    break;
                case MushroomState.third:
                    SetCycle(false);
                    timerMushroomFSM = 0.0f;
                    break;
                default:
                    break;
            }
        }
    }

    private void NextStage(MushroomState stage)
    {
        state = stage;
        spriteRenderer.sprite = spriteCycle[(int)stage];
    }

    public void OnHit()
    {
        if (state != MushroomState.third)
            return;

        if (amountHits < hitsNeededToFarm - 1)
        {
            amountHits++;
            TriggerAnimation("OnHit");
            TriggerSoundEffect("PlantChop");
            return;
        }

        TriggerAnimation("OnHit");
        TriggerSoundEffect("PlantChop");

        if (amountGashroom <= 0)
        {
            SetCycle(true);
            timerMushroomFSM = 0.0f;
            amountGashroom = 1;
            NextStage(MushroomState.first);
        }
        else
        {
            amountGashroom--;
            WorldItem mushroom = Instantiate(gashroomPrefab, transform.position + (Vector3.up * heightOffset), Quaternion.identity);
            mushroom.SetOnItemTaked(inventoryController.GenerateItem);
        }

        amountHits = 0;
        //throw new System.NotImplementedException();
    }
}
