using System.Collections.Generic;
using Enums;
using UnityEngine;
using static Hex;

public class HexRangeFinder {
    private static MouseCoordinates mouseCoordinates = MouseCoordinates.GetInstance();

    public static List<Hex> GetAllBuildingsInRange(Hex centralHex, int range) {
        List<Hex> result = new List<Hex>();
        centralHex = mouseCoordinates.GetKeyFromMap(centralHex);
        if (centralHex is not null) {
            result = GetAllInRange(centralHex, range);
        }

        return result;
    }

    public static List<Hex> GetOnlyConnectedInRange(Hex hex, int range) {
        List<Hex> result = new List<Hex>();
        var inRangeCoordinates = HexPathfinding.GetInRangeCoordinates(hex, range);
        foreach (var hexInRange in inRangeCoordinates) {
            Hex currentHex = mouseCoordinates.GetKeyFromMap(hexInRange);
            if (currentHex is null || currentHex == hex) continue;
            if (HexPathfinding.FindPath(hex, currentHex, mouseCoordinates.GetMap(), (_, _) => true,
                    range) is not null) {
                result.Add(currentHex);
            }
        }
        return result;
    }

    public static List<Hex> GetOnlyConnectedVillageInRange(Hex hex, int range) {
        return GetOnlyConnectedInRange(hex, range).FindAll(x => mouseCoordinates.GetVillageMap()[x] is not null);
    }
    
    private static List<Hex> GetAllInRange(Hex hex, int range) {
        List<Hex> result = new List<Hex>();
        var inRangeCoordinates = HexPathfinding.GetInRangeCoordinates(hex, range);
        foreach (var hexInRange in inRangeCoordinates) {
            Hex currentHex = mouseCoordinates.GetKeyFromMap(hexInRange);
            if (currentHex is null || currentHex == hex) continue;
            result.Add(currentHex);
        }

        return result;
    }
    public static List<Hex> GetAllConnectedHexes(Hex startHex)
    {
        List<Hex> connectedHexes = new List<Hex>();
        HashSet<Hex> visited = new HashSet<Hex>();
        Queue<Hex> queue = new Queue<Hex>();

        Hex keyHex = mouseCoordinates.GetKeyFromMap(startHex);
        if (keyHex is null)
        {
            return connectedHexes;
        }

        queue.Enqueue(keyHex);
        visited.Add(keyHex);

        while (queue.Count > 0)
        {
            Hex currentHex = queue.Dequeue();
            connectedHexes.Add(currentHex);

            var neighbors = HexPathfinding.GetNeighborsConnected(currentHex, mouseCoordinates.GetMap());

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return connectedHexes;
    }

    public static List<Hex> GetAllConnectedByType(Hex startHex, BuildingType buildingType) {
        return GetAllConnectedHexes(startHex).FindAll(x => x.GetHexProperties().GetBuildingType() == buildingType);
        
    }
}