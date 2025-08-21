using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player {
    public class CameraController : MonoBehaviour {
        private InputAction _lookAction;
        private InputAction _cursorToggleAction;
        private InputAction _clickAction;
    
        public float mouseSensitivity = 0.25f;

        [SerializeField] private Camera playerCam;

        public bool gamePaused = false;
        private bool _camBlock = false;

        [Range(-90f, 90f)] private float _cameraY;
        private float _cameraX;
        
        private void Start() {
            LockCursor();
            _lookAction = InputSystem.actions.FindAction("Look");
            _cursorToggleAction = InputSystem.actions.FindAction("CursorToggle");
            _clickAction = InputSystem.actions.FindAction("ClickAction");
        }

        private void Update() {
            if (_cursorToggleAction.WasPressedThisFrame()) {
                gamePaused = !_camBlock;
                _camBlock = !_camBlock;
                ToggleCameraMove(_camBlock);
                ToggleCursor(_camBlock);
                // toggle pause menu UI here <--- !
            }
            
            if (_clickAction.WasPressedThisFrame() & !_camBlock) {
                LockCursor();
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
            } else {
                transform.rotation = Quaternion.Euler(_cameraY, _cameraX, 0f);
            }
        }

        public void LockCursor() {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ToggleCursor(bool locked) {
            if (!locked) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void ToggleCameraMove(bool block) {
            print(block);
            _camBlock = block;
        }
    }
}