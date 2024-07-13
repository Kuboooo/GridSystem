using System;
using System.Collections.Generic;
using System.IO;
using Enums;
using Grid;
using SOs;
using StructureBuilding;
using UI;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
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
    // private Dictionary<Hex, Building> buildingsMap;
    // private Dictionary<Hex, Hex> pondsMap;
    // private Dictionary<Hex, Hex> villageMap;
    //
    // private Dictionary<Hex, Hex> hospitalsMap;
    // private Dictionary<Hex, Hex> pizzeriasMap;
    // private Dictionary<Hex, Hex> schoolsMap;
    // private Dictionary<Hex, Hex> powerPlantsMap;
    // private Dictionary<Hex, Hex> jobCentersMap;
    

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

    private void Save() {
        hexMap = MouseCoordinates.GetInstance().GetMap();
        Debug.Log(Application.persistentDataPath);
        var path = Path.Combine(Application.persistentDataPath, "test.map");
        using var writer =
            new BinaryWriter(File.Open(path, FileMode.Create));
        writer.Write(hexMap.Count);
        foreach (var hexEntry in hexMap.Keys) {
            HexSerialization.Save(writer, hexEntry, hexEntry.GetHexProperties());
        }
        //
        // writer.Write(buildingsMap.Count);
        // foreach (var buildingEntry in buildingsMap.Keys) {
        //     HexSerialization.Save(writer, buildingEntry, buildingEntry.GetHexProperties());
        // }
        // writer.Write(hospitalsMap.Count);
        // foreach (var hospitalEntry in hospitalsMap.Keys) {
        //     HexSerialization.Save(writer, hospitalEntry, hospitalEntry.GetHexProperties());
        // }
        // writer.Write(pizzeriasMap.Count);
        // foreach (var pizzeriaEntry in pizzeriasMap.Keys) {
        //     HexSerialization.Save(writer, pizzeriaEntry, pizzeriaEntry.GetHexProperties());
        // }
        // writer.Write(schoolsMap.Count);
        // foreach (var schoolEntry in schoolsMap.Keys) {
        //     HexSerialization.Save(writer, schoolEntry, schoolEntry.GetHexProperties());
        // }
        // writer.Write(powerPlantsMap.Count);
        // foreach (var powerPlantEntry in powerPlantsMap.Keys) {
        //     HexSerialization.Save(writer, powerPlantEntry, powerPlantEntry.GetHexProperties());
        // }
        // writer.Write(jobCentersMap.Count);
        // foreach (var jobCenterEntry in jobCentersMap.Keys) {
        //     HexSerialization.Save(writer, jobCenterEntry, jobCenterEntry.GetHexProperties());
        // }
        // writer.Write(pondsMap.Count);
        // foreach (var pondEntry in pondsMap.Keys) {
        //     HexSerialization.Save(writer, pondEntry, pondEntry.GetHexProperties());
        // }
        // writer.Write(villageMap.Count);
        // foreach (var villageEntry in villageMap.Keys) {
        //     HexSerialization.Save(writer, villageEntry, villageEntry.GetHexProperties());
        // }
        //
        UIManager.instance.SaveStats(writer);
    }

    private void Load() {
        foreach (var hex in hexMap.Values) {
            Destroy(hex);
        }

        hexMap.Clear();
        Dictionary<Hex, Building>
        buildingsMap = new Dictionary<Hex, Building>();
        Dictionary<Hex,Hex> hospitalsMap = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> pizzeriasMap = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> schoolsMap = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> powerPlantsMap = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> jobCentersMap = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> pondsMap = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> villageMap = new Dictionary<Hex, Hex>();

        var path = Path.Combine(Application.persistentDataPath, "test.map");
        using BinaryReader reader =
            new BinaryReader(File.OpenRead(path));
        var hexCount = reader.ReadInt32();
        for (var i = 0; i < hexCount; i++) {
            var hex = new Hex(0, 0, 0);
            HexSerialization.Load(reader, hex, hex.GetHexProperties());
            var buildingType = hex.GetHexProperties().GetBuildingType();
            GameObject buildingInstance;
            if (BuildingType.Basic != buildingType) {
                var so = buildingType switch {
                    BuildingType.Village => toLoadBuildingVillageSO,
                    BuildingType.Pizzeria => toLoadBuildingPizzeriaSO,
                    BuildingType.Pond => toLoadBuildingPondSO,
                    BuildingType.Hospital => toLoadBuildingHospitalSO,
                    BuildingType.PowerPlant => toLoadBuildingPowerPlantSO,
                    _ => toLoadBuildingRoadMaintenanceSO
                };

                buildingInstance = Instantiate(so.prefabToBuild, hex.GetHexProperties().worldPosition,
                    Quaternion.Euler(0, hex.GetHexProperties().GetRotation() * -60, 0));
                var b = new Building(so.buildingType, buildingInstance, hex);
                buildingsMap.Add(hex, b);

                switch (buildingType) {
                    case BuildingType.Pond:
                        pondsMap.Add(hex, hex);
                        break;
                    case BuildingType.Village:
                        villageMap.Add(hex, hex);
                        break;
                    case BuildingType.Pizzeria:
                        pizzeriasMap.Add(hex, hex);
                        break;
                    case BuildingType.Hospital:
                        hospitalsMap.Add(hex, hex);
                        break;
                    case BuildingType.PowerPlant:
                        powerPlantsMap.Add(hex, hex);
                        break;
                    case BuildingType.RoadMaintainance:
                        schoolsMap.Add(hex, hex);
                        break;
                    case BuildingType.JobCenter:
                        jobCentersMap.Add(hex, hex);
                        break;
                }

                if (buildingType == BuildingType.Hospital) {
                    hospitalsMap.Add(hex, hex);
                }
                

                var newRoads = new int[so.roads[hex.GetHexProperties().GetMultiHexDirection()].roadArray.Length];
                for (var x = 0; x < newRoads.Length; x++) {
                    newRoads[x] = (so.roads[hex.GetHexProperties().GetMultiHexDirection()].roadArray[x] +
                                   hex.GetHexProperties().GetRotation()) %
                                  MAX_ROTATION;
                }

                var waypoints =
                    new Dictionary<int, Dictionary<int, List<Vector3>>>();
                for (var k = 0; k < 6; k++) {
                    waypoints[k] = new Dictionary<int, List<Vector3>>();
                    for (var j = 0; j < 6; j++) {
                        waypoints[k][j] = new List<Vector3>();
                    }
                }

                hex.GetHexWaypoints().waypoints = waypoints;
                hex.GetHexWaypoints().SetWaypoints(so.waypoints[hex.GetHexProperties().GetMultiHexDirection()],
                    hex.GetHexProperties().GetRotation());
                hex.AddConnections(newRoads);
            }
            else {
                buildingInstance = Instantiate(baseHexPrefab, hex.GetHexProperties().worldPosition,
                    Quaternion.Euler(0, 0, 0));
            }

            hexMap.Add(hex, buildingInstance);
        }
        
        MouseCoordinates.GetInstance().SetHexMap(hexMap);
        MouseCoordinates.GetInstance().SetBuildingMap(buildingsMap);
        MouseCoordinates.GetInstance().SetPondsMap(pondsMap);
        MouseCoordinates.GetInstance().SetVillageMap(villageMap);
        MouseCoordinates.GetInstance().SetPizzeriasMap(pizzeriasMap);
        MouseCoordinates.GetInstance().SetHospitalsMap(hospitalsMap);
        MouseCoordinates.GetInstance().SetSchoolsMap(schoolsMap);
        MouseCoordinates.GetInstance().SetPowerPlantsMap(powerPlantsMap);
        MouseCoordinates.GetInstance().SetJobCentersMap(jobCentersMap);
        
        UIManager.instance.LoadStats(reader);
    }
}