using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RedblockGrid;

public class HumanAIPathfinding : MonoBehaviour {

    [SerializeField] int maxDistance = 1;
    [SerializeField] float waitBeforeNextAttempt = 60f;

    private float moveSpeed = 40f; // Speed at which the transform moves towards the next hex
    private float moveTime; // Time elapsed since the last move
    private float lastAttemptTime;
    private Vector3 targetPosition = Vector3.zero;
    private MouseCoordinates mouseCoordinates;
    private Hex startingHex;
    private Hex goalHex;
    private List<Hex> path;
    private bool returning;
    private int startingPoint;
    private Hex previousHex;
    private Hex futureHex;
    private Hex currentHex;
    private List<Vector3> waypoints = new List<Vector3>();
        
    private int iterator = 0;
    // private List<Vector3> waypointsHelperList;

    private void Start() {
        mouseCoordinates = MouseCoordinates.GetInstance();
        mouseCoordinates.GetHexFromMapNoRay(gameObject, out Hex startHex);
        startingHex = mouseCoordinates.GetMap().Keys.FirstOrDefault(k => k == startHex);
    }

    private void Update() {
        if (path is null || goalHex is null) {
            if (Time.time - lastAttemptTime > waitBeforeNextAttempt) {
                float closestPizzeria = float.MaxValue;
                lastAttemptTime = 0f;
                foreach (Hex hex in mouseCoordinates.GetBuildingMap().Keys) {
                    if (!hex.IsPizzeria) continue;
                    var potentialPath = HexPathfinding.FindPath(startingHex, hex, mouseCoordinates.GetMap(),
                        (_, _) => true, maxDistance);

                    if (potentialPath == null || !(potentialPath.Count < closestPizzeria)) continue;
                    goalHex = hex;
                    path = potentialPath;
                    closestPizzeria = potentialPath.Count;
                }

                lastAttemptTime += Time.time;
            }
            else {
                return;
            }
        }

        if (path?.Count >= 0) {
            // Increment move time
            moveTime += Time.deltaTime;

            // Check if it's time to move
            if (!(moveTime >= 1f / moveSpeed)) return;
            moveTime = 0f;
            
            // Set the next target position if there isn't one
            if (targetPosition == Vector3.zero &&  path.Count > 0 && (waypoints.Count == 0 || iterator >= waypoints.Count)) {
                waypoints = new List<Vector3>();
                if (startingPoint == 0) {
                    // we are at the start
                    Hex hex = path[0];
                    currentHex = hex;
                    Point point = HexToPixel(mouseCoordinates.GetLayout(), hex);
                    targetPosition = new Vector3((float)point.x, 0f, (float)point.y);
                    previousHex = hex;
                    path.RemoveAt(0);
                    startingPoint++;
                }
                else {
                    // now we have access to the previous hex
                    // now we have access to the future hex
                    if (path.Count > 1) {
                        iterator = 0;
                        futureHex = path[1];
                        Hex hex = path[0];
                        currentHex = hex;
                        // TODO KUBO pizzeria hexes is NOT connected properly!!
                        waypoints = hex.GetRoad(Hex.GetDirection(hex, previousHex),
                            Hex.GetDirection(hex, futureHex));
                        if (waypoints?.Count > 0 && iterator < waypoints.Count) {
                            targetPosition = waypoints[iterator];
                            // iterator++;
                            targetPosition += currentHex.worldPosition;
                            transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                                moveSpeed * Time.deltaTime);
                        }
                        else {
                            iterator = 0;
                            Point point = HexToPixel(mouseCoordinates.GetLayout(), hex);
                            targetPosition = new Vector3((float)point.x, 0f, (float)point.y);
                        }

                        path.RemoveAt(0);
                    }
                    else {
                        // we are at the goal
                        Hex hex = path[0];
                        path.RemoveAt(0);
                        Point point = HexToPixel(mouseCoordinates.GetLayout(), hex);
                        targetPosition = new Vector3((float)point.x, 0f, (float)point.y);
                        iterator = 0;
                    }
                }
            }

            // Move towards the target position
            transform.position =
                Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if the target position is reached
            if (transform.position != targetPosition) return;
            targetPosition = Vector3.zero;
            // If there are more waypoints, set the next target position
            if (waypoints?.Count > 0
                && iterator < waypoints.Count
                ) {
                targetPosition = waypoints[iterator];
                iterator++;
                targetPosition += currentHex.worldPosition;
                return;
                // TODO KUBO return
            }

            // If the path is empty after reaching the target, check if we need to return or complete the return
            if (path.Count != 0) return;
            if (!returning) {
                path = HexPathfinding.FindPath(goalHex, startingHex, mouseCoordinates.GetMap(),
                    (_, _) => true, maxDistance);

                if (path == null || path.Count == 0) {
                    path = null;
                    return;
                }

                startingPoint = 0;
                returning = true;
                iterator = 0;
            }
            else {
                startingPoint = 0;
                path = null;
                goalHex = null;
                returning = false;
                iterator = 0;
            }
        }
    }

}