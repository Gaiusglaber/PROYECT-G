using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;
using UnityEngine.U2D.Animation;

public class SaturnitaFSM : Machine, IHittable
{
    private enum SaturnitaState
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
    [SerializeField] private WorldItem saturnitaPrefab;
    [SerializeField] private List<Sprite> spriteCycle = new List<Sprite>();
    [SerializeField] private SpriteSkin spriteSkin = null;
    [SerializeField] private List<Transform> bones = null;

    //privaste fields
    private InventoryController inventoryController;
    private SpriteRenderer spriteRenderer;
    private int amountSaturnita;
    private SaturnitaState state;
    private float timerSaturnitaFSM;
    private bool isStarted;

    private int amountHits = 0;

    void Start()
    {
        timerSaturnitaFSM = 0.0f;
        inventoryController = FindObjectOfType<InventoryController>();
        SetCycle(true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        amountSaturnita = 1;
        state = SaturnitaState.first;
        Init(null);
    }

    protected override void Update()
    {
        base.Update();

        if (amountSaturnita <= 0)
        {
            SetCycle(true);
            timerSaturnitaFSM = 0.0f;
            amountSaturnita = 1;
            NextStage(SaturnitaState.first);
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
            timerSaturnitaFSM += Time.deltaTime;
            switch (state)
            {
                case SaturnitaState.first:
                    if (timerSaturnitaFSM >= timeSecondStage)
                    {
                        NextStage(SaturnitaState.second);
                    }
                    break;
                case SaturnitaState.second:
                    if (timerSaturnitaFSM >= timeThirdStage)
                    {
                        NextStage(SaturnitaState.third);
                    }
                    break;
                case SaturnitaState.third:
                    SetCycle(false);
                    timerSaturnitaFSM = 0.0f;
                    break;
                default:
                    break;
            }
        }
    }

    private void NextStage(SaturnitaState stage)
    {
        state = stage;
        spriteRenderer.sprite = spriteCycle[(int)stage];
    }

    public void OnHit()
    {

        if (state != SaturnitaState.third)
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

        if (amountSaturnita <= 0)
        {
            SetCycle(true);
            timerSaturnitaFSM = 0.0f;
            amountSaturnita = 1;
            NextStage(SaturnitaState.first);
        }
        else
        {
            amountSaturnita--;
            WorldItem saturnita = Instantiate(saturnitaPrefab, transform.position + (Vector3.up * heightOffset), Quaternion.identity);
            saturnita.SetOnItemTaked(inventoryController.GenerateItem);
        }

        amountHits = 0;
        //throw new System.NotImplementedException();
    }
}
