using System;
using System.IO;
using SOs;
using StructureBuilding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public class UIManager : MonoBehaviour {

        public static UIManager instance;
        public static event Action<PreviewBuildingSO> OnPreviewingBuilding;

        public static event Action<object, EventArgs> OnStopPreviewing;
        public static event Action<object, EventArgs> OnSaveGameButtonPressed;
        public static event Action<object, EventArgs> OnLoadGameButtonPressed;

        private static void PreviewBuilding(PreviewBuildingSO buildingSO) {
            OnPreviewingBuilding?.Invoke(buildingSO);
        }

        private static void StopPreviewing() {
            OnStopPreviewing?.Invoke(null, EventArgs.Empty);
        }

        private static void SaveGame() {
            OnSaveGameButtonPressed?.Invoke(null, EventArgs.Empty);
        }

        private static void LoadGame() {
            OnLoadGameButtonPressed?.Invoke(null, EventArgs.Empty);
        }

        [SerializeField] private Button saveGameStateButton;
        [SerializeField] private Button loadGameStateButton;

        [SerializeField] private Button natureButton;
        [SerializeField] private Button buildingButton;
        [SerializeField] private Button bigBuildingButton;
        [SerializeField] private Button roadMaintenanceButton;
        [SerializeField] private Button powerPlantBuildingButton;
        [SerializeField] private Button hospitalButton;
        [SerializeField] private Button cancelPreview;
        
        [SerializeField] private TextMeshProUGUI populationCount;
        [SerializeField] private TextMeshProUGUI moneyCount;

        // TODO KUBO pull these somehow automatically out of some SOlist
        [SerializeField] private PreviewBuildingSO previewBuildingVillageSO;
        [SerializeField] private PreviewBuildingSO previewBuildingPondSO;
        [SerializeField] private PreviewBuildingSO previewBuildingPizzeriaSO;
        [SerializeField] private PreviewBuildingSO previewBuildingHospitalSO;
        [SerializeField] private PreviewBuildingSO previewRoadMaintenanceButtonSO;
        [SerializeField] private PreviewBuildingSO powerPlantBuildingSO;

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

        
        private void Awake() {
            instance = this;
        }

        private void Start() {

            previewBuildingVillageSO.InitializeSO();
            previewBuildingPondSO.InitializeSO();
            previewBuildingPizzeriaSO.InitializeSO();
            previewBuildingHospitalSO.InitializeSO();
            powerPlantBuildingSO.InitializeSO();

            //TODO KUBO
            moneyCount.text = 999999999.ToString();
            currentIncome = 5000;
            incomePerPopulation = 10;

            saveGameStateButton.onClick.AddListener(() => {
                Debug.Log("saveGameStateButton button clicked");
                SaveGame();
            });

            loadGameStateButton.onClick.AddListener(() => {
                Debug.Log("loadGameStateButton button clicked");
                LoadGame();
            });
            
            natureButton.onClick.AddListener(() => {
                Debug.Log("natureButton button 1 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingPondSO.cost) {

                    ShowPreview(previewBuildingPondSO);
                }
            });
            buildingButton.onClick.AddListener(() => {
                Debug.Log("buildingButton button 2 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingVillageSO.cost) {

                    ShowPreview(previewBuildingVillageSO);
                }
            });
            hospitalButton.onClick.AddListener(() => {
                Debug.Log("hospital button 4 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingHospitalSO.cost) {
                    ShowPreview(previewBuildingHospitalSO);
                }
            });   
            roadMaintenanceButton.onClick.AddListener(() => {
                Debug.Log("RoadMaintenance button 4 clicked");
                // if (int.Parse(moneyCount.text) >= previewRoadMaintenanceButtonSO.cost) {
                //     ShowPreview(previewRoadMaintenanceButtonSO);
                // }
            });
            powerPlantBuildingButton.onClick.AddListener(() => {
                Debug.Log("PowerPlant button clicked");
                if (int.Parse(moneyCount.text) >= powerPlantBuildingSO.cost) {
                    ShowPreview(powerPlantBuildingSO);
                }
            });
            bigBuildingButton.onClick.AddListener(() => {
                Debug.Log("bigBuildingButton button 3 clicked");
                if (int.Parse(moneyCount.text) >= previewBuildingPizzeriaSO.cost) {
                    ShowPreview(previewBuildingPizzeriaSO);
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
            BuildingPlacer.OnBuildingBuilt += OnBuildingBuilt;
            RestaurantUIManager.OnToppingsSelected += OnToppingsSelected;
        }

        private void OnDisable() {
            BuildingPlacer.OnBuildingBuilt -= OnBuildingBuilt;
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

        public void SaveStats(BinaryWriter writer) {
            writer.Write(int.Parse(moneyCount.text));
            writer.Write(currentIncome);
            writer.Write(currentPopulation);
            writer.Write(incomePerPopulation);
            writer.Write(pizzaValue);

            writer.Write(int.Parse(populationCount.text));

            writer.Write(timeSinceLastAction);
            writer.Write(interval);
        }

        public void LoadStats(BinaryReader reader) {
            moneyCount.text = reader.ReadInt32().ToString();
            currentIncome = reader.ReadInt32();
            currentPopulation = reader.ReadInt32();
            incomePerPopulation = reader.ReadInt32();
            pizzaValue = reader.ReadInt32();

            populationCount.text = reader.ReadInt32().ToString();

            timeSinceLastAction = reader.ReadSingle();
            interval = reader.ReadSingle();
        }
    }

}