using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerInGame : NetworkBehaviour
    {
        private Transform _camera;
        private Interactable _interactableObject;

        private void Start()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            _camera = Camera.main!.transform;
            var inputActions = InputManager.Instance.InputActions;
            inputActions.Player.Interact.performed += _ => Buy();
        }

        private void Buy()
        {
            if (_interactableObject != null)
            {
                if (PointManager.Instance.GetPoints() >= _interactableObject.price)
                {
                    PointManager.Instance.RemovePoints(_interactableObject.price);
                    _interactableObject.Buy();
                }
                else
                {
                    Debug.Log("Not enough points");
                }
            }
        }

        private void FixedUpdate()
        {
            CheckIfInteractable();
        }

        private void CheckIfInteractable()
        {
            if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Buyable"))
                {
                    if (_interactableObject != null)
                    {
                        _interactableObject.priceText.color =
                            PointManager.Instance.GetPoints() >= _interactableObject.price
                                ? Color.white
                                : Color.red;
                        return; // already interacting with an object
                    }
                    _interactableObject = hit.collider.GetComponent<Interactable>();
                    _interactableObject.priceText.gameObject.SetActive(true);
                    _interactableObject.priceText.color =
                        PointManager.Instance.GetPoints() >= _interactableObject.price
                            ? Color.white
                            : Color.red;
                }
                else if (_interactableObject != null)
                {
                    _interactableObject.priceText.gameObject.SetActive(false);
                    _interactableObject = null;
                }
            }
            else
            {
                if (_interactableObject == null)
                    return;
                _interactableObject.priceText.gameObject.SetActive(false);
                _interactableObject = null;
            }
        }
    }
}
