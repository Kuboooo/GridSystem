using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PreviewBuildingSO", menuName = "ScriptableObjects/PreviewBuildingSO", order = 1)]
public class PreviewBuildingSO : ScriptableObject
{
    public GameObject prefabToPreview;
    public GameObject prefabToBuild;
    public int basePopulationGrowth;
    public int tileSize;
    public int cost;
    public List<Roads> roads;
}
