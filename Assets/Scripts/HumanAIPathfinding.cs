using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RedblockGrid;

public class HumanAIPathfinding : MonoBehaviour {
    
    [SerializeField] int maxDistance = 1;
    [SerializeField] float waitBeforeNextAttempt = 60f;
    
    float moveSpeed = 40f; // Speed at which the transform moves towards the next hex
    float moveTime; // Time elapsed since the last move
    private float lastAttemptTime = 0f;
    Vector3 targetPosition = Vector3.zero;

    private MouseCoordinates mouseCoordinates;
    private Hex startingHex;
    private Hex goalHex;
    List<Hex> path;

    bool returning;

    void Start() {
        mouseCoordinates = MouseCoordinates.GetInstance();
        mouseCoordinates.GetHexFromMapNoRay(gameObject, out Hex startHex);
        startingHex = mouseCoordinates.GetMap().Keys.FirstOrDefault(k => k == startHex);
    }

    void Update() {
        if (path is null || goalHex is null) {
            if (Time.time - lastAttemptTime > waitBeforeNextAttempt) {
                
                float closestPizzeria = float.MaxValue;
                lastAttemptTime = 0f;
                foreach (Hex hex in mouseCoordinates.GetBuildingMap().Keys) {
                    if (hex.IsPizzeria)
                    {
                        var potentialPath = HexPathfinding.FindPath(startingHex, hex, mouseCoordinates.GetMap(), (_, _) => true, maxDistance);

                        if (potentialPath != null && potentialPath.Count < closestPizzeria)
                        {
                            goalHex = hex;
                            path = potentialPath;
                            closestPizzeria = potentialPath.Count;
                        }
                    }
                }

                lastAttemptTime += Time.time;
            }
            else {
                return;
            }
        }

        if (path != null && path.Count >= 0) {
            // Increment move time
            moveTime += Time.deltaTime;

            // Check if it's time to move
            if (moveTime >= 1f / moveSpeed) {
                moveTime = 0f;

                // Set the next target position if there isn't one
                if (targetPosition == Vector3.zero && path.Count > 0) {
                    Hex hex = path[0];
                    Point point = HexToPixel(mouseCoordinates.GetLayout(), hex);
                    targetPosition = new Vector3((float)point.x, 3, (float)point.y);
                    path.RemoveAt(0);
                }

                // Move towards the target position
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // Check if the target position is reached
                if (transform.position == targetPosition) {
                    targetPosition = Vector3.zero;

                    // If the path is empty after reaching the target, check if we need to return or complete the return
                    if (path.Count == 0) {
                        if (!returning) {
                            path = HexPathfinding.FindPath(goalHex, startingHex, mouseCoordinates.GetMap(),
                                (_, _) => true, maxDistance);

                            if (path == null || path.Count == 0) {
                                path = null;
                                return;
                            }

                            returning = true;
                        }
                        else {
                            path = null;
                            goalHex = null;
                            returning = false;
                        }
                    }
                }
            }
        }
    }
}