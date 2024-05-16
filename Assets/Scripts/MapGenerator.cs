using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] GameObject tilePrefab;
    [SerializeField] int mapWidth = 10;
    [SerializeField] int mapHeight = 10;
    [SerializeField] int tileSize = 1;
    
    private List<GameObject> tiles = new List<GameObject>();
    

    public void Start() {
        MakeMapGrid();
    }    

    private Vector2 GetHexCoords(int x, int z) {
        float xPos = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float zPos = z * tileSize + (x % 2 == 1 ? tileSize * 0.5f : 0);
        
        return new Vector2(xPos, zPos);
    }
    
    private void MakeMapGrid() {
        for (int x = 0; x < mapWidth; x++) {
            for (int z = 0; z < mapHeight; z++) {
                
                Vector2 pos = GetHexCoords(x, z);
                
                Vector3 position = new Vector3(pos.x * tileSize, 0, pos.y * tileSize);
                
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tiles.Add(tile);
            }
        }
    }
}
