using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;

public class HexGridGenerator : MonoBehaviour {
    public static HexGridGenerator GetInstance() {
        return instance;
    }

    public static HexGridGenerator instance;

    public GameObject hexPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexSize = 1.0f;
    private RedblockGrid.Layout layout;
    private Dictionary<RedblockGrid.Hex, GameObject> hexMap;

    private void Awake() {
        instance = this;
        hexMap = new Dictionary<RedblockGrid.Hex, GameObject>();
        GenerateGrid();
    }

    void GenerateGrid() {
        layout = new RedblockGrid.Layout(
            RedblockGrid.Orientation.LayoutPointy(),
            new RedblockGrid.Point(hexSize, hexSize),
            new RedblockGrid.Point(0, 0)
        );

        for (int q = -gridWidth; q <= gridWidth; q++) {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridHeight, -q + gridHeight);
            for (int r = r1; r <= r2; r++) {
                RedblockGrid.Hex hex = new RedblockGrid.Hex(q, r, -q - r);
                hex.SetBuildingType(BuildingType.Basic);
                CreateHex(hex);
            }
        }
    }

    private void CreateHex(RedblockGrid.Hex hex) {
        if (hexMap.ContainsKey(hex)) return;
        RedblockGrid.Point pos = RedblockGrid.HexToPixel(layout, hex);
        Vector3 position = new Vector3((float)pos.x, 0, (float)pos.y);
        GameObject instantiate = Instantiate(hexPrefab, position, Quaternion.identity, transform);
        hex.worldPosition = position;
        hexMap[hex] = instantiate;
    }

    public RedblockGrid.Layout GetLayout() {
        return layout;
    }

    public Dictionary<RedblockGrid.Hex, GameObject> GetHexMap() {
        return hexMap;
    }

    void Update() {
        if ( !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // TODO KUBO we now need to restrict the building to the grid only, because it can build upon already built building 
                // if pointers is far enough 
            if (Physics.Raycast(ray, out hit)) {
                Vector3 clickPosition = hit.point;
                TryExpandGridAtPosition(clickPosition);
            }
        }
    }

    void TryExpandGridAtPosition(Vector3 position) {
        RedblockGrid.Point point = new RedblockGrid.Point(position.x, position.z);
        RedblockGrid.FractionalHex fractionalHex = RedblockGrid.PixelToHex(layout, point);
        RedblockGrid.Hex clickedHex = RedblockGrid.HexRound(fractionalHex);
        RedblockGrid.Hex nearestHex = GetNearestExistingHex(clickedHex);

        if (nearestHex is not null && !hexMap.ContainsKey(clickedHex)) {
            CreateHex(clickedHex);
        }
    }

    RedblockGrid.Hex GetNearestExistingHex(RedblockGrid.Hex hex) {
        RedblockGrid.Hex[] neighbors = {
            
            new(hex.q_ + 1, hex.r_, hex.s_ - 1),
            new(hex.q_ - 1, hex.r_, hex.s_ + 1),
            
            new(hex.q_, hex.r_ + 1, hex.s_ - 1),
            new(hex.q_, hex.r_ - 1, hex.s_ + 1),
            new(hex.q_ + 1, hex.r_ - 1, hex.s_),
            new(hex.q_ - 1, hex.r_ + 1, hex.s_)
        };

        foreach (var neighbor in neighbors) {
            if (hexMap.ContainsKey(neighbor)) {
                return neighbor;
            }
        }
        return null;
    }
}