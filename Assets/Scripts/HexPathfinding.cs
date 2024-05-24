using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexPathfinding {
    public static List<RedblockGrid.Hex> FindPath(RedblockGrid.Hex start, RedblockGrid.Hex goal, Dictionary<RedblockGrid.Hex, GameObject> hexMap, Func<RedblockGrid.Hex, RedblockGrid.Hex, int, int, bool> isWalkable) {
        var openList = new PriorityQueue<RedblockGrid.Hex>();
        var cameFrom = new Dictionary<RedblockGrid.Hex, RedblockGrid.Hex>();
        var gScore = new Dictionary<RedblockGrid.Hex, float>();
        var fScore = new Dictionary<RedblockGrid.Hex, float>();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        openList.Enqueue(start, fScore[start]);

        while (openList.Count > 0) {
            var current = openList.Dequeue();

            if (current == goal) {
                return ReconstructPath(cameFrom, current);
            }
            int negboursCount = -1;
            int negboursDirection = 6;
            var neighbors = GetNeighborsConnected(current, hexMap);
            foreach (var neighbor in GetNeighborsConnected(current, hexMap)) {
                //TODO KUBO we dont need this now at all
                negboursCount++;
                negboursDirection--;

                if (!isWalkable(current, neighbor, negboursCount, negboursDirection)) {
                    continue;
                }
                float tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                    if (!openList.Contains(neighbor)) {
                        openList.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return null; // No path found
    }

    private static float Heuristic(RedblockGrid.Hex a, RedblockGrid.Hex b) {
        return RedblockGrid.Hex.HexDistance(a, b);
    }

    private static List<RedblockGrid.Hex> GetNeighborsConnected(RedblockGrid.Hex hex, Dictionary<RedblockGrid.Hex, GameObject> hexMap) {
        var neighbors = new List<RedblockGrid.Hex>();

        for (int i = 0; i <6; i++) {
            if (hex.HasConnection(i) && hexMap.ContainsKey(RedblockGrid.Hex.HexNeighbor(hex, i))) {
                RedblockGrid.Hex hexWithConnections = hexMap.Keys.FirstOrDefault(k => k == RedblockGrid.Hex.HexNeighbor(hex, i));
                if (hexWithConnections.HasConnection((i + 3) % 6)) {
                    neighbors.Add(hexWithConnections);
                }
            }
        }

        return neighbors;
    }

    private static List<RedblockGrid.Hex> ReconstructPath(Dictionary<RedblockGrid.Hex, RedblockGrid.Hex> cameFrom, RedblockGrid.Hex current) {
        var path = new List<RedblockGrid.Hex> { current };
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
