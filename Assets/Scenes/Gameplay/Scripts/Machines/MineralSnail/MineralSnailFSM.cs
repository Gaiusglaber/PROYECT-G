using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;
using UnityEngine.U2D.Animation;


public class MineralSnailFSM : Machine, IHittable
{
    private enum SnailState
    {
        first,
        second,
        third
    }

    [SerializeField] private float timeFirstStage;
    [SerializeField] private float timeSecondStage;
    [SerializeField] private float timeThirdStage;
    [SerializeField] private int amountPerFarm = 3;
    [SerializeField] private int hitsNeededToFarm = 0;
    [SerializeField] private float heightOffset = 0;
    [SerializeField] private WorldItem rockPrefab;
    [SerializeField] private List<Sprite> spriteCycle = new List<Sprite>();
    [SerializeField] private SpriteSkin spriteSkin = null;
    [SerializeField] private List<Transform> bones = null;

    //privaste fields
    private InventoryController inventoryController;
    private SpriteRenderer spriteRenderer;
    private int amountSnail;
    private SnailState state;
    private float timerMineralSnailFSM;
    private bool isStarted;

    private int amountHits = 0;
    void Start()
    {
        timerMineralSnailFSM = 0.0f;
        inventoryController = FindObjectOfType<InventoryController>();
        SetCycle(true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        amountSnail = 1;
        state = SnailState.first;
        Init(null);
    }

    protected override void Update()
    {
        base.Update();

        if (amountSnail <= 0)
        {
            SetCycle(true);
            timerMineralSnailFSM = 0.0f;
            amountSnail = 1;
            NextStage(SnailState.first);
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
            timerMineralSnailFSM += Time.deltaTime;
            switch (state)
            {
                case SnailState.first:
                    if (timerMineralSnailFSM >= timeSecondStage)
                    {
                        NextStage(SnailState.second);
                    }
                    break;
                case SnailState.second:
                    if (timerMineralSnailFSM >= timeThirdStage)
                    {
                        NextStage(SnailState.third);
                    }
                    break;
                case SnailState.third:
                    SetCycle(false);
                    timerMineralSnailFSM = 0.0f;
                    break;
                default:
                    break;
            }
        }
    }

    private void NextStage(SnailState stage)
    {
        state = stage;
        spriteRenderer.sprite = spriteCycle[(int)stage];
    }

    public void OnHit()
    {
        if (state != SnailState.third)
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

        if (amountSnail <= 0)
        {
            SetCycle(true);
            timerMineralSnailFSM = 0.0f;
            amountSnail = 1;
            NextStage(SnailState.first);
        }
        else
        {
            amountSnail--;
            WorldItem mushroom = Instantiate(rockPrefab, transform.position + (Vector3.up * heightOffset), Quaternion.identity);
            mushroom.SetOnItemTaked(inventoryController.GenerateItem);
        }

        amountHits = 0;
        //throw new System.NotImplementedException();
        //throw new System.NotImplementedException();
    }
}
