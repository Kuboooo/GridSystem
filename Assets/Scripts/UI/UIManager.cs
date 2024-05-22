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
        [SerializeField] private TextMeshProUGUI populationCount;
        [SerializeField] private TextMeshProUGUI moneyCount;
        // TODO KUBO pull these somehow automatically out of some SOlist
        [SerializeField] private PreviewBuildingSO previewBuildingVillageSO;
        [SerializeField] private PreviewBuildingSO previewBuildingPondSO;
        [SerializeField] private PreviewBuildingSO previewBuildingPizzeriaSO;

        [SerializeField] private Button hideBuildingUIButton;
        [SerializeField] private GameObject buildingUIToHide;
        [SerializeField] private Button hideStatsUIButton;
        [SerializeField] private GameObject statsUIToHide;
        [SerializeField] private Button hideRestaurantUIButton;
        [SerializeField] private GameObject restaurantUIToHide;
        [SerializeField] private GameObject gridUIToHide;
        private float currentIncome = 0f;
        private float incomePerPopulation = 1f;
        private int pizzaValue = 1;

        private float timeSinceLastAction = 0f;
        private float interval = 10f; // Interval in seconds



        private int currentPopulation = 1;

        private void Start() {
            building1.onClick.AddListener(() => {
                if (int.Parse(moneyCount.text) >= previewBuildingVillageSO.cost) {
                    ShowPreview(previewBuildingVillageSO);
                }
            });        
            building2.onClick.AddListener(() => {
                if (int.Parse(moneyCount.text) >= previewBuildingPondSO.cost) {
                ShowPreview(previewBuildingPondSO);
                }
            });
                        
            building3.onClick.AddListener(() => {
                if (int.Parse(moneyCount.text) >= previewBuildingPizzeriaSO.cost) {
                    ShowPreview(previewBuildingPizzeriaSO);
                }
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
            
            populationCount.text = "5";
            moneyCount.text = "50";
        }

        private void Update() {
            timeSinceLastAction += Time.deltaTime;
            if (timeSinceLastAction >= interval) {
                currentIncome = currentPopulation * incomePerPopulation * pizzaValue;

                moneyCount.text = (currentIncome + int.Parse(moneyCount.text)).ToString();
                timeSinceLastAction = 0f;
            }

            enableButtons();

            }

        private void enableButtons() {
if (int.Parse(moneyCount.text) >= previewBuildingVillageSO.cost) {
                building1.interactable = true;
            } else {
                building1.interactable = false;
            }
            if (int.Parse(moneyCount.text) >= previewBuildingPondSO.cost) {
                building2.interactable = true;
            } else {
                building2.interactable = false;
            }
            if (int.Parse(moneyCount.text) >= previewBuildingPizzeriaSO.cost) {
                building3.interactable = true;
            } else {
                building3.interactable = false;
            }
        }

        private void OnEnable() {

            MouseCoordinates.OnBuildingBuilt += OnBuildingBuilt;
            RestaurantUIManager.OnToppingsSelected += OnToppingsSelected;
        }
        
        private void OnDisable() {
            MouseCoordinates.OnBuildingBuilt -= OnBuildingBuilt;
            RestaurantUIManager.OnToppingsSelected -= OnToppingsSelected;
        }

        private void OnToppingsSelected(int toppingsValue) {
            pizzaValue = toppingsValue;
        }

        private void OnBuildingBuilt(object arg1, PreviewBuildingSO previewBuildingSO) {
            currentPopulation += previewBuildingSO.basePopulationGrowth;
            moneyCount.text = (int.Parse(moneyCount.text) - previewBuildingSO.cost).ToString();

            populationCount.text = currentPopulation.ToString();
        }
 
        private void ShowPreview(PreviewBuildingSO previewBuildingSo) {
            PreviewBuilding(previewBuildingSo);
        }

    }
}
