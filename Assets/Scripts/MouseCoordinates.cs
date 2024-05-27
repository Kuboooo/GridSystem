using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using static RedblockGrid;

public class MouseCoordinates : MonoBehaviour {

    public static event Action<object, PreviewBuildingSO> OnBuildingBuilt;
    public Material material;
    private Material backupMaterial;

    [SerializeField] private HexHighlighter hexHighlighter;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject map;
    private HexGridGenerator hexGridGenerator;
    private GameObject previous;
    private Dictionary<Hex, GameObject> hexMap;

    private Dictionary<Hex, GameObject> buildingsMap = new();

    private Layout layout;

    private HashSet<GameObject> previewInstances = new();
    private GameObject previewInstance;
    private GameObject buildingToBuild;
    private bool previewing;
    private PreviewBuildingSO previewBuildingSO;
    private int currentRotation; // Track the current rotation in multiples of 60 degrees
    private int initialRotation = 0; // Track the current rotation in multiples of 60 degrees

    [SerializeField] private GameObject previewContainer;
    private static MouseCoordinates instance;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void OnEnable() {

        UIManager.OnPreviewingBuilding += OnPreviewingBuilding;
        UIManager.OnStopPreviewing += OnStopPreviewing;
    }

    private void OnDisable() {
        UIManager.OnPreviewingBuilding -= OnPreviewingBuilding;
        UIManager.OnStopPreviewing -= OnStopPreviewing;
    }

    private void Awake() {
        instance = this;
    }

    private void OnStopPreviewing(object arg1, EventArgs arg2) {
        previewing = false;
        DestroyPreviewInstance();
    }

    private void OnPreviewingBuilding(PreviewBuildingSO previewBuildingSo) {
        previewing = true;
        DestroyPreviewInstance();
        previewBuildingSO = previewBuildingSo;
        previewInstance = Instantiate(previewBuildingSo.prefabToPreview, previewContainer.transform);
        buildingToBuild = previewBuildingSo.prefabToBuild;
        previewInstance.transform.localPosition = Vector3.zero;
        previewInstance.transform.localRotation = Quaternion.identity;
        previewInstance.transform.localScale = Vector3.one;
        previewInstances.Add(previewInstance);
    }

    private void Start() {
        hexGridGenerator = HexGridGenerator.GetInstance();
        hexMap = hexGridGenerator.GetHexMap();
        layout = hexGridGenerator.GetLayout();
        hexHighlighter = GetComponent<HexHighlighter>();
    }

    void Update() {
        HighlightTileOnMouseHover();
        ShowPreview();

        //DrawConnections();
    }

    private void ShowPreview() {
        HandleEscapeKey();
        if (!previewing) {
            DestroyPreviewInstance();
            return;
        }

        HandleRotation();

        RaycastHit hit;
        if (TryGetRaycastHit(out hit)) {
            ShowPreviewOnRaycastHit(hit, out List<Hex> hexesToBuild);
            
            if (CanBuild(hexesToBuild)) {
                UpdatePreviewColor(true);
                HandleBuildingPlacement(hexesToBuild);
            }
            else {
                UpdatePreviewColor(false);
            }
        }
    }

    private void HandleEscapeKey() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            previewing = false;
            DestroyPreviewInstance();
        }
    }

    private void HandleRotation() {
        if (Input.GetKeyDown(KeyCode.X)) {
            RotatePreviewInstance();
        }
    }

    private void RotatePreviewInstance() {
        previewInstance.transform.rotation *= Quaternion.Euler(0, 60, 0);
        currentRotation = (currentRotation + 1) % 6;
    }

    private bool TryGetRaycastHit(out RaycastHit hit) {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        return Physics.Raycast(ray, out hit);
    }

    private void ShowPreviewOnRaycastHit(RaycastHit hit, out List<Hex> hexesToBuild) {
        hexesToBuild = new List<Hex>();
        GameObject hexObject = GetHexFromRay(hit, out Hex hex);
        if (hexObject == null) return;

        hexesToBuild = CalculateHexesToBuild(hex);
        UpdatePreviewPosition(hex);
    }

    private List<Hex> CalculateHexesToBuild(Hex hex) {
        return previewBuildingSO.tileSize == 1 ? new List<Hex> { hex } : CalculateBuildingHexes(hex, currentRotation);
    }

    private void UpdatePreviewPosition(Hex hex) {
        Point pointPosition = HexToPixel(layout, hex);
        Vector3 position = new Vector3((float)pointPosition.x, 1.5f, (float)pointPosition.y);
        previewInstance.transform.position = new Vector3(position.x, 1.5f, position.z);
    }

    private void HandleBuildingPlacement(List<Hex> hexesToBuild) {
        if (Input.GetMouseButtonDown(0) && previewInstance != null && !EventSystem.current.IsPointerOverGameObject()) {
            PlaceBuilding(hexesToBuild);
        }
    }

    private void PlaceBuilding(List<Hex> hexesToBuild) {
        Vector3 buildPosition =
            new Vector3(previewInstance.transform.position.x, 0f, previewInstance.transform.position.z);
        GameObject buildingInstance = Instantiate(buildingToBuild, buildPosition, previewInstance.transform.rotation);
        buildingInstance.transform.parent = map.transform;

        int hexNumber = 0;
        foreach (var hex in hexesToBuild) {
            ProcessHex(hex, buildingInstance);
            int[] newRoads = new int[previewBuildingSO.roads[hexNumber].roadArray.Length];
            for (int i = 0; i < newRoads.Length; i++) {
                newRoads[i] = ((previewBuildingSO.roads[hexNumber].roadArray[i] + currentRotation) % 6);
            }

            hex.AddConnections(newRoads);

            hexNumber++;
        }

        FinalizeBuildingPlacement();
    }

    private void ProcessHex(Hex hex, GameObject buildingInstance) {
        if (hexMap.TryGetValue(hex, out GameObject hexObjectPart)) {
            Destroy(hexObjectPart);
            hexMap.Remove(hex);
        }

        hexMap.Add(hex, buildingInstance);
        buildingsMap[hex] = buildingInstance;
    }

    private void FinalizeBuildingPlacement() {
        previewing = false;
        currentRotation = initialRotation;
        DestroyPreviewInstance();
        OnBuildingBuilt?.Invoke(null, previewBuildingSO);
    }

    private List<Hex> CalculateBuildingHexes(Hex baseHex, int rotation) {
        List<Hex> hexesToBuild = new List<Hex> { baseHex };
        baseHex.SetPizzeria();

        // Rotate the direction indices based on the current rotation
        int direction1 = (3 + rotation) % 6;
        int direction2 = (4 + rotation) % 6;

        hexesToBuild.Add(Hex.HexNeighbor(baseHex, direction1)); // First neighbor based on rotation
        hexesToBuild.Add(Hex.HexNeighbor(baseHex, direction2)); // Second neighbor based on rotation

        return hexesToBuild;
    }

    private bool CanBuild(List<Hex> hexes) {
        foreach (var hex in hexes) {
            if (buildingsMap.ContainsKey(hex) || !hexMap.ContainsKey(hex)) {
                return false;
            }
        }

        return true;
    }

    private void UpdatePreviewColor(bool canBuild) {
        material.SetColor(BaseColor, canBuild ? Color.green : Color.red);
    }

    public GameObject GetHexFromRay(RaycastHit hit, out Hex hex) {
        FractionalHex fractionalHex = PixelToHex(layout, new Point(hit.point.x, hit.point.z));
        hex = HexRound(fractionalHex);
        hexMap.TryGetValue(hex, out GameObject hexObject);
        return hexObject;
    }

    private void DestroyPreviewInstance() {
        foreach (GameObject preview in previewInstances) {
            Destroy(preview);
        }

        previewInstances.Clear();
        previewInstance = null;
        currentRotation = initialRotation;
    }

    private void DrawConnections() {
        foreach (var kvp in hexMap) {
            Hex hex = kvp.Key;
            Vector3 hexPosition =
                new Vector3((float)HexToPixel(layout, hex).x, 0, (float)HexToPixel(layout, hex).y);

            for (int i = 0; i < 6; i++) {
                if (hex.connections[i]) {
                    Vector3 neighbourHexPosition = new Vector3(
                        (float)HexToPixel(layout, Hex.HexNeighbor(hex, i)).x,
                        0,
                        (float)HexToPixel(layout, Hex.HexNeighbor(hex, i)).y);
                    Vector3 midpoint = (hexPosition + neighbourHexPosition) / 2;
                    Debug.DrawLine(hexPosition, midpoint, Color.red);
                }
            }
        }
    }

    private void HighlightTileOnMouseHover() {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = GetMouseRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            GameObject hexObject = GetHexFromRay(hit, out Hex hex);
            hexHighlighter.HighlightHex(hexObject);
        }
    }

    private Ray GetMouseRay(Vector3 mousePos) {
        return mainCamera.ScreenPointToRay(mousePos);
    }

    public GameObject GetHexFromMapNoRay(GameObject position, out Hex hex) {

        FractionalHex fractionalHex = PixelToHex(layout,
            new Point(position.transform.position.x, position.transform.position.z));
        hex = HexRound(fractionalHex);
        hexMap.TryGetValue(hex, out GameObject hexObject);
        return hexObject;
    }

    public Dictionary<Hex, GameObject> GetBuildingMap() {
        return buildingsMap;
    }

    public Dictionary<Hex, GameObject> GetMap() {
        return hexMap;
    }

    public static MouseCoordinates GetInstance() {
        return instance;
    }

    public Layout GetLayout() {
        return layout;
    }

}