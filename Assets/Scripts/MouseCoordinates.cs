using Unity.VisualScripting;
using UnityEngine;

public class MouseCoordinates : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject sphere;
    [SerializeField] private GameObject building;

    private GameObject previous = null;

    void Update() {
        HighlightTileOnMouseHover();
        if (Input.GetMouseButtonDown(0) && previous != null && previous.name == "HexTop_ClayGround(Clone)" && previous.transform.Find("Building1(Clone)") == null) {
            GameObject buildingInstance = Instantiate(building, previous.transform.position, Quaternion.identity);
            buildingInstance.transform.parent = previous.transform;
        }

        if (Input.GetMouseButtonDown(1) && previous != null && previous.name == "HexTop_ClayGround(Clone)" && previous.transform.Find("Building1(Clone)") != null) {
            previous.transform.parent = null;
            
            Destroy(previous.transform.GameObject().transform.Find("Building1(Clone)").transform.GameObject());
        }
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
            Transform find = hit.transform.Find("Selected");
            if (find != null) {
                find.GameObject().SetActive(true);
            }

            sphere.transform.position = hit.point;
            previous = hit.transform.gameObject;
        }
    }

}