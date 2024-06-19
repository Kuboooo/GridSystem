using System;
using System.Collections.Generic;
using Grid;
using SOs;
using UnityEngine;
using UnityEngine.EventSystems;
using static Hex;

public class BuildingPlacer : MonoBehaviour {
    
    public static event Action<object, PreviewBuildingSO> OnBuildingBuilt;

    private const int MAX_ROTATION = 6;

    [SerializeField]
    private GameObject map;
    
    private int currentRotation;
    private PreviewBuildingSO previewBuildingSO;
    private MouseCoordinates mouseCoordinates;

    private void Start() {
        mouseCoordinates = MouseCoordinates.GetInstance();
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
        if (mouseCoordinates.GetMap().TryGetValue(hex, out GameObject hexObjectPart)) {
            Destroy(hexObjectPart);
            mouseCoordinates.GetMap().Remove(hex);
        }

        hex.GetHexProperties().worldPosition = buildingInstance.transform.position;
        hex.GetHexProperties().SetRotation(currentRotation);
        hex.GetHexProperties().SetBuildingType(previewBuildingSO.buildingType);
        hex.GetHexProperties().SetAOERange(previewBuildingSO.baseRange);
        Debug.Log("Adding hex: ");
        mouseCoordinates.GetMap().Add(hex, buildingInstance);
        mouseCoordinates.GetBuildingMap()[hex] = buildingInstance;
    }

    private void ProcessWaypointsAndRoads(int hexNumber, Hex hex) {
        hex.GetHexProperties().SetMultiHexDirection(hexNumber);
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

        hex.GetHexWaypoints().waypoints = waypoints;
        hex.GetHexWaypoints().SetWaypoints(previewBuildingSO.waypoints?[hexNumber], currentRotation);
        hex.AddConnections(newRoads);
    }

}