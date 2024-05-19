using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexSize = 1.0f;
    private RedBlockGridConverted.Layout layout;
    private Dictionary<RedBlockGridConverted.Hex, GameObject> hexMap;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        hexMap = new Dictionary<RedBlockGridConverted.Hex, GameObject>();
        GenerateGrid();
    }

     void Update() {
         if (Input.GetMouseButtonDown(0)) {
         
             Vector3 mousePos = Input.mousePosition;
             Ray ray = cam.ScreenPointToRay(mousePos);
             if (Physics.Raycast(ray, out RaycastHit hit)) {
                 RedBlockGridConverted.FractionalHex fractionalHex =
                     RedBlockGridConverted.PixelToHex(layout, new RedBlockGridConverted.Point(hit.point.x, hit.point.z));
                 RedBlockGridConverted.Hex hex = RedBlockGridConverted.HexRound(fractionalHex);
                 GameObject hexObject = hexMap[hex];
                 if (hexObject != null) {
                     Destroy(hexObject);
                     hexMap.Remove(hex);
                 }
             }
         }
     }

    void GenerateGrid()
    {
        layout = new RedBlockGridConverted.Layout(
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
                CreateHex(hex);
            }
        }
    }

    void CreateHex(RedBlockGridConverted.Hex hex) {
        RedBlockGridConverted converted = new RedBlockGridConverted();
        RedBlockGridConverted.Point pos = converted.HexToPixel(layout, hex);
        Vector3 position = new Vector3((float)pos.x, 0, (float)pos.y);
        GameObject instantiate = Instantiate(hexPrefab, position, Quaternion.identity, transform);
        hexMap[hex] = instantiate;
    }
}