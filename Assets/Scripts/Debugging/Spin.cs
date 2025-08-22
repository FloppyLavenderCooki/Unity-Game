using UnityEngine;

public class Spin : MonoBehaviour {
    void FixedUpdate() {
        transform.Rotate(Vector3.up, 10);
    }
}
