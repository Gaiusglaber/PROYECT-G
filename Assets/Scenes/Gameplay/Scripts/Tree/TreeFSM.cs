using System.Collections.Generic;

using UnityEngine;
using UnityEngine.U2D.Animation;

using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;

public class TreeFSM : Machine, IHittable
{
    [SerializeField] private float timeSecondStage;
    [SerializeField] private int amountPerFarm = 3;
    [SerializeField] private int hitsNeededToFarm = 0;
    [SerializeField] private WorldItem woodPrefab;
    [SerializeField] private List<Sprite> spriteCycle = new List<Sprite>();
    [SerializeField] private SpriteSkin spriteSkin = null;
    [SerializeField] private List<Transform> bones = null;

    private InventoryController InventoryController;
    private SpriteRenderer spriteRenderer;
    private int amountLogs = 0;

    private int amountHits = 0;
    
    private enum TreeState
    {
        first,
        second,
    };
    
    private TreeState state;
    private float timerTreeFSM;
    private bool isStarted;
    void Start()
    {
        timerTreeFSM = 0.0f;
        InventoryController = FindObjectOfType<InventoryController>();

        SetCycle(true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        amountLogs = 1;
        state = TreeState.first;

        amountHits = 0;

        Init(null);
    }

    void Update()
    {
        if (amountLogs <= 0)
        {
            SetCycle(true);
            NextStage(TreeState.first);
            timerTreeFSM = 0f;
            amountLogs = 1;
        }
        StartCylce();
    }

    private void SetCycle(bool state)
    {
        isStarted = state;
    }

    private void StartCylce()
    {
        if (isStarted)
        {
            timerTreeFSM += Time.deltaTime;

            switch (state)
            {
                case TreeState.first:
                    if (timerTreeFSM >= timeSecondStage)
                    {
                        NextStage(TreeState.second);
                    }
                    break;
                case TreeState.second:
                    SetCycle(false);
                    timerTreeFSM = 0.0f;
                    break;
                default:
                    break;
            }
        }
    }

    private void NextStage(TreeState stage)
    {
        state = stage;
        spriteRenderer.sprite = spriteCycle[(int)stage];
    }

    public void OnHit()
    {
        if (state != TreeState.second)
            return;

        if (amountHits < hitsNeededToFarm - 1)
        {
            amountHits++;
            Debug.Log("HIT");
            TriggerAnimation("OnHit");
            return;
        }

        Debug.Log("HIT");
        TriggerAnimation("OnHit");

        if (amountLogs <= 0)
        {
            SetCycle(true);
            timerTreeFSM = 0f;
            NextStage(TreeState.first);
            amountLogs = 1;
        }
        else
        {
            amountLogs--;

            for (int i = 0; i < amountPerFarm; i++)
            {
                WorldItem wood = Instantiate(woodPrefab, transform.position, Quaternion.identity);
                wood.SetOnItemTaked(InventoryController.GenerateItem);
            }
        }

        amountHits = 0;
    }
}
