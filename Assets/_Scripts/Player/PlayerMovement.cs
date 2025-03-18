using System;
using UnityEngine;

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

        [SerializeField]
        private Vector2 sensitivity;
        [SerializeField]
        private float maxLookAngle;
        [SerializeField]
        private float moveSpeed;
        
        
        
        
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
            _inputActions.Player.Look.performed += ctx => _mouseInput = ctx.ReadValue<Vector2>() * sensitivity;
            _inputActions.Player.Move.canceled += _ => _movementInput = Vector2.zero;
            _inputActions.Player.Look.canceled += _ => _mouseInput = Vector2.zero;
        }

        private void Update()
        {
            HandleMouse();
            HandleMovement();
        }

        private void HandleMouse()
        {
            float newCamX = _cameraRotation.x + _mouseInput.x; // rotates the player left and right
            float newCamY = _cameraRotation.y + _mouseInput.y; // rotates the camera up and down
            newCamY = Mathf.Clamp(newCamY, -maxLookAngle, maxLookAngle);
            _cameraRotation = new Vector2(newCamX, newCamY);
            _camera.transform.localRotation = Quaternion.Euler(-_cameraRotation.y, 0, 0);
            transform.rotation = Quaternion.Euler(0, _cameraRotation.x, 0);
        }

        private void HandleMovement()
        {
            Vector3 moveDirection = transform.right * _movementInput.x + transform.forward * _movementInput.y;
            _rigidbody.AddForce(moveDirection * moveSpeed, ForceMode.Force);
        }
    }
}
