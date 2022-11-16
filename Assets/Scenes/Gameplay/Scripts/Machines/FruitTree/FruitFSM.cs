using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProyectG.Gameplay.Objects;
using ProyectG.Gameplay.Objects.Inventory.Controller;
using ProyectG.Gameplay.Interfaces;
using UnityEngine.U2D.Animation;


public class FruitFSM : Machine, IHittable
{
    private enum FruitState
    {
        first,
        second,
    }

    [SerializeField] private float timeSecondStage;
    [SerializeField] private int amountPerFarm = 3;
    [SerializeField] private int hitsNeededToFarm = 0;
    [SerializeField] private WorldItem woodPrefab;
    [SerializeField] private List<Sprite> spriteCycle = new List<Sprite>();
    [SerializeField] private SpriteSkin spriteSkin = null;
    [SerializeField] private List<Transform> bones = null;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnHit()
    {
        throw new System.NotImplementedException();
    }
}
