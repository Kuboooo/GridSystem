using System.Collections.Generic;
using UnityEngine;

namespace Grid {
    public class HexWaypoints {
        
        public Dictionary<int, Dictionary<int, List<Vector3>>> waypoints;

        public void SetWaypoints(Dictionary<int, Dictionary<int, List<Vector3>>> roads, int rotation) {
            if (roads == null) return;

            for (int i = 0; i < 6; i++) {
                if (!roads.ContainsKey(i)) continue;

                for (int j = 0; j < 6; j++) {
                    if (!roads[i].ContainsKey(j)) continue;

                    foreach (Vector3 road in roads[i][j]) {
                        Vector3 rotatedVector = RotateVectorAroundPoint(road, rotation * -60);
                        waypoints[(i + rotation) % 6][(j + rotation) % 6].Add(rotatedVector);
                    }
                }
            }
        }

        private Vector3 RotateVectorAroundPoint(Vector3 vector, float angle) {
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rotatedVector = rotation * vector;

            // Round off the rotated vector to ensure precision
            rotatedVector.y = 0;
            rotatedVector.x = Mathf.Round(rotatedVector.x * 100f) / 100f;
            rotatedVector.z = Mathf.Round(rotatedVector.z * 100f) / 100f;

            return rotatedVector;
        }

        public void AddRoad(int fromDirection, int toDirection, List<Vector3> waypoints) {
            this.waypoints[fromDirection][toDirection] = waypoints;
        }

        public List<Vector3> GetRoad(int fromDirection, int toDirection) {
            return waypoints.GetValueOrDefault(fromDirection)?.GetValueOrDefault(toDirection);
        }
    }
}