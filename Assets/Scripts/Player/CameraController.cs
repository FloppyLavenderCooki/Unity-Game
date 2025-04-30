using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CameraController : MonoBehaviour {
        private InputAction _lookAction;
    
        public float mouseSensitivity = 1f;

        [SerializeField] private Camera playerCam;

        [Range(-90f, 90f)] private float _cameraY;
        private float _cameraX;

        private void Start() {
            LockCursor();
            _lookAction = InputSystem.actions.FindAction("Look");
        }

        private void Update() {
            Vector2 lookInput = _lookAction.ReadValue<Vector2>();

            float inputY = lookInput.y * mouseSensitivity;
            float inputX = lookInput.x * mouseSensitivity;
            
            if (_cameraY - inputY < 90f && _cameraY - inputY > -90f) {
                _cameraY -= inputY;
            }

            _cameraX += inputX;

            transform.rotation = Quaternion.Euler(_cameraY, _cameraX, 0f);
        }

        public void LockCursor() {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}