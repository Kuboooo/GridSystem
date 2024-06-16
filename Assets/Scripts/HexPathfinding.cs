using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RedblockGrid;

public class HexPathfinding {
    public static List<RedblockGrid.Hex> FindPath(RedblockGrid.Hex start, RedblockGrid.Hex goal, Dictionary<RedblockGrid.Hex, GameObject> hexMap, Func<RedblockGrid.Hex, RedblockGrid.Hex, bool> isWalkable, int maxDistance) {
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
            var neighbors = GetNeighborsConnected(current, hexMap);
            foreach (var neighbor in GetNeighborsConnected(current, hexMap)) {
                if (!isWalkable(current, neighbor)) {
                    continue;
                }
                float tentativeGScore = gScore[current] + 1;
                if (tentativeGScore > maxDistance)
                {
                    continue;
                }

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
                if (hexWithConnections is null) continue;
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

    public static List<Hex>  GetInRangeCoordinates(Hex initialHex, int range) {
        var results = new List<Hex>();
        for (int q = -range; q <= range; q++)
        {
            int r1 = Math.Max(-range, -q - range);
            int r2 = Math.Min(range, -q + range);
            for (int r = r1; r <= r2; r++)
            {
                int s = -q - r;
                results.Add(Hex.HexAdd(initialHex, new Hex(q, r, s)));
            }
        }

        return results;
    }
}
