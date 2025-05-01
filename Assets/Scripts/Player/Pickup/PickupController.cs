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

                float inputY = lookInput.y;
                float inputX = lookInput.x;

                if (_cameraY - inputY < 90f && _cameraY - inputY > -90f) {
                    _cameraY -= inputY;
                }

                _cameraX += inputX;

                Quaternion yaw = Quaternion.AngleAxis(_cameraX, Vector3.up);
                
                Vector3 flatRight = yaw * Vector3.right;

                Quaternion pitch = Quaternion.AngleAxis(_cameraY, flatRight);

                heldObject.transform.rotation = yaw * pitch;


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

            // Boom-out effect based on force
            float force = minFOV + (_throwForce * 2f);
            StartCoroutine(boomFOV(force));
        }

        heldObject = null;
        holding = EHoldingObject.empty;
    }

    private System.Collections.IEnumerator boomFOV(float boomFOV) {
        cam.fieldOfView = boomFOV;
    
        float duration = 0.5f;
        float elapsed  = 0f;
        float startFOV = boomFOV;
        float endFOV   = 60f;
    
        while (elapsed < duration) {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            cam.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);
        
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        cam.fieldOfView = endFOV;
    }
}