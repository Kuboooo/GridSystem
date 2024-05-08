using Unity.VisualScripting;
using UnityEngine;

public class MouseCoordinates : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject sphere;
    [SerializeField] private GameObject building;
    [SerializeField] private GameObject building2;
    
    private GameObject buildingPreview;
    private GameObject previous = null;

    void Update() {
        HighlightTileOnMouseHover();
        BuildBuilding();
        DestroyBuilding();
    }

    private void BuildBuilding() {

        if (Input.GetMouseButtonDown(0) && previous != null && previous.name == "HexTop_ClayGround(Clone)" && previous.transform.Find("Building1(Clone)") == null) {
            GameObject buildingInstance = Instantiate(building, previous.transform.position, Quaternion.identity);
            buildingInstance.transform.parent = previous.transform;
        }
        if (Input.GetKeyDown("space") && previous != null && previous.name == "HexTop_ClayGround(Clone)" && previous.transform.Find("Building1(Clone)") == null) {
            GameObject buildingInstance = Instantiate(building2, previous.transform.position, Quaternion.identity);
            buildingInstance.transform.parent = previous.transform;
        }
    }

    private void DestroyBuilding() {
        if (Input.GetMouseButtonDown(1) && previous != null && previous.name == "HexTop_ClayGround(Clone)" && previous.transform.Find("Building1(Clone)") != null) {
            Destroy(previous.transform.GameObject().transform.Find("Building1(Clone)").transform.GameObject());
        } else if ( Input.GetMouseButtonDown(1) ) {
            if (previous != null && (previous.name == "Cube" || previous.name == "Building2(Clone)")) {
                Destroy(previous.transform.parent.GameObject());
            }
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