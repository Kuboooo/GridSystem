using System.Collections.Generic;
using System.Linq;
using Grid;
using StructureBuilding;
using UnityEngine;
using static Hex;
using Layout = Grid.Layout;

public class MouseCoordinates : MonoBehaviour {
    
    private const string HEXOBJECT_LAYER = "HexObject";
    
    private static MouseCoordinates instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject map;
    
    private HexGridGenerator hexGridGenerator;
    private Layout layout;
    private Dictionary<Hex, GameObject> hexMap;
    private Dictionary<Hex, Building> buildingsMap = new();
    // TODO KUBO serialize these(save/load) 
    private Dictionary<Hex, Hex> hospitalsMap = new();
    private Dictionary<Hex, Hex> pizzeriasMap = new();
    private Dictionary<Hex, Hex> schoolsMap = new();
    private Dictionary<Hex, Hex> powerPlantsMap = new();
    private Dictionary<Hex, Hex> jobCentersMap = new();
    private Dictionary<Hex, Hex> pondsMap = new();
    private Dictionary<Hex, Hex> villageMap = new();
    

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

    public Dictionary<Hex, Building> GetBuildingMap() {
        return buildingsMap;
    }

    public Dictionary<Hex, Hex> GetPizzeriasMap() {
        return pizzeriasMap;
    }

    public Dictionary<Hex, Hex> GetHospitalsMap() {
        return hospitalsMap;
    }

    public Dictionary<Hex, Hex> GetSchoolsMap() {
        return schoolsMap;
    }

    public Dictionary<Hex, Hex> GetPowerPlantsMap() {
        return powerPlantsMap;
    }

    public Dictionary<Hex, Hex> GetJobCentersMap() {
        return jobCentersMap;
    }

    public Dictionary<Hex, Hex> GetPondsMap() {
        return pondsMap;
    }
    
    public Dictionary<Hex, Hex> GetVillageMap() {
        return villageMap;
    }

    public Dictionary<Hex, GameObject> GetMap() {
        return hexMap;
    }

    public void SetBuildingMap(Dictionary<Hex, Building> buildingMap) {
        buildingsMap = buildingMap;
    }
    public void SetPondsMap(Dictionary<Hex, Hex> pondMap) {
        pondsMap = pondMap;
    }
    public void SetVillageMap(Dictionary<Hex, Hex> villagseMap) {
        this.villageMap = villageMap;
    }
    
    public void SetPizzeriasMap(Dictionary<Hex, Hex> pizzeriaMap) {
        this.pizzeriasMap = pizzeriaMap;
    }
    
    public void SetHospitalsMap(Dictionary<Hex, Hex> hospitalMap) {
        this.hospitalsMap = hospitalMap;
    }
    
    public void SetSchoolsMap(Dictionary<Hex, Hex> schoolMap) {
        this.schoolsMap = schoolMap;
    }
    
    public void SetPowerPlantsMap(Dictionary<Hex, Hex> powerPlantMap) {
        this.powerPlantsMap = powerPlantMap;
    }
    
    public void SetJobCentersMap(Dictionary<Hex, Hex> jobCenterMap) {
        this.jobCentersMap = jobCenterMap;
    }
    
    public static MouseCoordinates GetInstance() {
        return instance;
    }

    public Layout GetLayout() {
        return layout;
    }

    public void SetHexMap(Dictionary<Hex, GameObject> serializedMap) {
        hexMap = serializedMap;
    }
}