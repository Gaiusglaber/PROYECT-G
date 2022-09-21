using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class BuildView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private ResourceView prefabResourceView = null;
        [SerializeField] private GameObject holderResources = null;
        [SerializeField] private Image buildingImage = null;
        [SerializeField] private TMP_Text buildingName = null;

        [Header("Interactions")]
        [SerializeField] private Button btnBuild = null;
        [SerializeField] private Button btnDestroy = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<ResourceView> resourcesNeeded = new List<ResourceView>();
        #endregion

        #region PUBLIC_METHODS
        public void InitBuildView(Sprite image, string name, List<ResourceModel> resourcesNeeded)
        {
            buildingImage.sprite = image;
            buildingName.text = name;

            ConfigureBuildResources(resourcesNeeded);
        }
        #endregion

        #region PRIVATE_FIELDS
        private void ConfigureBuildResources(List<ResourceModel> resources)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i] != null)
                {
                    ResourceView resourceView = Instantiate(prefabResourceView, holderResources.transform);

                    resourceView.Init(resources[i]);

                    resourcesNeeded.Add(resourceView);
                }
            }
        }
        #endregion
    }
}