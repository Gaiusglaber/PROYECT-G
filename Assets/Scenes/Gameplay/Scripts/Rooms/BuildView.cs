using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

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
        private string nameBuild = null;
        private List<ResourceView> resourcesNeeded = new List<ResourceView>();
        #endregion

        #region PROPERTIES
        public string NameBuild { get { return nameBuild; } }
        #endregion

        #region PUBLIC_METHODS
        public void InitBuildView(Sprite image, string name, List<ResourceModel> resourcesNeeded, Action<string, Action<bool>> onBuildPressed,
             Action<string,Action<bool>> onDestroyPressed)
        {
            buildingImage.sprite = image;
            buildingName.text = name;

            nameBuild = name;

            btnBuild.onClick.AddListener(() => 
            {
                onBuildPressed?.Invoke(name, (opState) => 
                {
                    if(opState)
                    {
                        btnBuild.gameObject.SetActive(false);
                        btnDestroy.gameObject.SetActive(true);
                    }
                    else
                    {
                        btnBuild.gameObject.SetActive(true);
                        btnDestroy.gameObject.SetActive(false);
                    }
                });
            });

            btnDestroy.onClick.AddListener(() =>
            {
                onDestroyPressed?.Invoke(name,(opState) => 
                {
                    if(opState)
                    {
                        btnBuild.gameObject.SetActive(true);
                        btnDestroy.gameObject.SetActive(false);
                    }
                    else
                    {
                        btnBuild.gameObject.SetActive(false);
                        btnDestroy.gameObject.SetActive(true);
                    }
                });
            });

            ConfigureBuildResources(resourcesNeeded);
        }

        public void OnBuildCreated(bool state)
        {
            if (state)
            {
                btnBuild.gameObject.SetActive(false);
                btnDestroy.gameObject.SetActive(true);
            }
            else
            {
                btnBuild.gameObject.SetActive(true);
                btnDestroy.gameObject.SetActive(false);
            }
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