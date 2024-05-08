using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseCoordinates : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject sphere;
    [SerializeField] private GameObject building;

    private GameObject previous = null;

    void Update() {
        HighlightTileOnMouseHover();
        if (Input.GetMouseButtonDown(0) && previous != null) {
            Instantiate(building, previous.transform.position, Quaternion.identity);
        }        
        if (Input.GetMouseButtonDown(1) && previous != null) {
            Destroy(previous.transform.GameObject());
        }
    }

    private void HighlightTileOnMouseHover() {

        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (previous != null) {
            previous.GameObject().SetActive(false);
            previous = null;
        }

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Transform find = hit.transform.Find("Selected");
            if (find != null) {
                find.GameObject().SetActive(true);
            }
            sphere.transform.position = hit.point;
            previous = find.GameObject();
        }
    }

}