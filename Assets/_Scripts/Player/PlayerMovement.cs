using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : NetworkBehaviour // fps movement (handles only movement)
    {

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
        private float sideAcceleration;

        [SerializeField]
        private float backwardAcceleration;

        [SerializeField]
        private float airAcceleration;

        [SerializeField]
        private float maxSpeed;
        private bool _canJump = true;
        private bool _isJumping;
        private bool _jumpCoroutine;

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

        [Header("Misc")]
        [SerializeField]
        private Transform camHolder;

        private void Start()
        {
            
            if (!IsOwner)
            {
                enabled = false;
                return;
            }

            var inputActions = InputManager.Instance.InputActions;
            _rigidbody = GetComponent<Rigidbody>();
            _camera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            CamMover.CamHolder = camHolder;
            inputActions.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Look.performed += ctx =>
                _mouseInput = ctx.ReadValue<Vector2>() * sensitivity;
            inputActions.Player.Move.canceled += _ => _movementInput = Vector2.zero;
            inputActions.Player.Look.canceled += _ => _mouseInput = Vector2.zero;
            inputActions.Player.Jump.performed += _ => _isJumping = true;
            inputActions.Player.Jump.canceled += _ => _isJumping = false;
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
            Jump();
            if (_isGrounded && !_jumpCoroutine && !_canJump)
                StartCoroutine(CoolDownJump());
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
            float newAccel = airAcceleration;
            if (_isGrounded)
            {
                if (_movementInput.y < 0)
                    newAccel = backwardAcceleration;
                else if (_movementInput.x != 0)
                    newAccel = sideAcceleration;
                else
                    newAccel = acceleration;
            }
            _rigidbody.AddForce(moveDirection.normalized * newAccel, ForceMode.Force);
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
            if (!_isGrounded || !_canJump || !_isJumping)
                return;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _canJump = false;
        }

        private IEnumerator CoolDownJump()
        {
            _jumpCoroutine = true;
            yield return new WaitUntil(() => _isGrounded);
            yield return new WaitForSeconds(jumpCooldown);
            _canJump = true;
            _jumpCoroutine = false;
        }
    }
}
