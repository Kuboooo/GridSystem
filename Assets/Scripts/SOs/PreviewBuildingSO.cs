using System.Collections.Generic;
using Enums;
using SOs.SOHelpers;
using UnityEngine;
using static Hex;

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
        public List<Hex> multiHexPositionDirectionList;
        public int baseRange;

        public void InitializeSO() {
            waypoints = new List<Dictionary<int, Dictionary<int, List<Vector3>>>>();
            WaypointInitializer waypointInitializer = new WaypointInitializer();
            waypoints = waypointInitializer.InitializeWaypoints(buildingType);
            if (buildingType == BuildingType.Hospital) {
                MultiHexPositionInitializer multiHexPositionInitializer = new MultiHexPositionInitializer();
                multiHexPositionDirectionList = multiHexPositionInitializer.InitializeHospitalMultiHexPositionDirectionList();
            }
        }
    }
}