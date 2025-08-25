using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class CameraController : MonoBehaviour {
        public PauseMenuManager pmm;
        
        private InputAction _lookAction;
        private InputAction _cursorToggleAction;
        private InputAction _clickAction;
    
        public float mouseSensitivity = 0.25f;
        [SerializeField] private Camera playerCam;

        public bool gamePaused = false;
        private bool _camBlock = false;

        public bool IsBlocked => _camBlock;

        [Range(-90f, 90f)] private float _cameraY;
        private float _cameraX;

        private GameObject _books;
        
        private void Start() {
            ToggleCursor(true);
            _lookAction = InputSystem.actions.FindAction("Look");
            _cursorToggleAction = InputSystem.actions.FindAction("CursorToggle");
            _clickAction = InputSystem.actions.FindAction("ClickAction");
            _books = GameObject.Find("Books");
        }

        private void Update() {
            if (_cursorToggleAction.WasPressedThisFrame()) {
                gamePaused = !gamePaused;
                ToggleCameraMove(gamePaused);
                ToggleCursor(!gamePaused);
                
                if (gamePaused) {
                    pmm.PauseGame();
                } else {
                    pmm.ResumeGame();
                }
            }
            
            if (_clickAction.WasPressedThisFrame() && !gamePaused) {
                ToggleCursor(true);
            }
            
            if (!_camBlock) {
                Vector2 lookInput = _lookAction.ReadValue<Vector2>();

                float inputY = lookInput.y * mouseSensitivity;
                float inputX = lookInput.x * mouseSensitivity;

                if (_cameraY - inputY < 90f && _cameraY - inputY > -90f) {
                    _cameraY -= inputY;
                }

                _cameraX += inputX;
            }

            transform.rotation = Quaternion.Euler(_cameraY, _cameraX, 0f);
            
            // Book frustum culling
            if (!_books || _books.transform.childCount <= 0) return;
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCam);
            foreach (var rend in _books.GetComponentsInChildren<Renderer>())
            {
                if (rend)
                {
                    rend.enabled = GeometryUtility.TestPlanesAABB(frustumPlanes, rend.bounds);
                }
            }
        }

        public void ToggleCursor(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void ToggleCameraMove(bool block) {
            if (_camBlock == block) return;
            _camBlock = block;
        }
    }
}
