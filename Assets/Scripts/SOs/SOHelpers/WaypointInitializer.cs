using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace SOs.SOHelpers {
    public class WaypointInitializer {
        public List<Dictionary<int, Dictionary<int, List<Vector3>>>> InitializeWaypoints(BuildingType buildingType) {
            switch (buildingType) {
                case BuildingType.Pond:
                    return InitializePondWaypoints();
                case BuildingType.Village:
                return InitializeVillageWaypoints();
                case BuildingType.Pizzeria:
                    return InitializePizzeriaWaypoints();
                case BuildingType.Hospital:
                return InitializeHospitalWaypoints();
                case BuildingType.PowerPlant:
                    return InitializePowerPlantWaypoints();
                default:
                    return new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();
            }
        }

        private  List<Dictionary<int, Dictionary<int, List<Vector3>>>>  InitializeVillageWaypoints() {
            var villageWaypointsList = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();

            var villageWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();

            // Village Waypoints
            AddWaypoints(villageWaypoints, 2, 4,
                new Vector3(4.7f, 0, -10.5f),
                new Vector3(3.6f, 0, -6.66f),
                new Vector3(3.74f, 0, -1.65f),
                new Vector3(4.87f, 0, 3.72f));

            AddWaypoints(villageWaypoints, 4, 2,
                new Vector3(4.7f, 0, 10.3f),
                new Vector3(4f, 0, 4.8f),
                new Vector3(4.6f, 0, -4.63f),
                new Vector3(5.76f, 0, -8.5f));

            villageWaypointsList.Add(villageWaypoints);

            return villageWaypointsList;
        }

        private List<Dictionary<int, Dictionary<int, List<Vector3>>>> InitializePizzeriaWaypoints() {
            var pizzeriaWaypointsList = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();

            var mainPizzeriaWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var rightPizzeriaWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var bottomPizzeriaWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();

            AddWaypoints(rightPizzeriaWaypoints, 0, 3,
                new Vector3(-33.32f, 0, 0f),
                new Vector3(-29.5f, 0, 4.4f),
                new Vector3(-25.66f, 0, -0.47f),
                new Vector3(-19.79f, 0, -1.77f));

            AddWaypoints(rightPizzeriaWaypoints, 3, 0,
                new Vector3(-19.79f, 0, -1.77f),
                new Vector3(-25.66f, 0, -0.47f),
                new Vector3(-29.5f, 0, 4.4f),
                new Vector3(-33.32f, 0, 0f));

            AddWaypoints(rightPizzeriaWaypoints, 0, 4,
                new Vector3(-33.57f, 0f, -0.26f),
                new Vector3(-29.59f, 0f, 2.67f),
                new Vector3(-23.72f, 0f, 9.23f));

            AddWaypoints(rightPizzeriaWaypoints, 4, 0,
                new Vector3(-23.72f, 0f, 9.23f),
                new Vector3(-29.59f, 0f, 2.67f),
                new Vector3(-33.57f, 0f, -0.26f));

            AddWaypoints(rightPizzeriaWaypoints, 3, 4,
                new Vector3(-14.55f, 0f, -2.95f),
                new Vector3(-20.67f, 0f, -2.09f),
                new Vector3(-25.59f, 0f, -0.44f),
                new Vector3(-29.49f, 0f, 2.61f),
                new Vector3(-27.27f, 0f, 4.68f),
                new Vector3(-23.91f, 0f, 8.51f),
                new Vector3(-22.59f, 0f, 10.52f));

            AddWaypoints(rightPizzeriaWaypoints, 4, 3,
                new Vector3(-22.59f, 0f, 10.52f),
                new Vector3(-23.91f, 0f, 8.51f),
                new Vector3(-27.27f, 0f, 4.68f),
                new Vector3(-29.49f, 0f, 2.61f),
                new Vector3(-25.59f, 0f, -0.44f),
                new Vector3(-20.67f, 0f, -2.09f),
                new Vector3(-14.55f, 0f, -2.95f));

            AddWaypoints(bottomPizzeriaWaypoints, 1, 5,
                new Vector3(-21.28f, 0, 12.61f),
                new Vector3(-18.69f, 0, 17.94f),
                new Vector3(-16.54f, 0, 25.57f),
                new Vector3(-16.28f, 0, 30.5f),
                new Vector3(-18.05f, 0, 33.49f));

            AddWaypoints(bottomPizzeriaWaypoints, 5, 1,
                new Vector3(-18.05f, 0, 33.49f),
                new Vector3(-16.28f, 0, 30.5f),
                new Vector3(-16.54f, 0, 25.57f),
                new Vector3(-18.69f, 0, 17.94f),
                new Vector3(-21.28f, 0, 12.61f));

            AddWaypoints(mainPizzeriaWaypoints, 0, 0,
                new Vector3(-18.05f, 0, 33.49f));

            pizzeriaWaypointsList.Add(mainPizzeriaWaypoints);
            pizzeriaWaypointsList.Add(rightPizzeriaWaypoints);
            pizzeriaWaypointsList.Add(bottomPizzeriaWaypoints);
            return pizzeriaWaypointsList;
        }

        private List<Dictionary<int, Dictionary<int, List<Vector3>>>> InitializePondWaypoints() {
            var pondWaypointsList = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();


            var pondWaypointsMain = new Dictionary<int, Dictionary<int, List<Vector3>>>();

            AddWaypoints(pondWaypointsMain, 0, 4,
                new Vector3(-9.4f, 0, 1.4f),
                new Vector3(-8.6f, 0, 6.3f),
                new Vector3(-4f, 0, 8.6f),
                new Vector3(1.3f, 0, 9.1f),
                new Vector3(4.9f, 0, 9f));

            AddWaypoints(pondWaypointsMain, 4, 0,
                new Vector3(4.9f, 0, 9f),
                new Vector3(1.3f, 0, 9.1f),
                new Vector3(-4f, 0, 8.6f),
                new Vector3(-8.6f, 0, 6.3f),
                new Vector3(-9.4f, 0, 1.4f));

            AddWaypoints(pondWaypointsMain, 2, 4,
                new Vector3(5.36f, 0, -7.34f),
                new Vector3(8.4f, 0, -4.5f),
                new Vector3(9.4f, 0, 0f),
                new Vector3(8.6f, 0, 3.5f),
                new Vector3(7.4f, 0, 6.6f),
                new Vector3(4.9f, 0, 9f));

            AddWaypoints(pondWaypointsMain, 4, 2,
                new Vector3(4.9f, 0, 9f),
                new Vector3(7.4f, 0, 6.6f),
                new Vector3(8.6f, 0, 3.5f),
                new Vector3(9.4f, 0, 0f),
                new Vector3(8.4f, 0, -4.5f),
                new Vector3(5.36f, 0, -7.34f));

            AddWaypoints(pondWaypointsMain, 2, 0,
                new Vector3(5.36f, 0, -7.34f),
                new Vector3(1.18f, 0, -9.15f),
                new Vector3(-2.9f, 0, -8.13f),
                new Vector3(-5.8f, 0, -6f),
                new Vector3(-8.3f, 0, -2.6f),
                new Vector3(-9.4f, 0, 1.4f));

            AddWaypoints(pondWaypointsMain, 0, 2,
                new Vector3(-9.4f, 0, 1.4f),
                new Vector3(-8.3f, 0, -2.6f),
                new Vector3(-5.8f, 0, -6f),
                new Vector3(-2.9f, 0, -8.13f),
                new Vector3(1.18f, 0, -9.15f),
                new Vector3(5.36f, 0, -7.34f));

            pondWaypointsList.Add(pondWaypointsMain);
            return pondWaypointsList;
        }

        private List<Dictionary<int, Dictionary<int, List<Vector3>>>> InitializeHospitalWaypoints() {
            var hospitalWaypointsList = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();

            var mainHospitalWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var rightHospitalWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var topRightHospitalWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var rightRightHospitalWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var rightTopHospitalWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            
            hospitalWaypointsList.Add(mainHospitalWaypoints);
            hospitalWaypointsList.Add(rightHospitalWaypoints);
            hospitalWaypointsList.Add(topRightHospitalWaypoints);
            hospitalWaypointsList.Add(rightRightHospitalWaypoints);
            hospitalWaypointsList.Add(rightTopHospitalWaypoints);

            return hospitalWaypointsList;
        }

        private List<Dictionary<int, Dictionary<int, List<Vector3>>>> InitializePowerPlantWaypoints() {
            var powerPlantWaypoints = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();

            var mainPowerPlantWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var rightPowerPlantWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            var topRightPowerPlantWaypoints = new Dictionary<int, Dictionary<int, List<Vector3>>>();
            
            powerPlantWaypoints.Add(mainPowerPlantWaypoints);
            powerPlantWaypoints.Add(rightPowerPlantWaypoints);
            powerPlantWaypoints.Add(topRightPowerPlantWaypoints);

            return powerPlantWaypoints;
        }

        private void AddWaypoints(Dictionary<int, Dictionary<int, List<Vector3>>> waypointDict, int startKey,
            int endKey, params Vector3[] points) {
            if (!waypointDict.ContainsKey(startKey)) {
                waypointDict[startKey] = new Dictionary<int, List<Vector3>>();
            }

            waypointDict[startKey][endKey] = new List<Vector3>(points);
        }
    }
}