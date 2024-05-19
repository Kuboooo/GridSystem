using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIManager : MonoBehaviour {

        public static event Action<GameObject> OnPreviewingBuilding;
        public static event Action<object, EventArgs> OnStopPreviewing;

        private static void PreviewBuilding(GameObject buildingPrefab)
        {
            OnPreviewingBuilding?.Invoke(buildingPrefab);
        }

        private static void StopPreviewing()
        {
            OnStopPreviewing?.Invoke(null, EventArgs.Empty);
        }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private Button building1;
        [SerializeField] private Button building2;
        [SerializeField] private Button cancelPreview;
        [SerializeField] private TextMeshProUGUI buildingsCount;
        // TODO KUBO pull these somehow automatically out of some SOlist
        [SerializeField] private PreviewBuildingSO previewBuildingSO;
        [SerializeField] private PreviewBuildingSO previewBuildingSphereSO;

        private GameObject previewInstance;

        private void Awake() {
            building1.onClick.AddListener(() => {
                if (previewInstance != null) Destroy(previewInstance);
                ShowPreview(previewBuildingSO.prefabToPreview.GameObject());
            });        
            building2.onClick.AddListener(() => {
                if (previewInstance != null) Destroy(previewInstance);
                ShowPreview(previewBuildingSphereSO.prefabToPreview.GameObject());
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
    
        private void ShowPreview(GameObject prefab) {
            PreviewBuilding(prefab);
        }

    }
}
