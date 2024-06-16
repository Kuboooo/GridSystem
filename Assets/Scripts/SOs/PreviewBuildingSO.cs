using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace SOs {

    [CreateAssetMenu(fileName = "PreviewBuildingSO", menuName = "ScriptableObjects/PreviewBuildingSO", order = 1)]
    public class PreviewBuildingSO : ScriptableObject {

        public GameObject prefabToPreview;
        public GameObject prefabToBuild;
        public int basePopulationGrowth;
        public int tileSize;
        public int cost;
        public BuildingType buildingType;
        public List<Roads> roads;
        public List<Dictionary<int, Dictionary<int, List<Vector3>>>> waypoints;
        public List<int> multiHexIndexPosition;

        public void InitializeWaypoints() {
            waypoints = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();

            switch (buildingType) {
                case BuildingType.Pond:
                    InitializePondWaypoints();
                    break;
                case BuildingType.Village:
                    InitializeVillageWaypoints();
                    break;
                case BuildingType.Pizzeria:
                    InitializePizzeriaWaypoints();
                    break;                
                case BuildingType.Hospital:
                    InitializePizzeriaWaypoints();
                    break;
                case BuildingType.PowerPlant:
                    InitializePowerPlantWaypoints();
                    break;
                // Add more cases for other building types
            }
        }

        private void InitializeVillageWaypoints() {
            List<Dictionary<int, Dictionary<int, List<Vector3>>>> villageWaypointsList =
                new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();
            Dictionary<int, Dictionary<int, List<Vector3>>> villageWaypoints =
                new Dictionary<int, Dictionary<int, List<Vector3>>>();
            List<Vector3> list = new List<Vector3>();
            villageWaypoints[2] = new Dictionary<int, List<Vector3>>();
            villageWaypoints[4] = new Dictionary<int, List<Vector3>>();
            list.Add(new Vector3(4.7f, 0, -10.5f));
            list.Add(new Vector3(3.6f, 0, -6.66f));
            list.Add(new Vector3(3.74f, 0, -1.65f));
            list.Add(new Vector3(4.87f, 0, 3.72f));
            List<Vector3> fiveList = new List<Vector3>();

            fiveList.Add(new Vector3(4.7f, 0, 10.3f));
            fiveList.Add(new Vector3(4f, 0, 4.8f));
            fiveList.Add(new Vector3(4.6f, 0, -4.63f));
            fiveList.Add(new Vector3(5.76f, 0, -8.5f));

            villageWaypoints[2][4] = list;
            villageWaypoints[4][2] = fiveList;
            villageWaypointsList.Add(villageWaypoints);
            waypoints = villageWaypointsList;
        }


        private void InitializePondWaypoints() {

            List<Dictionary<int, Dictionary<int, List<Vector3>>>> pondWaypointsList =
                new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();
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

            threeOneList.Add(new Vector3(4.9f, 0, 9f));
            threeOneList.Add(new Vector3(1.3f, 0, 9.1f));
            threeOneList.Add(new Vector3(-4f, 0, 8.6f));
            threeOneList.Add(new Vector3(-8.6f, 0, 6.3f));
            threeOneList.Add(new Vector3(-9.4f, 0, 1.4f));

            fiveOneList.Add(new Vector3(4.9f, 0, 9f));
            fiveOneList.Add(new Vector3(7.4f, 0, 6.6f));
            fiveOneList.Add(new Vector3(8.6f, 0, 3.5f));
            fiveOneList.Add(new Vector3(9.4f, 0, 0f));
            fiveOneList.Add(new Vector3(8.4f, 0, -4.5f));
            fiveOneList.Add(new Vector3(5.36f, 0, -7.34f));

            oneThreeList.Add(new Vector3(-9.4f, 0, 1.4f));
            oneThreeList.Add(new Vector3(-8.6f, 0, 6.3f));
            oneThreeList.Add(new Vector3(-4f, 0, 8.6f));
            oneThreeList.Add(new Vector3(1.3f, 0, 9.1f));
            oneThreeList.Add(new Vector3(4.9f, 0, 9f));

            oneFiveList.Add(new Vector3(5.36f, 0, -7.34f));
            oneFiveList.Add(new Vector3(8.4f, 0, -4.5f));
            oneFiveList.Add(new Vector3(9.4f, 0, 0f));
            oneFiveList.Add(new Vector3(8.6f, 0, 3.5f));
            oneFiveList.Add(new Vector3(7.4f, 0, 6.6f));
            oneFiveList.Add(new Vector3(4.9f, 0, 9f));

            fiveThreeList.Add(new Vector3(-9.4f, 0, 1.4f));
            fiveThreeList.Add(new Vector3(-8.3f, 0, -2.6f));
            fiveThreeList.Add(new Vector3(-5.8f, 0, -6f));
            fiveThreeList.Add(new Vector3(-2.9f, 0, -8.13f));
            fiveThreeList.Add(new Vector3(1.18f, 0, -9.15f));
            fiveThreeList.Add(new Vector3(5.36f, 0, -7.34f));

            threeFiveList.Add(new Vector3(5.36f, 0, -7.34f));
            threeFiveList.Add(new Vector3(1.18f, 0, -9.15f));
            threeFiveList.Add(new Vector3(-2.9f, 0, -8.13f));
            threeFiveList.Add(new Vector3(-5.8f, 0, -6f));
            threeFiveList.Add(new Vector3(-8.3f, 0, -2.6f));
            threeFiveList.Add(new Vector3(-9.4f, 0, 1.4f));

            pondWaypointsList.Add(pondWaypoints);
            waypoints = pondWaypointsList;
        }

        private void InitializePizzeriaWaypoints() {
            var pizzeriaWaypointsList = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();
            var mainPizzeriaWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();

            //right
            var rightPizzeriaWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            List<Vector3> rightZeroThreeWaypoints = new List<Vector3>();
            List<Vector3> rightThreeZeroWaypoints = new List<Vector3>();
            List<Vector3> rightZeroFourWaypoints = new List<Vector3>();
            List<Vector3> rightFourZeroWaypoints = new List<Vector3>();
            List<Vector3> rightThreeFourWaypoints = new List<Vector3>();
            List<Vector3> rightFourThreeWaypoints = new List<Vector3>();
        
            // Right
            rightPizzeriaWaypoints[0] =  new Dictionary<int, List<Vector3>>();
            rightPizzeriaWaypoints[3] =  new Dictionary<int, List<Vector3>>();
            rightPizzeriaWaypoints[4] =  new Dictionary<int, List<Vector3>>();

            // Zero three
            rightZeroThreeWaypoints.Add(new Vector3(-33.32f, 0, 0f));
            rightZeroThreeWaypoints.Add(new Vector3(-29.5f, 0, 4.4f));
            rightZeroThreeWaypoints.Add(new Vector3(-25.66f, 0, -0.47f));
            rightZeroThreeWaypoints.Add(new Vector3(-19.79f, 0, -1.77f));
            rightPizzeriaWaypoints[0][3] =  rightZeroThreeWaypoints;
        
            rightThreeZeroWaypoints.Add(new Vector3(-19.79f, 0, -1.77f));
            rightThreeZeroWaypoints.Add(new Vector3(-25.66f, 0, -0.47f));
            rightThreeZeroWaypoints.Add(new Vector3(-29.5f, 0, 4.4f));
            rightThreeZeroWaypoints.Add(new Vector3(-33.32f, 0, 0f));
            rightPizzeriaWaypoints[3][0] =  rightThreeZeroWaypoints;
                
            // Zero Four
            rightZeroFourWaypoints.Add(new Vector3(-33.57f, 0f, -0.26f));
            rightZeroFourWaypoints.Add(new Vector3(-29.59f, 0f, 2.67f));
            rightZeroFourWaypoints.Add(new Vector3(-23.72f, 0f, 9.23f));
            rightPizzeriaWaypoints[0][4] =  rightZeroFourWaypoints;

            // Four Zero
            rightFourZeroWaypoints.Add(new Vector3(-23.72f, 0f, 9.23f));
            rightFourZeroWaypoints.Add(new Vector3(-29.59f, 0f, 2.67f));
            rightFourZeroWaypoints.Add(new Vector3(-33.57f, 0f, -0.26f));
            rightPizzeriaWaypoints[4][0] =  rightFourZeroWaypoints;
                
            // Three Four
            rightThreeFourWaypoints.Add(new Vector3(-14.55f, 0f, -2.95f));
            rightThreeFourWaypoints.Add(new Vector3(-20.67f, 0f, -2.09f));
            rightThreeFourWaypoints.Add(new Vector3(-25.59f, 0f, -0.44f));
            rightThreeFourWaypoints.Add(new Vector3(-29.49f, 0f, 2.61f));
            rightThreeFourWaypoints.Add(new Vector3(-27.27f, 0f, 4.68f));
            rightThreeFourWaypoints.Add(new Vector3(-23.91f, 0f, 8.51f));
            rightThreeFourWaypoints.Add(new Vector3(-22.59f, 0f, 10.52f));
            rightPizzeriaWaypoints[3][4] =  rightThreeFourWaypoints;

            // Four Three
            rightFourThreeWaypoints.Add(new Vector3(-22.59f, 0f, 10.52f));
            rightFourThreeWaypoints.Add(new Vector3(-23.91f, 0f, 8.51f));
            rightFourThreeWaypoints.Add(new Vector3(-27.27f, 0f, 4.68f));
            rightFourThreeWaypoints.Add(new Vector3(-29.49f, 0f, 2.61f));
            rightFourThreeWaypoints.Add(new Vector3(-25.59f, 0f, -0.44f));
            rightFourThreeWaypoints.Add(new Vector3(-20.67f, 0f, -2.09f));
            rightFourThreeWaypoints.Add(new Vector3(-14.55f, 0f, -2.95f));
            rightPizzeriaWaypoints[4][3] =  rightFourThreeWaypoints;

            // bottom
            var bottomPizzeriaWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            List<Vector3> bottomOneFiveWaypoints = new List<Vector3>();
            List<Vector3> bottomFiveOneWaypoints = new List<Vector3>();
        
            bottomPizzeriaWaypoints[1] =  new Dictionary<int, List<Vector3>>();
            bottomPizzeriaWaypoints[5] =  new Dictionary<int, List<Vector3>>();
        
            // One Five
            bottomOneFiveWaypoints.Add(new Vector3(-21.28f, 0, 12.61f));
            bottomOneFiveWaypoints.Add(new Vector3(-18.69f, 0, 17.94f));
            bottomOneFiveWaypoints.Add(new Vector3(-16.54f, 0, 25.57f));
            bottomOneFiveWaypoints.Add(new Vector3(-16.28f, 0, 30.5f));
            bottomOneFiveWaypoints.Add(new Vector3(-18.05f, 0, 33.49f));
            bottomPizzeriaWaypoints[1][5] =  bottomOneFiveWaypoints;
        
            // Five One
            bottomFiveOneWaypoints.Add(new Vector3(-18.05f, 0, 33.49f));
            bottomFiveOneWaypoints.Add(new Vector3(-16.28f, 0, 30.5f));
            bottomFiveOneWaypoints.Add(new Vector3(-16.54f, 0, 25.57f));
            bottomFiveOneWaypoints.Add(new Vector3(-18.69f, 0, 17.94f));
            bottomFiveOneWaypoints.Add(new Vector3(-21.28f, 0, 12.61f));
            bottomPizzeriaWaypoints[5][1] =  bottomFiveOneWaypoints;
        
            pizzeriaWaypointsList.Add(mainPizzeriaWaypoints);
            pizzeriaWaypointsList.Add(rightPizzeriaWaypoints);
            pizzeriaWaypointsList.Add(bottomPizzeriaWaypoints);
            waypoints = pizzeriaWaypointsList;
        }

        private void InitializeHospitalWaypoints() {
            // TODO KUBO add waypoints
        }

        private void InitializePowerPlantWaypoints() {
            var waypointsList = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();
            var mainWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var rightWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var topWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();

            waypointsList.Add(mainWaypoints);
            waypointsList.Add(rightWaypoints);
            waypointsList.Add(topWaypoints);
            
            waypoints = waypointsList;
        }

    }

}