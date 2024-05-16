using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexSize = 1.0f;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        var layout = new RedBlockGridConverted.Layout(
            RedBlockGridConverted.Orientation.LayoutPointy(),
            new RedBlockGridConverted.Point(hexSize, hexSize),
            new RedBlockGridConverted.Point(0, 0)
        );

        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridHeight, -q + gridHeight);
            for (int r = r1; r <= r2; r++)
            {
                RedBlockGridConverted.Hex hex = new RedBlockGridConverted.Hex(q, r, -q - r);
                CreateHex(hex, layout);
            }
        }
    }

    void CreateHex(RedBlockGridConverted.Hex hex, RedBlockGridConverted.Layout layout) {
        RedBlockGridConverted converted = new RedBlockGridConverted();
        RedBlockGridConverted.Point pos = converted.HexToPixel(layout, hex);
        // RedBlockGridConverted.Point pos = RedBlockGridConverted.HexToPixel(layout, hex);
        Vector3 position = new Vector3((float)pos.x, 0, (float)pos.y);
        Instantiate(hexPrefab, position, Quaternion.identity, this.transform);
    }
}