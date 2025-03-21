using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour // fps movement (handles only movement)
    {
        private PlayerInputs _inputActions;
        private Rigidbody _rigidbody;
        private Vector2 _movementInput;
        private Vector2 _mouseInput;
        private Camera _camera;
        private Vector2 _cameraRotation;

        [Header("Mouse")]
        [SerializeField]
        private Vector2 sensitivity;

        [SerializeField]
        private float maxLookAngle;

        [Header("Movement")]
        [SerializeField, FormerlySerializedAs("moveSpeed")]
        private float acceleration;

        [SerializeField]
        private float maxSpeed;
        private bool _canJump = true;

        [SerializeField]
        private float jumpForce;

        [SerializeField]
        private float jumpCooldown;

        [Header("Drag")]
        [SerializeField]
        private float playerHeight;

        [SerializeField]
        private float groundDrag;

        [SerializeField]
        private LayerMask groundMask;
        private bool _isGrounded;

        private void Awake()
        {
            _inputActions = new PlayerInputs();
            _inputActions.Enable();
            _rigidbody = GetComponent<Rigidbody>();
            _camera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start()
        {
            _inputActions.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Look.performed += ctx =>
                _mouseInput = ctx.ReadValue<Vector2>() * sensitivity;
            _inputActions.Player.Move.canceled += _ => _movementInput = Vector2.zero;
            _inputActions.Player.Look.canceled += _ => _mouseInput = Vector2.zero;
            _inputActions.Player.Jump.performed += _ => Jump();
        }

        private void Update()
        {
            HandleMouse();
            _isGrounded = CheckGrounded();
            ApplyGroundDrag();
            HandleHorizontalSpeed();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMouse()
        {
            float newCamX = _cameraRotation.x + _mouseInput.x; // rotates the player left and right
            float newCamY = _cameraRotation.y + _mouseInput.y; // rotates the camera up and down
            newCamY = Mathf.Clamp(newCamY, -maxLookAngle, maxLookAngle);
            _cameraRotation = new Vector2(newCamX, newCamY);
            _camera.transform.localRotation = Quaternion.Euler(
                -_cameraRotation.y,
                _cameraRotation.x,
                0
            );
            _rigidbody.MoveRotation(Quaternion.Euler(0, _cameraRotation.x, 0));
        }

        private void HandleMovement()
        {
            Vector3 moveDirection =
                transform.right * _movementInput.x + transform.forward * _movementInput.y;
            _rigidbody.AddForce(moveDirection.normalized * acceleration, ForceMode.Force);
        }

        private bool CheckGrounded() =>
            Physics.Raycast(transform.position, Vector3.down, playerHeight, groundMask);

        private void ApplyGroundDrag() => _rigidbody.linearDamping = _isGrounded ? groundDrag : 0;

        private void HandleHorizontalSpeed()
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.y = 0;
            if (!(velocity.magnitude > maxSpeed))
                return;
            velocity = velocity.normalized * maxSpeed;
            _rigidbody.linearVelocity = new Vector3(
                velocity.x,
                _rigidbody.linearVelocity.y,
                velocity.z
            );
        }

        private void Jump()
        {
            if (!_isGrounded || !_canJump)
                return;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _canJump = false;
            StartCoroutine(CoolDownJump());
        }

        private IEnumerator CoolDownJump()
        {
            yield return new WaitForSeconds(jumpCooldown);
            _canJump = true;
        }
    }
}
