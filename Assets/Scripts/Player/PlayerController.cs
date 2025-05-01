using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private new Transform camera;
        public Transform groundCheck;
        public LayerMask mask;

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;

        private float _xInput;
        private float _yInput;
        private float _magnitude;

        [Header("Player Movement")]
        public float walkSpeed = 5f;
        public float jumpForce = 7.5f;
        public float sprintSpeed = 10f;

        private float moveSpeed = 5f;
        
        private bool _sprinting = false;

        [Header("View Bobbing")]

        [SerializeField] private float _frequency = 3f;
        [SerializeField] private float _bobSpeed = 1f;
        [SerializeField] private float _bobAmplitude = 0.05f;
        private float _bobTimer = 0f;
        private Vector3 _cameraInitialLocalPos;

        private void Start() {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _sprintAction = InputSystem.actions.FindAction("Sprint");

            _cameraInitialLocalPos = camera.localPosition;
        }

        void HandleViewBobbing() {
            // Get flat movement magnitude (ignore vertical)
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float speed = horizontalVelocity.magnitude;

            if (speed > 0.1f && IsGrounded()) {
                _bobTimer += Time.deltaTime * _frequency * (speed * _bobSpeed);
                float bobOffsetY = Mathf.Sin(_bobTimer) * _bobAmplitude;
                float bobOffsetX = Mathf.Cos(_bobTimer * 0.5f) * _bobAmplitude * 0.5f;

                camera.localPosition = _cameraInitialLocalPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
            } else {
                _bobTimer = 0f;
                camera.localPosition = Vector3.Lerp(
                    camera.localPosition,
                    _cameraInitialLocalPos,
                    Time.deltaTime * 5f
                );
            }
        }

        private bool IsGrounded() {
            return Physics.CheckSphere(groundCheck.position, 0.5f, mask);
        }


        private void Update() {
            // inputs
            _xInput = _moveAction.ReadValue<Vector2>().x;
            _yInput = _moveAction.ReadValue<Vector2>().y;

            if (_jumpAction.WasPressedThisFrame() && IsGrounded()) {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }

            _magnitude = new Vector3(_xInput, _yInput).magnitude;

            if (_magnitude > 0) {
                HandleViewBobbing();
            }
            
            if (_sprintAction.WasPressedThisFrame()) {
                if (_sprinting) {
                    moveSpeed = walkSpeed;
                    _sprinting = false;
                } else {
                    moveSpeed = sprintSpeed;
                    _sprinting = true;
                }
            }

            if (rb.transform.position.y <= -50) {
                rb.transform.position = Vector3.zero;
            }
        }

        private void FixedUpdate() {
            // movement
            rb.transform.rotation = Quaternion.Euler(0f, camera.eulerAngles.y, 0f);

            var inputH = _xInput * moveSpeed;
            var inputV = _yInput * moveSpeed;

            var playerF = rb.transform.forward;
            playerF.y = 0f;
            var cameraR = camera.transform.right;
            cameraR.y = 0f;

            var forwardRel = inputV * playerF;
            var rightRel = inputH * cameraR;

            var moveDir = forwardRel + rightRel;

            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }
    }
}