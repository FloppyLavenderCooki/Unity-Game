using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupController : MonoBehaviour {
    public GameObject player;
    public Camera cam;
    public EHoldingObject holding;

    public Transform objectHold;
    
    public float pickupDistMax = 5f;
    
    public float throwForceMax = 10f;
    private float throwForce;

    private InputAction grabObject;
    private InputAction useObject;

    public GameObject heldObject;
    
    private int noCastLayer;
    private int layerMask;

    public Transform bookParent;


    void Start() {
        noCastLayer = LayerMask.NameToLayer("NoCast");
        layerMask = ~(1 << noCastLayer);
        
        grabObject = InputSystem.actions.FindAction("ClickAction");
    }

    void Update() {
        if (grabObject.WasPressedThisFrame()) {
            if (holding == EHoldingObject.empty) {
                // grab
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out RaycastHit hit, pickupDistMax, layerMask)) {
                    if (hit.collider.gameObject.transform.parent == bookParent) {
                        holding = EHoldingObject.holding;
                        
                        heldObject = hit.collider.gameObject;
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

        while (grabObject.IsPressed()) {
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
            throwForce = time * throwForceMax;
            if (throwForce >= throwForceMax) {
                throwForce = throwForceMax;
            }
            heldObject.transform.parent = bookParent;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject.transform.GetComponent<Collider>().enabled = true;
            heldObject.GetComponent<Rigidbody>().AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        }
        heldObject = null;
        holding = EHoldingObject.empty;
    }
}