using System.Collections.Generic;
using System.Linq;
using Enums;
using StructureBuilding;
using UnityEngine;

public class City {
    public int Population { get; private set; }
    public List<Building> Buildings { get; private set; }
    public float Happiness { get; private set; }
    public float SocialFeeling { get; private set; }

    public City(int initialPopulation) {
        Population = initialPopulation;
        Buildings = new List<Building>();
    }

    public void UpdateCityStatus() {
        Buildings = MouseCoordinates.GetInstance().GetBuildingMap().Values.ToList();
        CalculateHappiness();
        CalculateSocialFeeling();
        UpdatePopulation();
    }

    private void CalculateHappiness() {
        int uniqueConnectedToPizzeriasCount = 0;
        int connectedToSchools = 0;
        int connectedToJobCenter = 0;
        int connectedToPonds = 0;
        int totalAmountOfVIllageBuildings = MouseCoordinates.GetInstance().GetVillageMap().Count;
        if (totalAmountOfVIllageBuildings == 0) return;
        
        var pizzeriasConnections = new List<Hex>();
        var schoolsConnections = new List<Hex>();
        var jobCentersConnections = new List<Hex>();
        var pondsConnections = new List<Hex>();
        
        foreach (var pizzeria in MouseCoordinates.GetInstance().GetPizzeriasMap().Values) {
            pizzeriasConnections.AddRange(pizzeria.GetHexProperties().GetConnectedHexesInRange());
        }
        var uniquePizzaVillageConnections = new HashSet<Hex>(pizzeriasConnections);
        uniqueConnectedToPizzeriasCount = uniquePizzaVillageConnections.Count;

        foreach (var school in MouseCoordinates.GetInstance().GetSchoolsMap().Values) {
            schoolsConnections.AddRange(school.GetHexProperties().GetConnectedHexesInRange());
        }
        var uniqueSchoolsConnections = new HashSet<Hex>(schoolsConnections);
        connectedToSchools = uniqueSchoolsConnections.Count;

        foreach (var jobCenter in MouseCoordinates.GetInstance().GetJobCentersMap().Values) {
            jobCentersConnections.AddRange(jobCenter.GetHexProperties().GetConnectedHexesInRange());
        }
        var uniqueJobCenterConnections = new HashSet<Hex>(jobCentersConnections);
        connectedToJobCenter = uniqueJobCenterConnections.Count;

        foreach (var pond in MouseCoordinates.GetInstance().GetPondsMap().Values) {
            pondsConnections.AddRange(pond.GetHexProperties().GetConnectedHexesInRange());
        }
        var uniquePondsConnections = new HashSet<Hex>(pondsConnections);
        connectedToPonds = uniquePondsConnections.Count;

        Happiness = (uniqueConnectedToPizzeriasCount / totalAmountOfVIllageBuildings * 1.5f) +
                    (connectedToSchools / totalAmountOfVIllageBuildings * 2.0f) +
                    (connectedToJobCenter / totalAmountOfVIllageBuildings * 1.0f) +
                    (connectedToPonds / totalAmountOfVIllageBuildings * 1.0f);
        Debug.Log("Unique connected to pizzerias: " + uniqueConnectedToPizzeriasCount);
        Debug.Log("Connected to schools: " + connectedToSchools);
        Debug.Log("Connected to job centers: " + connectedToJobCenter);
        Debug.Log("Connected to ponds: " + connectedToPonds);
        Debug.Log("Total amount of village buildings: " + totalAmountOfVIllageBuildings);
        
        Debug.Log("Happiness: " + Happiness);
    }

    private void CalculateSocialFeeling() {
        // int connectedVillages = HexRangeFinder.GetOnlyConnectedInRange(BuildingType.Village).Count;
        // int connectedPonds = HexRangeFinder.GetOnlyConnectedInRange(BuildingType.Pond).Count;
        // int connectedPowerPlants = HexRangeFinder.GetOnlyConnectedInRange(BuildingType.PowerPlant).Count;
        //
        // SocialFeeling = (connectedVillages * 1.0f) + (connectedPonds * 1.5f) + (connectedPowerPlants * 1.0f);
    }

    private void UpdatePopulation() {
        if (Happiness > 5 && SocialFeeling > 5) {
            Population += (int)(Population * 0.02); // 2% growth
        } else if (Happiness < 3 || SocialFeeling < 3) {
            Population -= (int)(Population * 0.02); // 2% decline
        }
    }
}