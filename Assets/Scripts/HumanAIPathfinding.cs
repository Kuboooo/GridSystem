using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static RedblockGrid;

public class HumanAIPathfinding : MonoBehaviour {

    float moveSpeed = 40f; // Speed at which the transform moves towards the next hex
    float moveTime; // Time elapsed since the last move
    Vector3 targetPosition = Vector3.zero;

    private MouseCoordinates mouseCoordinates;
    private Hex startingHex;
    private Hex goalHex;
    private Dictionary<Hex, GameObject> buildingsMap;
    List<Hex> path;
    bool returning;
    // Start is called before the first frame update
    void Start() {
        mouseCoordinates = MouseCoordinates.GetInstance();
        mouseCoordinates.GetHexFromMapNoRay(gameObject, out Hex startHex);
        startingHex = mouseCoordinates.GetMap().Keys.FirstOrDefault(k => k == startHex);
        Debug.Log("Starthex set");
    }

    void Update() {
        buildingsMap = mouseCoordinates.GetBuildingMap();

        if (Input.GetMouseButtonDown(1)) {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 vector3 = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(vector3);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                mouseCoordinates.GetHexFromRay(hit, out Hex endHex);
                goalHex = mouseCoordinates.GetMap().Keys.FirstOrDefault(k => k == endHex);
                Debug.Log("goalHex set");
            }
        }

        if (startingHex is not null && goalHex is not null) {
            Func<Hex, Hex, bool> isWalkable = (_, _) => {
                return true;
            };
            if (path == null) {
                path = HexPathfinding.FindPath(startingHex, goalHex, mouseCoordinates.GetMap(), isWalkable);
                Debug.Log("Looking for Path");

            }
            
            if (path != null && path.Count >= 0) {
                if (moveTime >= 1f / moveSpeed) {
                    moveTime = 0f;
                    if (targetPosition == Vector3.zero) {
                        Hex hex = path[0];
                        Point point = HexToPixel(mouseCoordinates.GetLayout(), hex);
                        targetPosition = new Vector3((float)point.x, 3, (float)point.y);
                        path.RemoveAt(0);
                        // TODO KUBO, they sometimes decide to skip the path and move straight
                        if (path.Count == 0) {
                            if (!returning) {
                                // Path to the goal completed, now reverse path to return
                                path = HexPathfinding.FindPath(goalHex, startingHex, mouseCoordinates.GetMap(), (_, _) => true);
                                returning = true;
                            } else {
                                // Return path completed
                                path = null;
                                returning = false;
                            }
                        }
                    }
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                    if (transform.position == targetPosition) {
                        targetPosition = Vector3.zero;
                    }
                } else {
                    moveTime += Time.deltaTime;
                }
            }
        }
    }
    
}