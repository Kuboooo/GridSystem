using System;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class MouseCoordinates : MonoBehaviour {
    public static event Action<object, EventArgs> OnBuildingBuilt;
    public Material material;
    private Material backupMaterial;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject map;
    private HexGridGenerator hexGridGenerator;
    private GameObject previous;
    private Dictionary<RedblockGrid.Hex, GameObject> hexMap;

    private Dictionary<RedblockGrid.Hex, Boolean> buildingsMap = new();

    private RedblockGrid.Layout layout;

    private HashSet<GameObject> previewInstances = new();
    private GameObject previewInstance;
    private GameObject buildingToBuild;
    private bool previewing;
    private PreviewBuildingSO previewBuildingSO;
    private int currentRotation = 3; // Track the current rotation in multiples of 60 degrees

    [SerializeField] private GameObject previewContainer;

    private void OnEnable() {
        UIManager.OnPreviewingBuilding += OnPreviewingBuilding;
        UIManager.OnStopPreviewing += OnStopPreviewing;
    }

    private void OnDisable() {
        UIManager.OnPreviewingBuilding -= OnPreviewingBuilding;
        UIManager.OnStopPreviewing -= OnStopPreviewing;
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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            previewing = false;
            DestroyPreviewInstance();
        }
    }

    void HighlightTileOnMouseHover() {
        if (previous != null) {
            if (previous.transform.Find("Selected") != null) {
                previous.transform.Find("Selected").GameObject().SetActive(false);
            }

            previous = null;
        }

        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            GameObject hexObject = GetHexFromRay(hit, out RedblockGrid.Hex hex);
            if (hexObject != null && hexObject.transform.Find("Selected") != null) {
                Transform find = hexObject.transform.Find("Selected");
                find.GameObject().SetActive(true);
                previous = hexObject;
            }
            else {
                previous = null;
            }
        }
    }

    private void ShowPreview() {
        if (!previewing) {
            DestroyPreviewInstance();
        }
        else {
            if (Input.GetKeyDown(KeyCode.Q)) {
                previewInstance.transform.rotation *= Quaternion.Euler(0, 60, 0);
                currentRotation = (currentRotation + 1) % 6; // Increment rotation and wrap around every 6 steps
            }

            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hexObject = GetHexFromRay(hit, out RedblockGrid.Hex hex);
                if (hexObject != null)
                {
                    
                    List<RedblockGrid.Hex> hexesToBuild = previewBuildingSO.tileSize == 1 ? new List<RedblockGrid.Hex> {
                        hex
                    } : CalculateBuildingHexes(hex, currentRotation);

                    Vector3 position = hexObject.transform.position;
                    previewInstance.transform.position = new Vector3((float)RedblockGrid.HexToPixel(layout, hex).x, 1.5f, (float)RedblockGrid.HexToPixel(layout, hex).y);
                    bool canBuild = CanBuild(hexesToBuild);
                    UpdatePreviewColor(canBuild);

                    if (Input.GetMouseButtonDown(0) && previewInstance != null && canBuild)
                    {
                        Vector3 buildPosition = new Vector3(previewInstance.transform.position.x, 0f, previewInstance.transform.position.z);
                        GameObject buildingInstance = Instantiate(buildingToBuild, buildPosition, previewInstance.transform.rotation);
                        buildingInstance.transform.parent = map.transform;

                        foreach (var buildHex in hexesToBuild)
                        {
                            hexMap.TryGetValue(buildHex, out GameObject hexObjectPart);
                            Destroy(hexObjectPart);
                            hexMap.Remove(buildHex);
                            hexMap.Add(buildHex, buildingInstance);
                            buildingsMap[buildHex] = true;
                        }

                        // Destroy(hexObject);
                        previewing = false;
                        currentRotation = 3;
                        DestroyPreviewInstance();
                        OnBuildingBuilt?.Invoke(null, EventArgs.Empty);
                    }
                }
            }
        }
    }

    private List<RedblockGrid.Hex> CalculateBuildingHexes(RedblockGrid.Hex baseHex, int rotation)
    {
        List<RedblockGrid.Hex> hexesToBuild = new List<RedblockGrid.Hex> { baseHex };

        // Rotate the direction indices based on the current rotation
        int direction1 = (0 + rotation) % 6;
        int direction2 = (1 + rotation) % 6;

        hexesToBuild.Add(RedblockGrid.Hex.HexNeighbor(baseHex, direction1)); // First neighbor based on rotation
        hexesToBuild.Add(RedblockGrid.Hex.HexNeighbor(baseHex, direction2)); // Second neighbor based on rotation

        return hexesToBuild;
    }

    private bool CanBuild(List<RedblockGrid.Hex> hexes)
    {
        foreach (var hex in hexes)
        {
            if (buildingsMap.ContainsKey(hex))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdatePreviewColor(bool canBuild)
    {
        material.SetColor("_BaseColor", canBuild ? Color.green : Color.red);
    }

    private GameObject GetHexFromRay(RaycastHit hit, out RedblockGrid.Hex hex) {
        RedblockGrid.FractionalHex fractionalHex = RedblockGrid.PixelToHex(layout, new RedblockGrid.Point(hit.point.x, hit.point.z));
        hex = RedblockGrid.HexRound(fractionalHex);
        hexMap.TryGetValue(hex, out GameObject hexObject);
        return hexObject;
    }

    private void DestroyPreviewInstance() {
        foreach (GameObject preview in previewInstances) {
            Destroy(preview);
        }

        previewInstances.Clear();
        previewInstance = null;
        currentRotation = 3;
    }
}