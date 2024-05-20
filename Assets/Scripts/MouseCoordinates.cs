using System;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class MouseCoordinates : MonoBehaviour {
    public static event Action<object, EventArgs> OnBuildingBuilt;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject map;
    private HexGridGenerator hexGridGenerator;
    private GameObject previous;
    private Dictionary<RedBlockGridConverted.Hex, GameObject> hexMap;

    private Dictionary<RedBlockGridConverted.Hex, Boolean> buildingsMap = new();

    private RedBlockGridConverted.Layout layout;

    private HashSet<GameObject> previewInstances = new();
    private GameObject previewInstance;
    private bool previewing;
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

    private void OnPreviewingBuilding(GameObject prefab) {
        previewing = true;
        DestroyPreviewInstance();
        previewInstance = Instantiate(prefab, previewContainer.transform);
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
            GameObject hexObject = GetHexFromRay(hit, out RedBlockGridConverted.Hex hex);
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
            }

            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                GameObject hexObject = GetHexFromRay(hit, out RedBlockGridConverted.Hex hex);
                previewInstance.transform.position = hexObject.transform.position;

                // todo kubo this needs rework - the find should be searching only for 1  for all buildings so buildings need to somehow flag the tile that it's there
                if (Input.GetMouseButtonDown(0) && previewInstance != null && CanBuild(hex)) {
                    GameObject buildingInstance = Instantiate(previewInstance, previewInstance.transform.position,
                        previewInstance.transform.rotation);
                    buildingInstance.transform.parent = map.transform;
                    hexMap.Remove(hex);
                    hexMap.Add(hex, buildingInstance);
                    buildingsMap[hex] = true;

                    Destroy(hexObject);
                    previewing = false;
                    DestroyPreviewInstance();
                    OnBuildingBuilt?.Invoke(null, EventArgs.Empty);
                }
            }
        }
    }

    private bool CanBuild(RedBlockGridConverted.Hex hex) {
        return !buildingsMap.ContainsKey(hex);
    }

    private GameObject GetHexFromRay(RaycastHit hit, out RedBlockGridConverted.Hex hex) {
        RedBlockGridConverted.FractionalHex fractionalHex =
            RedBlockGridConverted.PixelToHex(layout, new RedBlockGridConverted.Point(hit.point.x, hit.point.z));
        hex = RedBlockGridConverted.HexRound(fractionalHex);
        GameObject hexObject = hexMap[hex];
        return hexObject;
    }

    private void DestroyPreviewInstance() {
        foreach (GameObject preview in previewInstances) {
            Destroy(preview);
        }

        previewInstances.Clear();
        previewInstance = null;
    }
}