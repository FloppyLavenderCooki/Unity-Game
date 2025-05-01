using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private new Transform camera;

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;

        private float _xInput;
        private float _yInput;
        private float _magnitude;


        public float moveSpeed = 50f;
        public float jumpForce = 50f;
        
        private bool _sprinting = false;

        public Transform groundCheck;

        [Header("View Bobbing")]
        [SerializeField] private float frequency = 3f;
        [SerializeField] private float bobSpeed = 1f;
        [SerializeField] private float bobAmplitude = 0.05f;
        private float bobTimer = 0f;
        private Vector3 cameraInitialLocalPos;

        private void Start() {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _sprintAction = InputSystem.actions.FindAction("Sprint");

            cameraInitialLocalPos = camera.localPosition;
        }

        void HandleViewBobbing()
        {
            // Get flat movement magnitude (ignore vertical)
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float speed = horizontalVelocity.magnitude;

            if (speed > 0.1f)
            {
                bobTimer += Time.deltaTime * frequency * (speed * bobSpeed);
                float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude;
                float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmplitude * 0.5f;

                camera.localPosition = cameraInitialLocalPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
            }
            else
            {
                bobTimer = 0f;
                camera.localPosition = Vector3.Lerp(
                    camera.localPosition,
                    cameraInitialLocalPos,
                    Time.deltaTime * 5f
                );
            }
        }


        private void Update() {
            // inputs
            _xInput = _moveAction.ReadValue<Vector2>().x;
            _yInput = _moveAction.ReadValue<Vector2>().y;
            
            if (_jumpAction.WasPressedThisFrame()) {
                if (Physics.CheckSphere(groundCheck.position, 0.5f)) {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                }
            }

            _magnitude = new Vector3(_xInput, _yInput).magnitude;

            if (_magnitude > 0) {
                HandleViewBobbing();
            }
            
            if (_sprintAction.WasPressedThisFrame()) {
                if (_sprinting) {
                    moveSpeed = 5f;
                    _sprinting = false;
                } else {
                    moveSpeed = 10f;
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