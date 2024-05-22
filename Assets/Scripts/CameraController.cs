using UnityEngine;
using UnityEngine.Android;

public class CameraController : MonoBehaviour {

    public Transform cameraTransform;

    [SerializeField] private float fastSpeed = 3f;
    [SerializeField] private float normalSpeed = 0.5f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float moveTime = 10f;
    [SerializeField] float rotationAmount = 1;
    [SerializeField] Vector3 zoomAmount;


    [SerializeField] private Quaternion newRotation;
    [SerializeField] private Vector3 newPosition;
    [SerializeField] private Vector3 newZoom;

    [SerializeField] private Vector3 dragStartPosition;
    [SerializeField] private Vector3 dragCurrentPosition;

    [SerializeField] private Vector3 rotationStartPosition;
    [SerializeField] private Vector3 rotationCurrentPosition;

    private void Start() {

        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    private void HandleMouseInput() {
        // todo kubo handle min/max scroll and movement
        if (Input.mouseScrollDelta.y != 0) {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if (Input.GetMouseButtonDown(0)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray, out entry)) {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry)) {
                dragCurrentPosition = ray.GetPoint(entry);
                
                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        if (Input.GetMouseButtonDown(2)) {
            rotationStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2)) {
            rotationCurrentPosition = Input.mousePosition;
            Vector3 difference = rotationStartPosition - rotationCurrentPosition;
            rotationStartPosition = rotationCurrentPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }

    private void handleMovement() {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            moveSpeed = fastSpeed;
        } else {
            moveSpeed = normalSpeed;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            newPosition += transform.forward * moveSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            newPosition -= transform.forward * moveSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            newPosition += transform.right * moveSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            newPosition -= transform.right * moveSpeed;
        }

        if (Input.GetKey(KeyCode.Q)) {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E)) {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        if (Input.GetKey(KeyCode.R)) {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F)) {
            newZoom -= zoomAmount;
        }


        transform.position = Vector3.Lerp(transform.position, newPosition, moveTime * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, moveTime * Time.deltaTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, moveTime * Time.deltaTime);

    }

}
