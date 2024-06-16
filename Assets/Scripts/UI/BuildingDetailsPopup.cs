using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RedblockGrid;

namespace UI {

    public class BuildingDetailsPopup : MonoBehaviour {

        [SerializeField] private GameObject popupPanel;
        [SerializeField] private TextMeshProUGUI detailText;
        [SerializeField] private SphereMeshGenerator sphereMeshGenerator;
        [SerializeField] private CircleMesh circleMeshGenerator;
        private MouseCoordinates mouseCoordinates;
        private Camera mainCamera;

        [SerializeField] private int areaOfEffect = 4;

        private void Start() {
            popupPanel.SetActive(false);
            mouseCoordinates = MouseCoordinates.GetInstance();
            mainCamera = Camera.main;
        }

        private void Update() {
            if (Input.GetMouseButton(1)) {
                // ShowPopup();
                // HighlightConnectedBuildingsInRange();
               GetAllBuildingsInRange();
                
                // sphereMeshGenerator.GenerateMesh();
            }
            else {
                HidePopup();
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                // HexHighlighter.UnhighlightHexesRange();
            }
        }

        // void ShowPopup() {
        //     // Set the position of the popup to be on top of the game object
        //     Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        //     popupPanel.transform.position = screenPosition;
        //
        //     Vector3 mousePos = Input.mousePosition;
        //     Ray ray = GetMouseRay(mousePos);
        //     Hex hex = null;
        //     if (Physics.Raycast(ray, out RaycastHit hit)) {
        //         mouseCoordinates.GetHexFromRay(hit, out hex);
        //         hex = mouseCoordinates.GetKeyFromMap(hex);
        //         if (hex is not null) {
        //             bool pizzeriaOnPath = false;
        //             var inRangeCoordinates = HexPathfinding.GetInRangeCoordinates(hex, 5);
        //             HexHighlighter.HighlightHexRange(hex, mouseCoordinates.GetMap(), inRangeCoordinates);
        //             foreach (var hexInRange in inRangeCoordinates) {
        //                 Hex currentHex = mouseCoordinates.GetBuildingMap().Keys.FirstOrDefault(k => k == hexInRange);
        //                 // this calculates towards building pizzeria, not specific IS PIZZA check
        //                 if (currentHex?.GetBuildingType() == BuildingType.Pizzeria) {
        //                     if (HexPathfinding.FindPath(hex, currentHex, mouseCoordinates.GetMap(), (_, _) => true,
        //                             5) is not null) {
        //                         pizzeriaOnPath = true;
        //                         break;
        //                     }
        //                 }
        //             }
        //
        //             detailText.text = "Is this pizzeria " + hex.IsPizzeria + " building type: " +
        //                               hex.GetBuildingType() + " \n"
        //                               + " q: " + hex.q_ + " r: " + hex.r_ + " s: " + hex.s_ + " world pos: " +
        //                               hex.worldPosition + " \n"
        //                               + " rotation: " + hex.GetRotation() + " multiHexDirection: " +
        //                               hex.GetMultiHexDirection() + " \n" +
        //                               "is pizzeria in range of 5: " + pizzeriaOnPath;
        //         }
        //     }
        //     else {
        //         // Update the details in the popup
        //         detailText.text = "Is this pizzeria found nothing..? ";
        //     }
        //
        //     // Show the popup
        //     popupPanel.SetActive(true);
        // }

        private Ray GetMouseRay(Vector3 mousePos) {
            return mainCamera.ScreenPointToRay(mousePos);
        }

        void HidePopup() {
            // Hide the popup
            popupPanel.SetActive(false);
        }

        private void HighlightConnectedBuildingsInRange() {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = GetMouseRay(mousePos);
            Hex hex = null;
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                mouseCoordinates.GetHexFromRay(hit, out hex);
                hex = mouseCoordinates.GetKeyFromMap(hex);
                if (hex is not null) {
                    var onlyConnectedInRange = GetOnlyConnectedInRange(hex);
                    HexHighlighter.HighlightHexRange(hex, mouseCoordinates.GetMap(), onlyConnectedInRange);
                }
            }
        }
        private  void GetAllBuildingsInRange() {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = GetMouseRay(mousePos);
            Hex hex;
            float furthestX = 0;
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                mouseCoordinates.GetHexFromRay(hit, out hex);
                hex = mouseCoordinates.GetKeyFromMap(hex);
                if (hex is not null) {
                    var allInRange = GetAllInRange(hex);
                    foreach (var inRange in allInRange) {
                        if (Math.Abs(hex.worldPosition.x) + Math.Abs(inRange.worldPosition.x) > furthestX) {
                            furthestX = hex.worldPosition.x - inRange.worldPosition.x;
                        }
                    }
                sphereMeshGenerator.worldPosition = new Vector3(hex.worldPosition.x, hex.worldPosition.y, hex.worldPosition.z);
                sphereMeshGenerator.radius = furthestX;
                sphereMeshGenerator.GenerateMesh();
                circleMeshGenerator.worldPosition = new Vector3(hex.worldPosition.x, hex.worldPosition.y, hex.worldPosition.z);
                circleMeshGenerator.radius = furthestX;
                circleMeshGenerator.GenerateMesh();
                HexHighlighter.HighlightHexRange(hex, mouseCoordinates.GetMap(), allInRange);
                }
            }
        }

        private List<Hex> GetOnlyConnectedInRange(Hex hex) {
            List<Hex> result = new List<Hex>();
            var inRangeCoordinates = HexPathfinding.GetInRangeCoordinates(hex, 2);
            Debug.Log("In range count: " + inRangeCoordinates.Count);
            foreach (var hexInRange in inRangeCoordinates) {
                Hex currentHex = mouseCoordinates.GetKeyFromMap(hexInRange);
                if (currentHex is null || currentHex == hex) continue;
                // if (currentHex.GetBuildingType() == BuildingType.Basic) continue;
                // this calculates towards building pizzeria, not specific IS PIZZA check
                // if (currentHex?.GetBuildingType() == BuildingType.Pizzeria) {
                if (HexPathfinding.FindPath(hex, currentHex, mouseCoordinates.GetMap(), (_, _) => true,
                        areaOfEffect) is not null) {
                    result.Add(currentHex);
                }
                // }
            }

            Debug.Log("Results count: " + result.Count);
            return result;
        }

        private List<Hex> GetAllInRange(Hex hex) {
            List<Hex> result = new List<Hex>();
            var inRangeCoordinates = HexPathfinding.GetInRangeCoordinates(hex, areaOfEffect);
            foreach (var hexInRange in inRangeCoordinates) {
                Hex currentHex = mouseCoordinates.GetKeyFromMap(hexInRange);
                if (currentHex is null || currentHex == hex) continue;
                result.Add(currentHex);
            }

            Debug.Log("Results count: " + result.Count);
            return result;
        }

    }

}