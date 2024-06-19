using System;
using System.Collections.Generic;
using System.IO;
using Enums;
using Grid;
using SOs;
using UI;
using UnityEngine;
using static Hex;

public class GameStateManager : MonoBehaviour
{
    
    [SerializeField] private PreviewBuildingSO toLoadBuildingVillageSO;
    [SerializeField] private PreviewBuildingSO toLoadBuildingPondSO;
    [SerializeField] private PreviewBuildingSO toLoadBuildingPizzeriaSO;
    [SerializeField] private PreviewBuildingSO toLoadBuildingHospitalSO;
    [SerializeField] private PreviewBuildingSO toLoadBuildingRoadMaintenanceSO;
    [SerializeField] private PreviewBuildingSO toLoadBuildingPowerPlantSO;
    [SerializeField] private GameObject baseHexPrefab;
    [SerializeField] private HexSerialization hexSerializer;
    
    private const int MAX_ROTATION = 6;
    
    private Dictionary<Hex, GameObject> hexMap;
    private Dictionary<Hex, GameObject> buildingsMap;

    private void OnEnable() {
        UIManager.OnSaveGameButtonPressed += OnSaveGameButtonPressed;
        UIManager.OnLoadGameButtonPressed += OnLoadGameButtonPressed;
    }
    
    private void OnDisable() {
        UIManager.OnSaveGameButtonPressed -= OnSaveGameButtonPressed;
        UIManager.OnLoadGameButtonPressed -= OnLoadGameButtonPressed;
    }

    private void OnLoadGameButtonPressed(object arg1, EventArgs arg2) {
        Load();
    }

    private void OnSaveGameButtonPressed(object arg1, EventArgs arg2) {
        Save();
    }

    private void Start() {
        hexSerializer = new HexSerialization();
    }

    public void Save() {
        buildingsMap = MouseCoordinates.GetInstance().GetBuildingMap();
        hexMap = MouseCoordinates.GetInstance().GetMap();
        Debug.Log(Application.persistentDataPath);
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (
            BinaryWriter writer =
            new BinaryWriter(File.Open(path, FileMode.Create))
        ) {
            writer.Write(hexMap.Count);
            foreach (var hexEntry in hexMap.Keys) {
                hexSerializer.Save(writer,hexEntry,hexEntry.GetHexProperties());
            }

            UIManager.instance.SaveStats(writer);
        }
    }

    public void Load() {
        foreach (var hex in hexMap.Values) {
            Destroy(hex);
        }

        hexMap.Clear();
        buildingsMap.Clear();

        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (
            BinaryReader reader =
            new BinaryReader(File.OpenRead(path))
        ) {
            int hexCount = reader.ReadInt32();
            for (int i = 0; i < hexCount; i++) {
                Hex hex = new Hex(0, 0, 0);
                hexSerializer.Load(reader, hex, hex.GetHexProperties());
                BuildingType buildingType = hex.GetHexProperties().GetBuildingType();
                GameObject buildingInstance;
                if (BuildingType.Basic != buildingType) {
                    PreviewBuildingSO so;
                    if (BuildingType.Village == buildingType) {
                        so = toLoadBuildingVillageSO;
                    }
                    else if (BuildingType.Pizzeria == buildingType) {
                        so = toLoadBuildingPizzeriaSO;
                    }
                    else if (BuildingType.Pond == buildingType) {
                        so = toLoadBuildingPondSO;
                    }
                    else if (BuildingType.Hospital == buildingType) {
                        so = toLoadBuildingHospitalSO;
                    }
                    else if (BuildingType.PowerPlant == buildingType) {
                        so = toLoadBuildingPowerPlantSO;
                    }
                    else {
                        so = toLoadBuildingRoadMaintenanceSO;
                    }

                    buildingInstance = Instantiate(so.prefabToBuild, hex.GetHexProperties().worldPosition,
                        Quaternion.Euler(0, hex.GetHexProperties().GetRotation() * -60, 0));
                    buildingsMap.Add(hex, buildingInstance);

                    int[] newRoads = new int[so.roads[hex.GetHexProperties().GetMultiHexDirection()].roadArray.Length];
                    for (int x = 0; x < newRoads.Length; x++) {
                        newRoads[x] = ((so.roads[hex.GetHexProperties().GetMultiHexDirection()].roadArray[x] + hex.GetHexProperties().GetRotation()) %
                                       MAX_ROTATION);
                    }

                    Dictionary<int, Dictionary<int, List<Vector3>>> waypoints =
                        new Dictionary<int, Dictionary<int, List<Vector3>>>();
                    for (int k = 0; k < 6; k++) {
                        waypoints[k] = new Dictionary<int, List<Vector3>>();
                        for (int j = 0; j < 6; j++) {
                            waypoints[k][j] = new List<Vector3>();
                        }
                    }

                    hex.GetHexWaypoints().waypoints = waypoints;
                    hex.GetHexWaypoints().SetWaypoints(so.waypoints[hex.GetHexProperties().GetMultiHexDirection()], hex.GetHexProperties().GetRotation());
                    hex.AddConnections(newRoads);
                }
                else {
                    buildingInstance = Instantiate(baseHexPrefab, hex.GetHexProperties().worldPosition, Quaternion.Euler(0, 0, 0));
                }

                hexMap.Add(hex, buildingInstance);
            }

            UIManager.instance.LoadStats(reader);
        }
    }
}