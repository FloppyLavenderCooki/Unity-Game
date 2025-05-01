using System.Threading.Tasks;
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
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                
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
    }
    
    private System.Collections.IEnumerator releaseObject(GameObject obj) {
        float time = 0f;

        while (_grabObject.IsPressed()) {
            time += Time.deltaTime;
            yield return null;
        }

        if (time < 0.3f) {
            // release
            heldObject.transform.parent = bookParent;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject.transform.GetComponent<Collider>().enabled = true;
        } else {
            // calculate throw force < x
            time = time - 0.3f;
            _throwForce = time * throwForceMax;
            if (_throwForce >= throwForceMax) {
                _throwForce = throwForceMax;
            }
            heldObject.transform.parent = bookParent;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject.transform.GetComponent<Collider>().enabled = true;
            heldObject.GetComponent<Rigidbody>().AddForce(cam.transform.forward * _throwForce, ForceMode.Impulse);
        }
        heldObject = null;
        holding = EHoldingObject.empty;
    }
}