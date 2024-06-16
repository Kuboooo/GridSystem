using System.Collections.Generic;
using UnityEngine;
using static RedblockGrid;

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

    private static List<Hex> GetAllInRange(Hex hex, int range) {
        List<Hex> result = new List<Hex>();
        var inRangeCoordinates = HexPathfinding.GetInRangeCoordinates(hex, range);
        foreach (var hexInRange in inRangeCoordinates) {
            Hex currentHex = mouseCoordinates.GetKeyFromMap(hexInRange);
            if (currentHex is null || currentHex == hex) continue;
            result.Add(currentHex);
        }

        Debug.Log("Results count: " + result.Count);
        return result;
    }
}