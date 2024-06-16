using TMPro;
using UnityEngine;

namespace UI {
    public class BuildingDetailsPopup : MonoBehaviour {
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private TextMeshProUGUI detailText;

        private MouseCoordinates mouseCoordinates;
        private Camera mainCamera;

        private void Start() {
            popupPanel.SetActive(false);
            mouseCoordinates = MouseCoordinates.GetInstance();
            mainCamera = Camera.main;
        }

        private void Update() {
            if (Input.GetMouseButton(1)) {
                // ShowPopup();
                // HighlightConnectedBuildingsInRange();
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
    }
}