using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RedblockGrid;

public class MouseCoordinates : MonoBehaviour {
    
    private const string HEXOBJECT_LAYER = "HexObject";
    
    private static MouseCoordinates instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject map;
    
    private HexGridGenerator hexGridGenerator;
    private Layout layout;
    private Dictionary<Hex, GameObject> hexMap;
    private Dictionary<Hex, GameObject> buildingsMap = new();


    private void Awake() {
        instance = this;
    }


    private void Start() {
        hexGridGenerator = HexGridGenerator.GetInstance();
        hexMap = hexGridGenerator.GetHexMap();
        layout = hexGridGenerator.GetLayout();
    }

    void Update() {
        HighlightTileOnMouseHover();
    }


    public bool TryGetRaycastHit(out RaycastHit hit) {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        LayerMask layerMask = LayerMask.GetMask(HEXOBJECT_LAYER);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
    }


    public GameObject GetHexFromRay(RaycastHit hit, out Hex hex) {
        FractionalHex fractionalHex = PixelToHex(layout, new Point(hit.point.x, hit.point.z));
        hex = HexRound(fractionalHex);
        hexMap.TryGetValue(hex, out GameObject hexObject);
        return hexObject;
    }

    private void HighlightTileOnMouseHover() {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = GetMouseRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            GameObject hexObject = GetHexFromRay(hit, out Hex hex);
            HexHighlighter.HighlightHex(hexObject);
        }
    }

    private Ray GetMouseRay(Vector3 mousePos) {
        return mainCamera.ScreenPointToRay(mousePos);
    }

    public GameObject GetHexFromMapNoRay(GameObject position, out Hex hex) {
        FractionalHex fractionalHex = PixelToHex(layout,
            new Point(position.transform.position.x, position.transform.position.z));
        hex = HexRound(fractionalHex);
        hexMap.TryGetValue(hex, out GameObject hexObject);
        return hexObject;
    }

    public Hex GetKeyFromMap(Hex hex) {
        return hexMap.Keys.FirstOrDefault(k => k == hex);
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