using UnityEngine;

public class HoldableItem : MonoBehaviour {
    private Vector3 _respawnPos;
    
    void Awake() {
        _respawnPos = transform.position;
    }

    void Update() {
        if (transform.position.y < -50f) {
            transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = _respawnPos;
            transform.rotation = Quaternion.identity;
        }
    }
}
