using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Button building1;
    [SerializeField] private Button building2;
    [SerializeField] private Button cancelPreview;
    [SerializeField] private GameObject previewContainer;
    [SerializeField] private GameObject prefabToPreview1;
    [SerializeField] private GameObject prefabToPreview2;

    private GameObject previewInstance;
    private bool previewing = false;

    private void Awake() {
        building1.onClick.AddListener(() => {
            if (previewInstance != null) Destroy(previewInstance);
            ShowPreview(prefabToPreview1);
        });        
        building2.onClick.AddListener(() => {
            if (previewInstance != null) Destroy(previewInstance);
            ShowPreview(prefabToPreview2);
        });
        cancelPreview.onClick.AddListener(() => {
            previewing = false;
        });
    }

    private void Update() {
        if (!previewing) {
            if (previewInstance != null) Destroy(previewInstance);
        }
        else {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
        
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.transform != null && hit.transform.name != "Cube" && hit.transform.name != "Sphere" ) {
                    Debug.Log(hit.transform.name);
                    previewInstance.transform.position = hit.transform.position;
                }
            }
        }
    }

    
    
    private void ShowPreview(GameObject prefab) {
        previewing = true;

        previewInstance = Instantiate(prefab, previewContainer.transform);
        previewInstance.transform.localPosition = Vector3.zero;
        previewInstance.transform.localRotation = Quaternion.identity;
        previewInstance.transform.localScale = Vector3.one;

    }

}
