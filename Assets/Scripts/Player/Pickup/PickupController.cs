using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupController : MonoBehaviour {
    public GameObject player;
    public Camera cam;
    public Camera outlineCam;
    public EHoldingObject holding;

    public Transform objectHold;
    
    public float pickupDistMax = 5f;
    public float throwForceMax = 10f;
    private float _throwForce;

    private InputAction _grabObject;
    private InputAction _rotateObject;
    private InputAction _lookAction;
    
    public CameraController camCon;
    public GameObject heldObject = null;
    public Transform bookParent;

    void Start() {
        _grabObject = InputSystem.actions.FindAction("ClickAction");
        _rotateObject = InputSystem.actions.FindAction("RotateHolding");
        _lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update() {
        bool rotating = _rotateObject.IsPressed() && heldObject != null;
        bool shouldBlockCam = camCon.gamePaused || rotating;

        if (camCon.IsBlocked != shouldBlockCam) {
            camCon.ToggleCameraMove(shouldBlockCam);
        }

        if (rotating) {
            Vector2 lookInput = _lookAction.ReadValue<Vector2>();

            float inputY = lookInput.y;
            float inputX = -lookInput.x;
        
            Quaternion yaw = Quaternion.AngleAxis(inputX, cam.transform.up);
            Quaternion pitch = Quaternion.AngleAxis(inputY, cam.transform.right);
            Quaternion rotationDelta = yaw * pitch;

            heldObject.transform.rotation = rotationDelta * heldObject.transform.rotation;
        }
    
        if (_grabObject.WasPressedThisFrame()) {
            // camCon.ToggleCameraMove(false);
            if (holding == EHoldingObject.empty) {
                var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            
                if (Physics.Raycast(ray, out RaycastHit hit, pickupDistMax)) {
                    if (hit.collider.gameObject.transform.parent.parent == bookParent) {
                        holding = EHoldingObject.holding;
                    
                        heldObject = hit.collider.gameObject;
                        heldObject.GetComponent<Collider>().enabled = false;
                        heldObject.transform.parent.parent = objectHold;
                        heldObject.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        heldObject.transform.parent.position = objectHold.position;
                        
                        heldObject.layer = LayerMask.NameToLayer("OutlineTarget");
                        foreach (Transform child in heldObject.transform) {
                            child.gameObject.layer = LayerMask.NameToLayer("OutlineTarget");
                        }
                    }
                }
            } else {
                StartCoroutine(releaseObject(heldObject));
            }

            if (holding == EHoldingObject.holding) {
                heldObject.transform.position = objectHold.position;
            }
        }

        if (heldObject != null) {
            if (Vector3.Distance(heldObject.transform.position, objectHold.position) > 0.1f) {
                heldObject.transform.position = objectHold.position;
            }
        }
    }
    
    private System.Collections.IEnumerator releaseObject(GameObject obj) {
        float time = 0f;
        float minFOV = 60f;
        float maxZoom = 10f;
    
        while (_grabObject.IsPressed()) {
            time += Time.deltaTime;

            float holdProgress = Mathf.Clamp01((time - 0.3f) / 1f);
            float targetFOV = minFOV - (holdProgress * maxZoom);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, 10f * Time.deltaTime);
            if (outlineCam) outlineCam.fieldOfView = Mathf.Lerp(outlineCam.fieldOfView, targetFOV, 10f * Time.deltaTime);

            yield return null;
        }

        if (time < 0.3f) {
            obj.transform.parent.parent = bookParent;
            obj.transform.parent.GetComponent<Rigidbody>().isKinematic = false;
            obj.transform.GetComponent<Collider>().enabled = true;
        } else {
            time -= 0.3f;
            _throwForce = Mathf.Min(time * throwForceMax, throwForceMax);

            obj.transform.parent.parent = bookParent;
            Rigidbody rb = obj.transform.parent.gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            obj.GetComponent<Collider>().enabled = true;
            rb.AddForce(cam.transform.forward * _throwForce, ForceMode.Impulse);
        }
        
        StartCoroutine(ResetFOV());
        
        obj.layer = LayerMask.NameToLayer("Default");
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer =
                LayerMask.NameToLayer(child.gameObject.name.StartsWith("Canvas") ? "UI" : "Default");
        }

        heldObject = null;
        holding = EHoldingObject.empty;

        // if (!camCon.gamePaused) {
        //     camCon.ToggleCameraMove(false);
        // }
    }
    
    private System.Collections.IEnumerator ResetFOV() {
        while (Mathf.Abs(cam.fieldOfView - 60f) > 0.01f) {
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, 60f, 40f * Time.deltaTime);
            if (outlineCam) outlineCam.fieldOfView = Mathf.MoveTowards(outlineCam.fieldOfView, 60f, 40f * Time.deltaTime);
            yield return null;
        }

        cam.fieldOfView = 60f;
        if (outlineCam) outlineCam.fieldOfView = 60f;
    }
}