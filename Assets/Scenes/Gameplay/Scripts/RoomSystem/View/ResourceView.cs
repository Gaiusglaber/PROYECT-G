using UnityEngine;
using UnityEngine.UI;

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
        public void Init(int amount, Sprite resourceImg)
        {
            amountResources.text = amount.ToString();
            imageResource.sprite = resourceImg;
        }
        #endregion
    }
}