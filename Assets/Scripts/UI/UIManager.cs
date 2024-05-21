using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIManager : MonoBehaviour {

        public static event Action<PreviewBuildingSO> OnPreviewingBuilding;
        public static event Action<object, EventArgs> OnStopPreviewing;

        private static void PreviewBuilding(PreviewBuildingSO buildingSO)
        {
            OnPreviewingBuilding?.Invoke(buildingSO);
        }

        private static void StopPreviewing()
        {
            OnStopPreviewing?.Invoke(null, EventArgs.Empty);
        }

        [SerializeField] private Button building1;
        [SerializeField] private Button building2;
        [SerializeField] private Button building3;
        [SerializeField] private Button cancelPreview;
        [SerializeField] private TextMeshProUGUI buildingsCount;
        // TODO KUBO pull these somehow automatically out of some SOlist
        [SerializeField] private PreviewBuildingSO previewBuildingSO;
        [SerializeField] private PreviewBuildingSO previewBuildingSphereSO;
        [SerializeField] private PreviewBuildingSO previewBuildingThreeHexSO;

        [SerializeField] private Button hideBuildingUIButton;
        [SerializeField] private GameObject buildingUIToHide;
        [SerializeField] private Button hideStatsUIButton;
        [SerializeField] private GameObject statsUIToHide;
        [SerializeField] private Button hideRestaurantUIButton;
        [SerializeField] private GameObject restaurantUIToHide;
        [SerializeField] private GameObject gridUIToHide;

        private void Start() {
            building1.onClick.AddListener(() => {
                ShowPreview(previewBuildingSO);
            });        
            building2.onClick.AddListener(() => {
                ShowPreview(previewBuildingSphereSO);
            });
                        
            building3.onClick.AddListener(() => {
                ShowPreview(previewBuildingThreeHexSO);
            });
            
            hideBuildingUIButton.onClick.AddListener(() => {
                buildingUIToHide.SetActive(!buildingUIToHide.activeInHierarchy);
            });
            hideStatsUIButton.onClick.AddListener(() => {
                statsUIToHide.SetActive(!statsUIToHide.activeInHierarchy);
            });
            hideRestaurantUIButton.onClick.AddListener(() => {
                restaurantUIToHide.SetActive(!restaurantUIToHide.activeInHierarchy);
                gridUIToHide.SetActive(!gridUIToHide.activeInHierarchy);
            });
            
            cancelPreview.onClick.AddListener(StopPreviewing);
            
            buildingsCount.text = "0";
        }

        private void OnEnable() {
            
            MouseCoordinates.OnBuildingBuilt += OnBuildingBuilt;
        }
        
        private void OnDisable() {
            MouseCoordinates.OnBuildingBuilt -= OnBuildingBuilt;
        }

        private void OnBuildingBuilt(object arg1, EventArgs arg2) {
            buildingsCount.text = Convert.ToString(Convert.ToInt32(buildingsCount.text) + 1);
        }
    
        private void ShowPreview(PreviewBuildingSO previewBuildingSo) {
            PreviewBuilding(previewBuildingSo);
        }

    }
}
