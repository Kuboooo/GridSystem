using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

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
                CreateHex(hex);
            }
        }
    }

    void CreateHex(RedblockGrid.Hex hex) {
        RedblockGrid.Point pos = RedblockGrid.HexToPixel(layout, hex);
        Vector3 position = new Vector3((float)pos.x, 0, (float)pos.y);
        GameObject instantiate = Instantiate(hexPrefab, position, Quaternion.identity, transform);
        hexMap[hex] = instantiate;
    }

    public RedblockGrid.Layout GetLayout() {
        return layout;
    }

    public Dictionary<RedblockGrid.Hex, GameObject> GetHexMap() {
        return hexMap;
    }
}