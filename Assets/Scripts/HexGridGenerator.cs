using System;
using System.Collections.Generic;
using Enums;
using Grid;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using static Hex;

public class HexGridGenerator : MonoBehaviour {
    public static HexGridGenerator GetInstance() {
        return instance;
    }

    public static HexGridGenerator instance;

    public GameObject hexPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexSize = 1.0f;
    private Layout layout;
    private Dictionary<Hex, GameObject> hexMap;

    private void Awake() {
        instance = this;
        hexMap = new Dictionary<Hex, GameObject>();
        GenerateGrid();
    }

    void GenerateGrid() {
        layout = new Layout(
            Orientation.LayoutPointy(),
            new Point(hexSize, hexSize),
            new Point(0, 0)
        );

        for (int q = -gridWidth; q <= gridWidth; q++) {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridHeight, -q + gridHeight);
            for (int r = r1; r <= r2; r++) {
                Hex hex = new Hex(q, r, -q - r);
                hex.GetHexProperties().SetBuildingType(BuildingType.Basic);
                CreateHex(hex);
            }
        }
    }

    private void CreateHex(Hex hex) {
        if (hexMap.ContainsKey(hex)) return;
        Point pos = HexToPixel(layout, hex);
        Vector3 position = new Vector3((float)pos.x, 0, (float)pos.y);
        GameObject instantiate = Instantiate(hexPrefab, position, Quaternion.identity, transform);
        hex.GetHexProperties().worldPosition = position;
        hexMap[hex] = instantiate;
    }

    public Layout GetLayout() {
        return layout;
    }

    public Dictionary<Hex, GameObject> GetHexMap() {
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
        Point point = new Point(position.x, position.z);
        FractionalHex fractionalHex = PixelToHex(layout, point);
        Hex clickedHex = HexRound(fractionalHex);
        Hex nearestHex = GetNearestExistingHex(clickedHex);

        if (nearestHex is not null && !hexMap.ContainsKey(clickedHex)) {
            CreateHex(clickedHex);
        }
    }

    Hex GetNearestExistingHex(Hex hex) {
        Hex[] neighbors = {
            
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