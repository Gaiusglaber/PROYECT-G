using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ProyectG.Gameplay.Objects.Inventory.Data
{
    [Serializable]
    public enum ItemType
    {
        fuel,
        common,
        crop
    }
    [Serializable]
    [CreateAssetMenu(fileName = "ItemModel", menuName = "ScriptableObjects/Data/Items", order = 0)]
    public class ItemModel : ScriptableObject
    {
        #region PROTECTED_FIELDS
        [Header("ITEM MODEL SETTINGS")]
        public string itemId = string.Empty;
        public Sprite itemSprite = null;

        [TextArea(1,8)]
        public string itemDescription = string.Empty;
        public int itemValue = 0;
        public int energyCost = 0;
        public float timeToComplete = 0;
        public float costInterval = 0;
        public int pollutantDecreaseMaxEnergy;
        public bool isPollutant = false;

        public ItemType itemType;


        [Header("ITEM PROCESS RESULT")]
        public List<ItemModel> itemResults = new List<ItemModel>();
        [Header("GENERATED FROM ITEMS")]
        public List<ItemModel> itemsFrom = new List<ItemModel>();
        
        [Header("OPPOSITE ITEMS")]
        public List<ItemModel> oppositeItems = new List<ItemModel>();
        public float costOpposite = 0f;
        public float timeToCompleteOpposite = 0;
        #endregion
    }
}