using System;
using System.Collections.Generic;
using System.IO;
using Enums;
using SOs;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using static RedblockGrid;

public class MouseCoordinates : MonoBehaviour {

    private const string HEXOBJECT_LAYER = "HexObject";
    public static event Action<object, PreviewBuildingSO> OnBuildingBuilt;
    public Material material;
    private Material backupMaterial;

    [SerializeField] private PreviewBuildingSO toLoadBuildingVillageSO;
    [SerializeField] private PreviewBuildingSO toLoadBuildingPondSO;
    [SerializeField] private PreviewBuildingSO toLoadBuildingPizzeriaSO;
    [SerializeField] private GameObject baseHexPrefab;

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
    private List<Dictionary<int, Dictionary<int, List<Vector3>>>> waypointsForCurrentSO;

    private void OnEnable() {
        UIManager.OnPreviewingBuilding += OnPreviewingBuilding;
        UIManager.OnStopPreviewing += OnStopPreviewing;
        UIManager.OnSaveGameButtonPressed += OnSaveGameButtonPressed;
        UIManager.OnLoadGameButtonPressed += OnLoadGameButtonPressed;
    }

    private void OnDisable() {
        UIManager.OnPreviewingBuilding -= OnPreviewingBuilding;
        UIManager.OnStopPreviewing -= OnStopPreviewing;
        UIManager.OnSaveGameButtonPressed -= OnSaveGameButtonPressed;
        UIManager.OnLoadGameButtonPressed -= OnLoadGameButtonPressed;
    }

    private void OnSaveGameButtonPressed(object arg1, EventArgs arg2) {
        Save();
    }

    private void OnLoadGameButtonPressed(object arg1, EventArgs arg2) {
        Load();
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
        waypointsForCurrentSO = previewBuildingSo.waypoints;
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
        else {
            UpdatePreviewColor(false);
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
        // currentRotation = (currentRotation + 1) % 6;
        currentRotation = ((currentRotation - 1) + 6) % 6;
    }

    private bool TryGetRaycastHit(out RaycastHit hit) {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        LayerMask layerMask = LayerMask.GetMask(HEXOBJECT_LAYER);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
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
            ProcessWaypointsAndRoads(hexNumber, hex);
            hexNumber++;
        }

        FinalizeBuildingPlacement();
    }

    private void ProcessWaypointsAndRoads(int hexNumber, Hex hex) {
        hex.SetMultiHexDirection(hexNumber);
        int[] newRoads = new int[previewBuildingSO.roads[hexNumber].roadArray.Length];
        for (int i = 0; i < newRoads.Length; i++) {
            newRoads[i] = ((previewBuildingSO.roads[hexNumber].roadArray[i] + currentRotation) % 6);
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
        hex.SetWaypoints(waypointsForCurrentSO[hexNumber], currentRotation);
        hex.AddConnections(newRoads);
    }

    private void ProcessHex(Hex hex, GameObject buildingInstance) {
        if (hexMap.TryGetValue(hex, out GameObject hexObjectPart)) {
            Destroy(hexObjectPart);
            hexMap.Remove(hex);
        }

        hex.worldPosition = buildingInstance.transform.position;
        hex.SetRotation(currentRotation);
        hex.SetBuildingType(previewBuildingSO.buildingType);
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
        int direction1 = (0 + rotation) % 6;
        int direction2 = (5 + rotation) % 6;

        var neighbour1 = Hex.HexNeighbor(baseHex, direction1);
        var neighbour2 = Hex.HexNeighbor(baseHex, direction2);
        hexesToBuild.Add(neighbour1); // First neighbor based on rotation
        hexesToBuild.Add(neighbour2); // Second neighbor based on rotation

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

    public void Save() {
        Debug.Log(Application.persistentDataPath);
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (
            BinaryWriter writer =
            new BinaryWriter(File.Open(path, FileMode.Create))
        ) {
            writer.Write(hexMap.Count);
            foreach (var hexEntry in hexMap.Keys) {
                hexEntry.Save(writer);
            }
        }
    }

    public void Load() {

        foreach (var hex in hexMap.Values) {
            Destroy(hex);
        }

        hexMap.Clear();
        buildingsMap.Clear();

        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (
            BinaryReader reader =
            new BinaryReader(File.OpenRead(path))
        ) {
            int hexCount = reader.ReadInt32();
            for (int i = 0; i < hexCount; i++) {
                RedblockGrid.Hex hex = new RedblockGrid.Hex(0, 0, 0);
                hex.Load(reader);
                BuildingType buildingType = hex.GetBuildingType();
                GameObject buildingInstance;
                PreviewBuildingSO so;
                if (BuildingType.Basic != buildingType) {
                    if (BuildingType.Village == buildingType) {
                        so = toLoadBuildingVillageSO;
                        buildingInstance = Instantiate(toLoadBuildingVillageSO.prefabToBuild, hex.worldPosition,
                            Quaternion.Euler(0, hex.GetRotation() * -60, 0));
                    }
                    else if (BuildingType.Pizzeria == buildingType) {
                        so = toLoadBuildingPizzeriaSO;
                        buildingInstance = Instantiate(toLoadBuildingPizzeriaSO.prefabToBuild, hex.worldPosition,
                            Quaternion.Euler(0, hex.GetRotation() * -60, 0));
                    }
                    else {
                        so = toLoadBuildingPondSO;
                        buildingInstance = Instantiate(toLoadBuildingPondSO.prefabToBuild, hex.worldPosition,
                            Quaternion.Euler(0, hex.GetRotation() * -60, 0));
                    }

                    buildingsMap.Add(hex, buildingInstance);

                    int[] newRoads = new int[so.roads[hex.GetMultiHexDirection()].roadArray.Length];
                    for (int x = 0; x < newRoads.Length; x++) {
                        newRoads[x] = ((so.roads[hex.GetMultiHexDirection()].roadArray[x] + hex.GetRotation()) % 6);
                    }

                    Dictionary<int, Dictionary<int, List<Vector3>>> waypoints =
                        new Dictionary<int, Dictionary<int, List<Vector3>>>();
                    for (int k = 0; k < 6; k++) {
                        waypoints[k] = new Dictionary<int, List<Vector3>>();
                        for (int j = 0; j < 6; j++) {
                            waypoints[k][j] = new List<Vector3>();
                        }
                    }

                    hex.waypoints = waypoints;
                    hex.SetWaypoints(so.waypoints[hex.GetMultiHexDirection()], hex.GetRotation());
                    hex.AddConnections(newRoads);

                }
                else {
                    buildingInstance = Instantiate(baseHexPrefab, hex.worldPosition, Quaternion.Euler(0, 0, 0));
                }

                hexMap.Add(hex, buildingInstance);
            }
        }
    }


}