using System.Threading.Tasks;
using FMODUnity;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupController : MonoBehaviour {
    public GameObject player;
    public Camera cam;
    public EHoldingObject holding;

    public Transform objectHold;
    
    public float pickupDistMax = 5f;
    
    public float throwForceMax = 10f;
    private float _throwForce;

    private InputAction _grabObject;
    private InputAction _rotateObject;
    
    private InputAction _lookAction;
    
    [Range(-90f, 90f)] private float _cameraY;
    private float _cameraX;

    public CameraController camCon;
    
    
    public GameObject heldObject = null;

    public Transform bookParent;


    void Start() {
        _grabObject = InputSystem.actions.FindAction("ClickAction");
        _rotateObject = InputSystem.actions.FindAction("RotateHolding");
        _lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update() {
        if (_rotateObject.IsPressed()) {
            if (heldObject != null) {
                camCon.ToggleCameraMove(true);
                Vector2 lookInput = _lookAction.ReadValue<Vector2>();

                float inputY = -lookInput.y;
                float inputX = lookInput.x;
                
                Quaternion yaw = Quaternion.AngleAxis(inputX, cam.transform.up);
                Quaternion pitch = Quaternion.AngleAxis(inputY, cam.transform.right);

                Quaternion rotationDelta = yaw * pitch;

                heldObject.transform.rotation = rotationDelta * heldObject.transform.rotation;
            }
        } else {
            camCon.ToggleCameraMove(false);
        }
        
        if (_grabObject.WasPressedThisFrame()) {
            camCon.ToggleCameraMove(false);
            if (holding == EHoldingObject.empty) {
                // grab
                var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                
                if (Physics.Raycast(ray, out RaycastHit hit, pickupDistMax)) {
                    if (hit.collider.gameObject.transform.parent == bookParent) {
                        holding = EHoldingObject.holding;
                        
                        heldObject = hit.collider.gameObject;
                        heldObject.GetComponent<Collider>().enabled = false;
                        heldObject.transform.parent = objectHold;
                        heldObject.GetComponent<Rigidbody>().isKinematic = true;
                        heldObject.transform.position = objectHold.position;
                    }
                }
            } else {
                StartCoroutine(releaseObject(heldObject));
            }

            if (holding == EHoldingObject.holding) {
                heldObject.transform.position = objectHold.position;
                heldObject.transform.rotation = objectHold.rotation;
            }
        }
        
        if (heldObject != null) {
            bool isInContactWithWall = false;
            Vector3 direction = heldObject.transform.forward;

            if (Physics.Raycast(heldObject.transform.position, direction, out RaycastHit hit, 0.3f)) {
                isInContactWithWall = true;
            } else {
                isInContactWithWall = false;
            }

            if (isInContactWithWall) {
                heldObject.transform.position = Vector3.MoveTowards(heldObject.transform.position, transform.position, 2f * Time.deltaTime);
            }
            
            else if (Vector3.Distance(heldObject.transform.position, objectHold.position) > 0.1f) {
                heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, objectHold.position, 2f * Time.deltaTime);
            }
        }
    }
    
    private System.Collections.IEnumerator releaseObject(GameObject obj) {
        float time = 0f;
        float minFOV = 60f;
        float maxZoom = 10f; // How much to zoom in (lower FOV)
    
        // Zoom in while holding
        while (_grabObject.IsPressed()) {
            time += Time.deltaTime;

            float holdProgress = Mathf.Clamp01((time - 0.3f) / 1f); // normalise hold time past 0.3s
            float targetFOV = minFOV - (holdProgress * maxZoom);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, 10f * Time.deltaTime);

            yield return null;
        }

        if (time < 0.3f) {
            // Light release
            heldObject.transform.parent = bookParent;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject.transform.GetComponent<Collider>().enabled = true;
        } else {
            // Calculate throw force
            time -= 0.3f;
            _throwForce = Mathf.Min(time * throwForceMax, throwForceMax);

            heldObject.transform.parent = bookParent;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            heldObject.GetComponent<Collider>().enabled = true;
            rb.AddForce(cam.transform.forward * _throwForce, ForceMode.Impulse);
        }

        heldObject = null;
        holding = EHoldingObject.empty;
    }
    
    private float _bobTimer = 0f;
    void HandleObjectBobbing() {
        // Get flat movement magnitude (ignore vertical)
        Vector3 horizontalVelocity = new Vector3(heldObject.GetComponent<Rigidbody>().linearVelocity.x, 0f, heldObject.GetComponent<Rigidbody>().linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed > 0.1f) {
            _bobTimer += Time.deltaTime * 3f * (speed * 1f);
            float bobOffsetY = Mathf.Sin(_bobTimer) * 0.05f;
            float bobOffsetX = Mathf.Cos(_bobTimer * 0.5f) * 0.05f * 0.5f;

            // heldObject.transform.localPosition = _cameraInitialLocalPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
        } else {
            _bobTimer = 0f;
            heldObject.transform.localPosition = Vector3.Lerp(
                heldObject.transform.localPosition,
                heldObject.transform.position,
                Time.deltaTime * 5f
            );
        }
    }
}