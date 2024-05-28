using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public class UIManager : MonoBehaviour {

        public static event Action<PreviewBuildingSO, Dictionary<int, Dictionary<int, List<Vector3>>>> OnPreviewingBuilding;
        public static event Action<object, EventArgs> OnStopPreviewing;

        private static void PreviewBuilding(PreviewBuildingSO buildingSO, Dictionary<int, Dictionary<int, List<Vector3>>> waypoints) {
            OnPreviewingBuilding?.Invoke(buildingSO, waypoints);
        }

        private static void StopPreviewing() {
            OnStopPreviewing?.Invoke(null, EventArgs.Empty);
        }

        [SerializeField] private Button natureButton;
        [SerializeField] private Button buildingButton;
        [SerializeField] private Button bigBuildingButton;
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
            //TODO KUBO
            moneyCount.text = 999999999.ToString();
            currentIncome = 5000;
            incomePerPopulation = 10;
            natureButton.onClick.AddListener(() => {
                Debug.Log("natureButton button 1 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingPondSO.cost) {
                    // Vector3(-11.2962799,4.38943624,-3.75619435)
                    ShowPreview(previewBuildingPondSO,null);
                }
            });
            buildingButton.onClick.AddListener(() => {
                Debug.Log("buildingButton button 2 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingVillageSO.cost) {
                    Dictionary<int, Dictionary<int, List<Vector3>>> villageWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
                    List<Vector3> list = new List<Vector3>();
                    villageWaypoints[1] = new Dictionary<int, List<Vector3>>();
                    list.Add(new Vector3(2.99f,3.683f,-1.05f));
                    list.Add(new Vector3(-0f,0f,-5f));
                    // list.Add(new Vector3(-11.2962799f,4.38943624f,-3.75619435f));
                    villageWaypoints[1][5] = list;
                    ShowPreview(previewBuildingVillageSO, villageWaypoints);
                }
            });
            bigBuildingButton.onClick.AddListener(() => {
                Debug.Log("bigBuildingButton button 3 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingPizzeriaSO.cost) {
                    ShowPreview(previewBuildingPizzeriaSO, null);
                }
            });

            hideBuildingUIButton.onClick.AddListener(() => {
                buildingUIToHide.SetActive(!buildingUIToHide.activeInHierarchy);
            });
            hideStatsUIButton.onClick.AddListener(() => { statsUIToHide.SetActive(!statsUIToHide.activeInHierarchy); });
            hideRestaurantUIButton.onClick.AddListener(() => {
                restaurantUIToHide.SetActive(!restaurantUIToHide.activeInHierarchy);
                gridUIToHide.SetActive(!gridUIToHide.activeInHierarchy);
            });

            cancelPreview.onClick.AddListener(() => {
                Debug.Log("cancelPreview button clicked");
                StopPreviewing();
            });


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

            // TODO kubo if out of range throws.. needs fixin
            EnableButtons();

        }

        private void EnableButtons() {
            if (int.Parse(moneyCount.text) >= previewBuildingVillageSO.cost) {
                buildingButton.interactable = true;
            }
            else {
                buildingButton.interactable = false;
            }

            if (int.Parse(moneyCount.text) >= previewBuildingPondSO.cost) {
                natureButton.interactable = true;
            }
            else {
                natureButton.interactable = false;
            }

            if (int.Parse(moneyCount.text) >= previewBuildingPizzeriaSO.cost) {
                bigBuildingButton.interactable = true;
            }
            else {
                bigBuildingButton.interactable = false;
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

        private void ShowPreview(PreviewBuildingSO previewBuildingSo, Dictionary<int, Dictionary<int, List<Vector3>>> waypoints) {
            PreviewBuilding(previewBuildingSo, waypoints);
        }

    }

}