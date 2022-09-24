using UnityEngine;
using UnityEngine.UI;

using ProyectG.Gameplay.Objects.Inventory.Data;

using TMPro;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class ResourceView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TMP_Text amountResources = null;
        [SerializeField] private Image imageResource = null;
        #endregion

        #region PUBLIC_METHODS
        public void Init(ResourceModel resource)
        {
            amountResources.text = resource.amount.ToString();
            imageResource.sprite = resource.item.itemSprite;
        }
        #endregion
    }

    [System.Serializable]
    public class ResourceModel
    {
        public string name;
        public int amount;
        public ItemModel item;
    }
}