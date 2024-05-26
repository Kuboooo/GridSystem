using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static RedblockGrid;

public class MouseCoordinates : MonoBehaviour {

    private Hex start;
    private Hex goal;

    private const string HEX_HOVER_IDENTIFIER = "Selected";


    public static event Action<object, PreviewBuildingSO> OnBuildingBuilt;
    public Material material;
    private Material backupMaterial;

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
    // private int singleCurrentRotation; // Track the current rotation in multiples of 60 degrees
    private int initialRotation = 0; // Track the current rotation in multiples of 60 degrees
    // private int singleInitialRotation = 0; // Track the current rotation in multiples of 60 degrees

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
    }

    void Update() {
        HighlightTileOnMouseHover();
        ShowPreview();
        //FindPath();

        //DrawConnections();
    }

    private void FindPath() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                GameObject hexObject = GetHexFromRay(hit, out Hex hex);
                if (hex is not null) {
                    start = hexMap.Keys.FirstOrDefault(k => k == hex);
                    hexObject.transform.Find("Selected").transform.gameObject.SetActive(true);
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                GameObject hexObject = GetHexFromRay(hit, out Hex hex);
                if (hex is not null) {
                    goal = hexMap.Keys.FirstOrDefault(k => k == hex);
                    hexObject.transform.Find("Selected").transform.gameObject.SetActive(true);
                }
            }
        }

        if (start is not null && goal is not null) {

            Func<Hex, Hex, bool> isWalkable = (_, _) => true;

            List<Hex> path = HexPathfinding.FindPath(start, goal, hexMap, isWalkable);

            if (path is not null) {
                Debug.Log("Path found:");
                foreach (var hex in path) {
                    var hexGameObject = hexMap[hex];
                    hexGameObject.transform.Find("Selected").transform.gameObject.SetActive(true);
                    start = null;
                    goal = null;
                }
            } else {
                Debug.Log("No path found.");
                start = null;
                goal = null;
            }
        }
    }

    private void HighlightTileOnMouseHover() {
        if (previous?.GameObject() != null) {
            if (previous.transform.Find(HEX_HOVER_IDENTIFIER) is not null) {
                previous.transform.Find(HEX_HOVER_IDENTIFIER).GameObject().SetActive(false);
            }

            previous = null;
        }

        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            GameObject hexObject = GetHexFromRay(hit, out Hex hex);
            if (hexObject is not null && hexObject.transform.Find(HEX_HOVER_IDENTIFIER) is not null) {
                Transform find = hexObject.transform.Find(HEX_HOVER_IDENTIFIER);
                find.GameObject().SetActive(true);
                previous = hexObject;
            } else {
                previous = null;
            }
        }
    }

    private void ShowPreview() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            previewing = false;
            DestroyPreviewInstance();
        }
        if (!previewing) {
            DestroyPreviewInstance();
        } else {

            if (Input.GetKeyDown(KeyCode.X)) {
                previewInstance.transform.rotation *= Quaternion.Euler(0, 60, 0);
                currentRotation = (currentRotation + 1) % 6; // Increment rotation and wrap around every 6 steps
            }

            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                GameObject hexObject = GetHexFromRay(hit, out Hex hex);
                if (hexObject is not null) {

                    List<Hex> hexesToBuild = previewBuildingSO.tileSize == 1 ? new List<Hex> {
                        hex
                    } : CalculateBuildingHexes(hex, currentRotation);

                    previewInstance.transform.position = new Vector3((float)HexToPixel(layout, hex).x, 1.5f, (float)HexToPixel(layout, hex).y);
                    bool canBuild = CanBuild(hexesToBuild);
                    UpdatePreviewColor(canBuild);

                    if (Input.GetMouseButtonDown(0) && previewInstance is not null && canBuild) {
                        if (!EventSystem.current.IsPointerOverGameObject()) {

                            Vector3 buildPosition = new Vector3(previewInstance.transform.position.x, 0f, previewInstance.transform.position.z);
                            GameObject buildingInstance = Instantiate(buildingToBuild, buildPosition, previewInstance.transform.rotation);
                            buildingInstance.transform.parent = map.transform;

                            for (int hexNumber = 0; hexNumber < hexesToBuild.Count; hexNumber++) {
                                Hex buildHex = hexesToBuild[hexNumber];
                                hexMap.TryGetValue(buildHex, out GameObject hexObjectPart);
                                Destroy(hexObjectPart);
                                hexMap.Remove(buildHex);
                                int[] newRoads = new int[previewBuildingSO.roads[hexNumber].roadArray.Length];
                                for (int i = 0; i < newRoads.Length; i++) {
                                    newRoads[i] = ((previewBuildingSO.roads[hexNumber].roadArray[i] + currentRotation) % 6);
                                }
                                buildHex.AddConnections(newRoads);
                                hexMap.Add(buildHex, buildingInstance);
                                buildingsMap[buildHex] = buildingInstance;
                            }

                            previewing = false;
                            currentRotation = initialRotation;
                            DestroyPreviewInstance();
                            OnBuildingBuilt?.Invoke(null, previewBuildingSO);
                        }
                    }
                }
            }
        }
    }

    private List<Hex> CalculateBuildingHexes(Hex baseHex, int rotation) {
        List<Hex> hexesToBuild = new List<Hex> { baseHex };

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
            Vector3 hexPosition = new Vector3((float)HexToPixel(layout, hex).x, 0, (float)HexToPixel(layout, hex).y);

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

    public GameObject GetHexFromMapNoRay(GameObject position, out Hex hex) {

        FractionalHex fractionalHex = PixelToHex(layout, new Point(position.transform.position.x, position.transform.position.z));
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