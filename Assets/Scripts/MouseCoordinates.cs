using Unity.VisualScripting;
using UnityEngine;

public class MouseCoordinates : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject sphere;
    private GameObject previous = null;

    void Update() {
        // HighlightTileOnMouseHover();
    }

    private void HighlightTileOnMouseHover() {

        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (previous != null) {
            if (previous.transform.gameObject.transform.Find("Selected") != null) {
                previous.transform.gameObject.transform.Find("Selected").GameObject().SetActive(false);
            }
            previous = null;
        }

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Debug.Log("Hit: " + hit.transform.position.x + " " + hit.transform.position.y + " " + hit.transform.position.z);
            // Debug.Log("Hit parent name: " + hit.transform.parent.transform.name);
            Transform find = hit.transform.parent.transform.Find("Selected");
            if (find != null) {
                find.GameObject().SetActive(true);
            }

            sphere.transform.position = hit.point;
            previous = hit.transform.parent.transform.gameObject;
        }
        else {
            // Debug.Log("missed");

        }
    }

}