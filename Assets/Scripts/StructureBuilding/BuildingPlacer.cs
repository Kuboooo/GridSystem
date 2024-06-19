using System;
using System.Collections.Generic;
using SOs;
using UnityEngine;
using UnityEngine.EventSystems;
using static RedblockGrid;

public class BuildingPlacer : MonoBehaviour {
    
    public static event Action<object, PreviewBuildingSO> OnBuildingBuilt;

    private const int MAX_ROTATION = 6;

    [SerializeField]
    private GameObject map;
    
    private int currentRotation;
    private PreviewBuildingSO previewBuildingSO;
    private HexGridGenerator hexGridGenerator;
    private Dictionary<Hex, GameObject> hexMap;
    private Dictionary<Hex, GameObject> buildingsMap = new();

    private void Start() {
        hexGridGenerator = HexGridGenerator.GetInstance();
        hexMap = hexGridGenerator.GetHexMap();
    }

    public void PlaceBuilding(List<Hex> hexesToBuild, GameObject buildingToBuild, GameObject previewInstance, int currentRotationParam, PreviewBuildingSO previewBuildingSOparam) {
        previewBuildingSO = previewBuildingSOparam;
        currentRotation = currentRotationParam;
        Vector3 buildPosition =
            new Vector3(previewInstance.transform.position.x, 0f, previewInstance.transform.position.z);
        GameObject buildingInstance = Instantiate(buildingToBuild, buildPosition, previewInstance.transform.rotation);
        buildingInstance.transform.parent = map.transform;

        int hexNumber = 0;
        foreach (var hex in hexesToBuild) {
            ProcessHex(hex, buildingInstance);
            ProcessWaypointsAndRoads(hexNumber, hex);
            hexNumber++;
        }

        FinalizeBuildingPlacement();
    }

    private void FinalizeBuildingPlacement() {
        OnBuildingBuilt?.Invoke(null, previewBuildingSO);
    }

    private void ProcessHex(Hex hex, GameObject buildingInstance) {
        if (hexMap.TryGetValue(hex, out GameObject hexObjectPart)) {
            Destroy(hexObjectPart);
            hexMap.Remove(hex);
        }

        hex.worldPosition = buildingInstance.transform.position;
        hex.SetRotation(currentRotation);
        hex.SetBuildingType(previewBuildingSO.buildingType);
        hex.SetAOERange(previewBuildingSO.baseRange);
        hexMap.Add(hex, buildingInstance);
        buildingsMap[hex] = buildingInstance;
    }

    private void ProcessWaypointsAndRoads(int hexNumber, Hex hex) {
        hex.SetMultiHexDirection(hexNumber);
        if (previewBuildingSO.roads == null) return;
        int[] newRoads = new int[previewBuildingSO.roads[hexNumber].roadArray.Length];
        for (int i = 0; i < newRoads.Length; i++) {
            newRoads[i] = ((previewBuildingSO.roads[hexNumber].roadArray[i] + currentRotation) % MAX_ROTATION);
        }

        Dictionary<int, Dictionary<int, List<Vector3>>> waypoints =
            new Dictionary<int, Dictionary<int, List<Vector3>>>();
        for (int i = 0; i < 6; i++) {
            waypoints[i] = new Dictionary<int, List<Vector3>>();
            for (int j = 0; j < 6; j++) {
                waypoints[i][j] = new List<Vector3>();
            }
        }

        hex.waypoints = waypoints;
        hex.SetWaypoints(previewBuildingSO.waypoints?[hexNumber], currentRotation);
        hex.AddConnections(newRoads);
    }

}