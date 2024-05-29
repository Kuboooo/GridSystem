using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIManager : MonoBehaviour {
        public static event Action<PreviewBuildingSO, Dictionary<int, Dictionary<int, List<Vector3>>>>
            OnPreviewingBuilding;

        public static event Action<object, EventArgs> OnStopPreviewing;

        private static void PreviewBuilding(PreviewBuildingSO buildingSO,
            Dictionary<int, Dictionary<int, List<Vector3>>> waypoints) {
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
        private float currentIncome = 500f;
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
                    
                    Dictionary<int, Dictionary<int, List<Vector3>>> pondWaypoints =
                        new Dictionary<int, Dictionary<int, List<Vector3>>>();
                    List<Vector3> oneThreeList = new List<Vector3>();
                    List<Vector3> oneFiveList = new List<Vector3>();
                    List<Vector3> threeOneList = new List<Vector3>();
                    List<Vector3> fiveOneList = new List<Vector3>();
                    List<Vector3> threeFiveList = new List<Vector3>();
                    List<Vector3> fiveThreeList = new List<Vector3>();
                    
                    pondWaypoints[0] = new Dictionary<int, List<Vector3>>();
                    pondWaypoints[2] = new Dictionary<int, List<Vector3>>();
                    pondWaypoints[4] = new Dictionary<int, List<Vector3>>();
                    
                    pondWaypoints[0][4] = oneThreeList;
                    pondWaypoints[4][0] = threeOneList;
                    pondWaypoints[2][4] = oneFiveList;
                    pondWaypoints[4][2] = fiveOneList;
                    pondWaypoints[2][0] = threeFiveList;
                    pondWaypoints[0][2] = fiveThreeList;
                    
                    threeOneList.Add(new Vector3(4.9f,0,9f));
                    threeOneList.Add(new Vector3(1.3f,0,9.1f));
                    threeOneList.Add(new Vector3(-4f,0,8.6f));
                    threeOneList.Add(new Vector3(-8.6f,0,6.3f));
                    threeOneList.Add(new Vector3(-9.4f,0,1.4f));
                    
                    fiveOneList.Add(new Vector3(4.9f,0,9f));
                    fiveOneList.Add(new Vector3(7.4f,0,6.6f));
                    fiveOneList.Add(new Vector3(8.6f,0,3.5f));
                    fiveOneList.Add(new Vector3(9.4f,0,0f));
                    fiveOneList.Add(new Vector3(8.4f,0,-4.5f));
                    fiveOneList.Add(new Vector3(5.36f,0,-7.34f));
                    
                    oneThreeList.Add(new Vector3(-9.4f,0,1.4f));
                    oneThreeList.Add(new Vector3(-8.6f,0,6.3f));
                    oneThreeList.Add(new Vector3(-4f,0,8.6f));
                    oneThreeList.Add(new Vector3(1.3f,0,9.1f));
                    oneThreeList.Add(new Vector3(4.9f,0,9f));

                    oneFiveList.Add(new Vector3(5.36f,0,-7.34f));
                    oneFiveList.Add(new Vector3(8.4f,0,-4.5f));
                    oneFiveList.Add(new Vector3(9.4f,0,0f));
                    oneFiveList.Add(new Vector3(8.6f,0,3.5f));
                    oneFiveList.Add(new Vector3(7.4f,0,6.6f));
                    oneFiveList.Add(new Vector3(4.9f,0,9f));

                    fiveThreeList.Add(new Vector3(-9.4f,0,1.4f));
                    fiveThreeList.Add(new Vector3(-8.3f,0,-2.6f));
                    fiveThreeList.Add(new Vector3(-5.8f,0,-6f));
                    fiveThreeList.Add(new Vector3(-2.9f,0,-8.13f));
                    fiveThreeList.Add(new Vector3(1.18f,0,-9.15f));
                    fiveThreeList.Add(new Vector3(5.36f,0,-7.34f));

                    threeFiveList.Add(new Vector3(5.36f,0,-7.34f));
                    threeFiveList.Add(new Vector3(1.18f,0,-9.15f));
                    threeFiveList.Add(new Vector3(-2.9f,0,-8.13f));
                    threeFiveList.Add(new Vector3(-5.8f,0,-6f));
                    threeFiveList.Add(new Vector3(-8.3f,0,-2.6f));
                    threeFiveList.Add(new Vector3(-9.4f,0,1.4f));

                    ShowPreview(previewBuildingPondSO, pondWaypoints);
                }
            });
            buildingButton.onClick.AddListener(() => {
                Debug.Log("buildingButton button 2 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingVillageSO.cost) {
                    Dictionary<int, Dictionary<int, List<Vector3>>> villageWaypoints =
                        new Dictionary<int, Dictionary<int, List<Vector3>>>();
                    List<Vector3> list = new List<Vector3>();
                    villageWaypoints[2] = new Dictionary<int, List<Vector3>>();
                    villageWaypoints[4] = new Dictionary<int, List<Vector3>>();
                    list.Add(new Vector3(4.7f,0,-10.5f));
                    list.Add(new Vector3(3.6f,0,-6.66f));
                    list.Add(new Vector3(3.74f,0,-1.65f));
                    list.Add(new Vector3(4.87f,0,3.72f));
                    List<Vector3> fiveList = new List<Vector3>();
                    
                    fiveList.Add(new Vector3(4.7f,0,10.3f));
                    fiveList.Add(new Vector3(4f,0,4.8f));
                    fiveList.Add(new Vector3(4.6f,0,-4.63f));
                    fiveList.Add(new Vector3(5.76f,0,-8.5f));
                    
                    villageWaypoints[2][4] = list;
                    villageWaypoints[4][2] = fiveList;
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


            // populationCount.text = "5";
            // moneyCount.text = "50";
        }

        private void Update() {
            timeSinceLastAction += Time.deltaTime;
            if (timeSinceLastAction >= interval) {
                currentIncome = currentPopulation * incomePerPopulation * pizzaValue;
                // TODO KUBO REMOVE
                if (currentIncome + int.Parse(moneyCount.text) > 9999999) {
                    incomePerPopulation = 1;
                    currentIncome = 1000;
                    moneyCount.text = 5000.ToString();
                }

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

        private void ShowPreview(PreviewBuildingSO previewBuildingSo,
            Dictionary<int, Dictionary<int, List<Vector3>>> waypoints) {
            PreviewBuilding(previewBuildingSo, waypoints);
        }
    }
}