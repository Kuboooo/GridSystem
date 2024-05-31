using System;
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
    private int currentWaypointIndex;
    private Hex previousHex;
    private Hex futureHex;
    private Hex currentHex;
    private List<Vector3> waypoints = new List<Vector3>();
    private bool atStart = true;

    private void Start() {
        mouseCoordinates = MouseCoordinates.GetInstance();
        mouseCoordinates.GetHexFromMapNoRay(gameObject, out Hex startHex);
        startingHex = mouseCoordinates.GetMap().Keys.FirstOrDefault(k => k == startHex);
    }

    private void Update() {
        if (path is null || goalHex is null) {
            TryFindPathToPizzeria();
        }
        else {
            MoveAlongPath();
        }
    }

    private void TryFindPathToPizzeria() {
        if (Time.time - lastAttemptTime <= waitBeforeNextAttempt) return;

        float closestPizzeriaDistance = float.MaxValue;
        lastAttemptTime = 0f;
        foreach (Hex hex in mouseCoordinates.GetBuildingMap().Keys) {
            if (!hex.IsPizzeria) continue;

            var potentialPath = HexPathfinding.FindPath(startingHex, hex, mouseCoordinates.GetMap(), (_, _) => true,
                maxDistance);

            if (potentialPath == null || potentialPath.Count >= closestPizzeriaDistance) continue;

            goalHex = hex;
            path = potentialPath;
            closestPizzeriaDistance = potentialPath.Count;
        }

        lastAttemptTime = Time.time;
    }

    private void MoveAlongPath() {
        moveTime += Time.deltaTime;

        if (moveTime < 1f / moveSpeed) return;
        moveTime = 0f;

        if (targetPosition == Vector3.zero && path.Count > 0 &&
            (waypoints?.Count == 0 || currentWaypointIndex >= waypoints?.Count)) {
            SetNextTargetPosition();
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Mathf.Approximately(MathF.Round(transform.position.x, 2), MathF.Round(targetPosition.x, 2))
            && Mathf.Approximately(MathF.Round(transform.position.z, 2), MathF.Round(targetPosition.z, 2))) {
            targetPosition = Vector3.zero;
            if (waypoints?.Count > 0 && currentWaypointIndex < waypoints.Count) {
                targetPosition = waypoints[currentWaypointIndex];
                currentWaypointIndex++;
                targetPosition += currentHex.worldPosition;
                return;
            }

            if (path.Count == 0) {
                HandlePathCompletion();
            }
        }
    }

    private void SetNextTargetPosition() {
        waypoints = new List<Vector3>();
        if (atStart) {
            // At the start of the path
            Hex hex = path[0];
            currentHex = hex;
            Point point = HexToPixel(mouseCoordinates.GetLayout(), hex);
            targetPosition = new Vector3((float)point.x, 0f, (float)point.y);
            previousHex = hex;
            path.RemoveAt(0);

            atStart = false;
        }
        else {
            // Moving along the path
            if (path.Count > 1) {
                currentWaypointIndex = 0;
                futureHex = path[1];
                Hex hex = path[0];
                currentHex = hex;
                int directionFrom = Hex.GetDirection(previousHex, hex);
                int directionTo = Hex.GetDirection(futureHex, hex);
                waypoints = hex.GetRoad(directionFrom, directionTo);
                if (waypoints?.Count > 0 && currentWaypointIndex < waypoints.Count) {
                    targetPosition = waypoints[currentWaypointIndex];
                    targetPosition += currentHex.worldPosition;
                    transform.position =
                        Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                }
                else {
                    SetHexTargetPosition(hex);
                }

                previousHex = hex;
                path.RemoveAt(0);
            }
            else {
                // At the goal
                Hex hex = path[0];
                path.RemoveAt(0);
                SetHexTargetPosition(hex);
            }
        }
    }

    private void SetHexTargetPosition(Hex hex) {
        Point point = HexToPixel(mouseCoordinates.GetLayout(), hex);
        targetPosition = new Vector3((float)point.x, 0f, (float)point.y);
        currentWaypointIndex = 0;
    }

    private void HandlePathCompletion() {
        if (!returning) {
            path = HexPathfinding.FindPath(goalHex, startingHex, mouseCoordinates.GetMap(), (_, _) => true,
                maxDistance);

            if (path == null || path.Count == 0) {
                path = null;

                atStart = true;
                return;
            }

            currentWaypointIndex = 0;
            returning = true;
        }
        else {
            currentWaypointIndex = 0;
            path = null;
            goalHex = null;
            returning = false;
        }

        atStart = true;
    }
}