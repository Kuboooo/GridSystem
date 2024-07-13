using System;
using System.Collections.Generic;
using Enums;
using Grid;
using SOs;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using static Hex;

namespace StructureBuilding {
    public class PreviewManager : MonoBehaviour {
        private const int MAX_ROTATION = 6;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        [SerializeField] private GameObject previewContainer;
        [SerializeField] private Material material;

        private bool previewing;
        private readonly int initialRotation = 0; // Track the current rotation in multiples of 60 degrees
        private int currentRotation = 0;
        private GameObject previewInstance;
        private PreviewBuildingSO previewBuildingSO;
        private MouseCoordinates mouseCoordinates;
        private BuildingPlacer buildingPlacer;

        private void OnEnable() {
            UIManager.OnPreviewingBuilding += OnPreviewingBuilding;
            UIManager.OnStopPreviewing += OnStopPreviewing;
            InputHandler.OnEscapePressed += HandleEscapeKey;
            InputHandler.OnRotatePressed += HandleRotation;
            // InputHandler.OnRightMouseButtonPressed += HandleRightMouseButtonPressed;
        }


        private void OnDisable() {
            UIManager.OnPreviewingBuilding -= OnPreviewingBuilding;
            UIManager.OnStopPreviewing -= OnStopPreviewing;
            InputHandler.OnEscapePressed -= HandleEscapeKey;
            InputHandler.OnRotatePressed -= HandleRotation;
            // InputHandler.OnRightMouseButtonPressed -= HandleRightMouseButtonPressed;
        }

        private void Start() {
            mouseCoordinates = MouseCoordinates.GetInstance();
            buildingPlacer = GetComponent<BuildingPlacer>();
        }

        private void Update() {
            ShowPreview();
        }

        private void ShowPreview() {
            if (!previewing) {
                DestroyPreviewInstance();
                return;
            }

            RaycastHit hit;
            if (mouseCoordinates.TryGetRaycastHit(out hit)) {
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

        private void HandleBuildingPlacement(List<Hex> hexesToBuild) {
            if (Input.GetMouseButtonDown(0) && previewInstance != null &&
                !EventSystem.current.IsPointerOverGameObject()) {
                buildingPlacer.PlaceBuilding(hexesToBuild, previewBuildingSO.prefabToBuild, previewInstance,
                    currentRotation, previewBuildingSO);
                FinalizePreviewing();
            }
        }


        private void HandleEscapeKey() {
            StopPreview();
        }

        private void StopPreview() {
            previewing = false;
            DestroyPreviewInstance();
        }

        private void HandleRotation() {
            if (previewInstance != null) {
                previewInstance.transform.rotation *= Quaternion.Euler(0, 60, 0);
                // currentRotation = (currentRotation + 1) % 6;
                currentRotation = ((currentRotation - 1) + 6) % MAX_ROTATION;
            }
        }

        public void UpdatePreviewColor(bool canBuild) {
            material.SetColor(BaseColor, canBuild ? Color.green : Color.red);
        }

        private void DestroyPreviewInstance() {
            if (previewInstance != null) {
                Destroy(previewInstance);
                previewInstance = null;
                currentRotation = 0;
            }
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
            previewInstance.transform.localPosition = Vector3.zero;
            previewInstance.transform.localRotation = Quaternion.identity;
            previewInstance.transform.localScale = Vector3.one;
        }

        private void ShowPreviewOnRaycastHit(RaycastHit hit, out List<Hex> hexesToBuild) {
            hexesToBuild = new List<Hex>();
            GameObject hexObject = mouseCoordinates.GetHexFromRay(hit, out Hex hex);
            if (hexObject == null) return;

            hexesToBuild = CalculateHexesToBuild(hex);
            UpdatePreviewPosition(hex);
        }

        private List<Hex> CalculateHexesToBuild(Hex hex) {
            return previewBuildingSO.tileSize == 1
                ? new List<Hex> { hex }
                : CalculateBuildingHexes(hex, currentRotation);
        }

        private List<Hex> CalculateBuildingHexes(Hex baseHex, int rotation) {
            List<Hex> hexesToBuild = InitializeHexesToBuild(baseHex);

            AddNeighborHexes(hexesToBuild, baseHex, rotation);
            SpecifyMainHex(baseHex, hexesToBuild);

            return hexesToBuild;
        }

        private List<Hex> InitializeHexesToBuild(Hex baseHex) {
            List<Hex> hexesToBuild = new List<Hex> { baseHex };

            if (previewBuildingSO.buildingType == BuildingType.Pizzeria) {
                baseHex.GetHexProperties().SetPizzeria(true);
            }

            return hexesToBuild;
        }

        private void AddNeighborHexes(List<Hex> hexesToBuild, Hex baseHex, int rotation) {
            if (previewBuildingSO.multiHexPositionDirectionList is not null) {
                foreach (Hex multiHexPositionDirection in previewBuildingSO.multiHexPositionDirectionList) {
                    var hexAdd = HexAdd(baseHex, multiHexPositionDirection);
                    var rotatedHex = RotateShoveHex(baseHex, hexAdd, rotation);
                    hexesToBuild.Add(rotatedHex);
                }
            }
            else {
                foreach (int index in previewBuildingSO.multiHexIndexPosition) {
                    int direction = CalculateDirection(index, rotation);
                    Hex neighbor = HexNeighbor(baseHex, direction);
                    hexesToBuild.Add(neighbor);
                }
            }
        }

        private int CalculateDirection(int index, int rotation) {
            return (index + rotation) % MAX_ROTATION;
        }

        private void SpecifyMainHex(Hex baseHex, List<Hex> hexesToBuild) {
            foreach (var hex in hexesToBuild) {
                hex.GetHexProperties().SetMainCoordinates(baseHex);
            }
        }

        private void UpdatePreviewPosition(Hex hex) {
            Point pointPosition = HexToPixel(mouseCoordinates.GetLayout(), hex);
            Vector3 position = new Vector3((float)pointPosition.x, 1.5f, (float)pointPosition.y);
            previewInstance.transform.position = new Vector3(position.x, 1.5f, position.z);
        }

        private bool CanBuild(List<Hex> hexes) {
            foreach (var hex in hexes) {
                if (mouseCoordinates.GetBuildingMap().ContainsKey(hex) || !mouseCoordinates.GetMap().ContainsKey(hex)) {
                    return false;
                }
            }

            return true;
        }

        private void FinalizePreviewing() {
            previewing = false;
            currentRotation = initialRotation;
            DestroyPreviewInstance();
        }
    }
}