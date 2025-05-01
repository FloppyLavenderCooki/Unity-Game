using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CameraController : MonoBehaviour {
        private InputAction _lookAction;
        private InputAction _cursorToggleAction;
        private InputAction _clickAction;
    
        public float mouseSensitivity = 1f;

        [SerializeField] private Camera playerCam;

        private bool _camBlock = false;

        [Range(-90f, 90f)] private float _cameraY;
        private float _cameraX;
        
        private Quaternion _camFreeze = Quaternion.identity;

        private void Start() {
            LockCursor();
            _lookAction = InputSystem.actions.FindAction("Look");
            _cursorToggleAction = InputSystem.actions.FindAction("CursorToggle");
            _clickAction = InputSystem.actions.FindAction("ClickAction");
        }

        private void Update() {
            if (_cursorToggleAction.WasPressedThisFrame()) {
                Cursor.lockState = CursorLockMode.None;
                _camBlock = true;
            } else if (_clickAction.WasPressedThisFrame()) {
                LockCursor();
                _camBlock = false;
            }
            
            if (!_camBlock) {
                Vector2 lookInput = _lookAction.ReadValue<Vector2>();

                float inputY = lookInput.y * mouseSensitivity;
                float inputX = lookInput.x * mouseSensitivity;

                if (_cameraY - inputY < 90f && _cameraY - inputY > -90f) {
                    _cameraY -= inputY;
                }

                _cameraX += inputX;

                transform.rotation = Quaternion.Euler(_cameraY, _cameraX, 0f);
                _camFreeze = transform.rotation;
            } else {
                transform.rotation = _camFreeze;
            }
        }

        public void LockCursor() {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ToggleCameraMove(bool block) {
            _camBlock = block;
        }
    }
}