using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Linq;
using static RedblockGrid;

public class HumanAIPathfinding : MonoBehaviour {

    float moveSpeed = 10f; // Speed at which the transform moves towards the next hex
    float moveTime = 0f; // Time elapsed since the last move
    Vector3 targetPosition = Vector3.zero;

    private MouseCoordinates mouseCoordinates;
    private RedblockGrid.Hex startingHex;
    private RedblockGrid.Hex goalHex;
    private Dictionary<RedblockGrid.Hex, GameObject> buildingsMap;
    List<RedblockGrid.Hex> path;
    bool returning = false;
    // Start is called before the first frame update
    void Start() {
        mouseCoordinates = MouseCoordinates.GetInstance();
        
    }

    // Update is called once per frame
    void Update() {
        buildingsMap = mouseCoordinates.GetBuildingMap();
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            mouseCoordinates.GetHexFromMapNoRay(gameObject, out RedblockGrid.Hex startHex);
            startingHex = mouseCoordinates.GetMap().Keys.FirstOrDefault(k => k == startHex);
            Debug.Log("Starthex set");
        }

        if (Input.GetMouseButtonDown(1)) {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 vector3 = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(vector3);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                mouseCoordinates.GetHexFromRay(hit, out RedblockGrid.Hex endHex);
                goalHex = mouseCoordinates.GetMap().Keys.FirstOrDefault(k => k == endHex);
                Debug.Log("goalHex set");
            }
        }

        if (startingHex is not null && goalHex is not null) {
            Func<RedblockGrid.Hex, RedblockGrid.Hex, bool> isWalkable = (currentHex, neighbourHex) => {
                return true;
            };
            if (path == null) {
                path = HexPathfinding.FindPath(startingHex, goalHex, mouseCoordinates.GetMap(), isWalkable);
                Debug.Log("Looking for Path");

            }

            //if (path != null && path.Count > 0) {
            //    foreach (var hex in path) {
            //        Point point = RedblockGrid.HexToPixel(mouseCoordinates.GetLayout(), hex);
            //        transform.position = new Vector3((float)point.x, 3, (float)point.y);
            //        Debug.Log("Path found, transform moved");

            //    }
            //}
            if (path != null && path.Count >= 0) {
                if (moveTime >= 1f / moveSpeed) {
                    moveTime = 0f;
                    if (targetPosition == Vector3.zero) {
                        RedblockGrid.Hex hex = path[0];
                        Point point = RedblockGrid.HexToPixel(mouseCoordinates.GetLayout(), hex);
                        targetPosition = new Vector3((float)point.x, 3, (float)point.y);
                        path.RemoveAt(0);
                        
                        if (path.Count == 0) {
                            if (!returning) {
                                // Path to the goal completed, now reverse path to return
                                path = HexPathfinding.FindPath(goalHex, startingHex, mouseCoordinates.GetMap(), (currentHex, neighbourHex) => true);
                                returning = true;
                            } else {
                                // Return path completed
                                path = null;
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