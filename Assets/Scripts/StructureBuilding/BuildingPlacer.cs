using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using SOs;
using UnityEngine;

namespace StructureBuilding {
    public class BuildingPlacer : MonoBehaviour {
        public static event Action<object, PreviewBuildingSO> OnBuildingBuilt;

        private const int MAX_ROTATION = 6;

        [SerializeField] private GameObject map;

        private int currentRotation;
        private PreviewBuildingSO previewBuildingSO;
        private MouseCoordinates mouseCoordinates;

        private void Start() {
            mouseCoordinates = MouseCoordinates.GetInstance();
        }

        public void PlaceBuilding(List<Hex> hexesToBuild, GameObject buildingToBuild, GameObject previewInstance,
            int currentRotationParam, PreviewBuildingSO previewBuildingSOparam) {
            previewBuildingSO = previewBuildingSOparam;
            currentRotation = currentRotationParam;
            var buildPosition =
                new Vector3(previewInstance.transform.position.x, 0f, previewInstance.transform.position.z);
            var buildingInstance = Instantiate(buildingToBuild, buildPosition, previewInstance.transform.rotation);
            buildingInstance.transform.parent = map.transform;

            int hexNumber = 0;
            foreach (var hex in hexesToBuild) {
                ProcessHex(hex, buildingInstance);
                ProcessWaypointsAndRoads(hexNumber, hex);

                hexNumber++;
            }

            AddToBuildingTypeMap(hexesToBuild.First());
            UpdateHexesAsync(hexesToBuild.First());

            FinalizeBuildingPlacement();
        }

        private void UpdateHexesAsync(Hex hex) {
            var connectedHexes = HexRangeFinder.GetAllConnectedHexes(hex);
            foreach (var connectedHex in connectedHexes) {
                connectedHex.UpdateConnectedHexesInRange(HexRangeFinder.GetAllConnectedByType(hex, BuildingType.Village));
            }
        }

        private void FinalizeBuildingPlacement() {
            OnBuildingBuilt?.Invoke(null, previewBuildingSO);
        }

        private void ProcessHex(Hex hex, GameObject buildingInstance) {
            if (mouseCoordinates.GetMap().TryGetValue(hex, out var hexObjectPart)) {
                Destroy(hexObjectPart);
                mouseCoordinates.GetMap().Remove(hex);
            }

            hex.GetHexProperties().worldPosition = buildingInstance.transform.position;
            hex.GetHexProperties().SetRotation(currentRotation);
            hex.GetHexProperties().SetBuildingType(previewBuildingSO.buildingType);
            hex.GetHexProperties().SetAOERange(previewBuildingSO.baseRange);
            mouseCoordinates.GetMap().Add(hex, buildingInstance);
            var building = new Building(previewBuildingSO.buildingType, buildingInstance, hex);
            mouseCoordinates.GetBuildingMap()[hex] = building;
        }

        private void AddToBuildingTypeMap(Hex hex) {
            switch (previewBuildingSO.buildingType) {
                case BuildingType.Pond:
                    mouseCoordinates.GetPondsMap()[hex] = hex;
                    break;
                case BuildingType.Pizzeria:
                    mouseCoordinates.GetPizzeriasMap()[hex] = hex;
                    break;

                case BuildingType.School:
                    mouseCoordinates.GetSchoolsMap()[hex] = hex;
                    break;

                case BuildingType.Hospital:
                    mouseCoordinates.GetHospitalsMap()[hex] = hex;
                    break;

                case BuildingType.PowerPlant:
                    mouseCoordinates.GetPowerPlantsMap()[hex] = hex;
                    break;

                case BuildingType.JobCenter:
                    mouseCoordinates.GetJobCentersMap()[hex] = hex;
                    break;
                case BuildingType.Village:
                    mouseCoordinates.GetVillageMap()[hex] = hex;
                    break;
                case BuildingType.Basic:
                    break;
                // Add other cases as needed
                case BuildingType.RoadMaintainance:
                default:
                    throw new ArgumentException("Unsupported building type");
            }
        }

        private void ProcessWaypointsAndRoads(int hexNumber, Hex hex) {
            hex.GetHexProperties().SetMultiHexDirection(hexNumber);
            if (previewBuildingSO.roads == null) return;
            var newRoads = new int[previewBuildingSO.roads[hexNumber].roadArray.Length];
            for (var i = 0; i < newRoads.Length; i++) {
                newRoads[i] = ((previewBuildingSO.roads[hexNumber].roadArray[i] + currentRotation) % MAX_ROTATION);
            }

            Dictionary<int, Dictionary<int, List<Vector3>>> waypoints =
                new Dictionary<int, Dictionary<int, List<Vector3>>>();
            for (var i = 0; i < 6; i++) {
                waypoints[i] = new Dictionary<int, List<Vector3>>();
                for (var j = 0; j < 6; j++) {
                    waypoints[i][j] = new List<Vector3>();
                }
            }

            hex.GetHexWaypoints().waypoints = waypoints;
            hex.GetHexWaypoints().SetWaypoints(previewBuildingSO.waypoints?[hexNumber], currentRotation);
            hex.AddConnections(newRoads);
        }
    }
}