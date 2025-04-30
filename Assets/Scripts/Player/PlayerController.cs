using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private new Transform camera;

        private InputAction _moveAction;
        private InputAction _jumpAction;

        private float _xInput;
        private float _yInput;

        public float moveSpeed = 50f;
        public float jumpForce = 50f;

        private void Start() {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
        }

        private void Update() {
            // inputs
            _xInput = _moveAction.ReadValue<Vector2>().x;
            _yInput = _moveAction.ReadValue<Vector2>().y;

            if (_jumpAction.WasPressedThisFrame()) {
                var grounded = Physics.Raycast(transform.position, Vector3.down, 2 * 0.5f + 0.3f);
                if (grounded) {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
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