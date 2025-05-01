using UnityEngine;

public class NoLightHoldableItem : MonoBehaviour {
    private Vector3 respawnPos;
    
    void Awake() {
        respawnPos = transform.position;
    }

    void Update() {
        if (transform.position.y < -50f) {
            transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = respawnPos;
            transform.rotation = Quaternion.identity;
        }
    }
}
