using System;
using GameObjects;
using UnityEngine;
using static Hex;

public class AreaIndicatorManager : MonoBehaviour {
    public GameObject circlePrefab; // Prefab with the CircleMesh component
    private GameObject currentCircle;

    public GameObject spherePrefab; // Prefab with the SphereMesh component
    private GameObject currentSphere;

    private MouseCoordinates mouseCoordinates;
    private Camera mainCamera;

    void Start() {
        mouseCoordinates = MouseCoordinates.GetInstance();
        mainCamera = Camera.main;
    }

    private void Update() {
        if (Input.GetMouseButton(1)) {
            HighlightArea();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Destroy(currentCircle);
            Destroy(currentSphere);
            HexHighlighter.UnhighlightHexesRange();
        }
    }

    private void HighlightArea() {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = GetMouseRay(mousePos);
        Hex hex;
        float furthestX = 0;
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        mouseCoordinates.GetHexFromRay(hit, out hex);
        hex = mouseCoordinates.GetKeyFromMap(hex);
        Destroy(currentCircle);
        Destroy(currentSphere);
        if (hex is null || hex.GetHexProperties().GetAOERange() == 0) return;
        // FOR multihex buildings to select the main/parent and calculate and show area around it
        if (hex.GetHexProperties().GetMainCoordinates() is not null) {
            hex = mouseCoordinates.GetKeyFromMap(hex.GetHexProperties().GetMainCoordinates());
        }
        var allInRange = HexRangeFinder.GetAllBuildingsInRange(hex, hex.GetHexProperties().GetAOERange());
        foreach (var inRange in allInRange) {
            if (!(Math.Abs(hex.GetHexProperties().worldPosition.x - inRange.GetHexProperties().worldPosition.x) > furthestX)) continue;
            furthestX = Math.Abs(hex.GetHexProperties().worldPosition.x - inRange.GetHexProperties().worldPosition.x);
        }

        SpawnSphere(hex, furthestX);
        SpawnCircle(hex, furthestX);
        HexHighlighter.HighlightMultipleHexes(hex, mouseCoordinates.GetMap(),
            HexRangeFinder.GetOnlyConnectedInRange(hex, hex.GetHexProperties().GetAOERange()));
    }

    private void SpawnSphere(Hex hex, float furthestX) {
        Destroy(currentSphere);
        currentSphere = Instantiate(spherePrefab,
            new Vector3(hex.GetHexProperties().worldPosition.x, hex.GetHexProperties().worldPosition.y, hex.GetHexProperties().worldPosition.z), Quaternion.identity);
        var sphereMesh = currentSphere.GetComponent<SphereMeshGenerator>();

        if (sphereMesh != null) {
            sphereMesh.GenerateMesh(furthestX);
        }
    }

    private void SpawnCircle(Hex hex, float furthestX) {
        Destroy(currentCircle);
        currentCircle = Instantiate(circlePrefab,
            new Vector3(hex.GetHexProperties().worldPosition.x, hex.GetHexProperties().worldPosition.y, hex.GetHexProperties().worldPosition.z), Quaternion.identity);
        CircleMeshGenerator meshGenerator = currentCircle.GetComponent<CircleMeshGenerator>();

        if (meshGenerator != null) {
            meshGenerator.GenerateMesh(furthestX);
        }
    }

    private Ray GetMouseRay(Vector3 mousePos) {
        return mainCamera.ScreenPointToRay(mousePos);
    }
}