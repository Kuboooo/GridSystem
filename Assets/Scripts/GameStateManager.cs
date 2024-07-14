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

    private const int MAX_ROTATION = 6;

    private Dictionary<Hex, GameObject> hexMap;


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

        UIManager.instance.SaveStats(writer);
    }

    private void Load() {
        foreach (var hex in hexMap.Values) {
            Destroy(hex);
        }

        hexMap.Clear();
        Dictionary<Hex, Building> buildingsMap = new Dictionary<Hex, Building>();
        Dictionary<Hex, Hex> hospitalsMap = new Dictionary<Hex, Hex>();
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