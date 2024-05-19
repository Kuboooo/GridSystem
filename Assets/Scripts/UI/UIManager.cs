using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static event Action<GameObject> OnPreviewingBuilding;
    public static event Action<object, EventArgs> OnStopPreviewing;

    public static void PreviewBuilding(GameObject buildingPrefab)
    {
        OnPreviewingBuilding?.Invoke(buildingPrefab);
    }

    public static void StopPreviewing()
    {
        OnStopPreviewing?.Invoke(null, EventArgs.Empty);
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Button building1;
    [SerializeField] private Button building2;
    [SerializeField] private Button cancelPreview;
    [SerializeField] private TextMeshProUGUI buildingsCount;
    // TODO KUBO pull these somehow automatically out of some SOlist
    [SerializeField] private PreviewBuildingSO previewBuildingSO;
    [SerializeField] private PreviewBuildingSO previewBuildingSphereSO;

    private GameObject previewInstance;
    private bool previewing = false;

    private void Awake() {
        building1.onClick.AddListener(() => {
            if (previewInstance != null) Destroy(previewInstance);
            ShowPreview(previewBuildingSO.prefabToPreview.GameObject());
        });        
        building2.onClick.AddListener(() => {
            if (previewInstance != null) Destroy(previewInstance);
            ShowPreview(previewBuildingSphereSO.prefabToPreview.GameObject());
        });
        cancelPreview.onClick.AddListener(StopPreviewing);
        buildingsCount.text = "0";
    }

    private void Update() {
        if (!previewing) {
            if (previewInstance != null) Destroy(previewInstance);
        }
        else {
            if (Input.GetKeyDown(KeyCode.Q) && previewInstance != null) {
                previewInstance.transform.rotation *= Quaternion.Euler(0, 60, 0);                
            }
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
        
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.transform != null && hit.transform.name != "Cube" && hit.transform.name != "Sphere") {
                    Debug.Log(hit.transform.name);
                    previewInstance.transform.position = hit.transform.position;
                    
                    // todo kubo this needs rework - the find should be searching only for 1  for all buildings so buildings need to somehow flag the tile that it's there
                    if (Input.GetMouseButtonDown(0) && previewInstance != null  && hit.transform.Find("Building1Preview(Clone)(Clone)") == null && hit.transform.Find("Building2Preview(Clone)(Clone)") == null)   {
                        GameObject newBuilding = previewInstance;
                        GameObject buildingInstance = Instantiate(newBuilding, hit.transform.position, previewInstance.transform.rotation);
                        buildingInstance.transform.parent = hit.transform;
                        
                        buildingsCount.text = Int32.Parse(buildingsCount.text) + 1 + "";
                        previewing = false;
                    }
                }
            }

        }
    }
    
    
    private void ShowPreview(GameObject prefab) {
        PreviewBuilding(prefab);
    }

}
