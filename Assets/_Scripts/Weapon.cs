using System.Collections;
using _Scripts.Player;
using _Scripts.Zombies;
using UnityEngine;

namespace _Scripts
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private WeaponData weaponData;

        private bool _setup;
        private bool _isShooting;
        private bool _isReloading;
        private bool _onCooldown;
        private int _currentMagAmmo;
        private int _currentTotalAmmo;
        private LayerMask _zombieMask;
        private Transform _shootPoint;

        private void Start()
        {
            InputManager.Instance.InputActions.Player.Shoot.performed += _ => _isShooting = true;
            InputManager.Instance.InputActions.Player.Shoot.canceled += _ => _isShooting = false;
            InputManager.Instance.InputActions.Player.Reload.performed += _ =>
                StartCoroutine(Reload());
            if (weaponData == null)
            {
                Debug.LogError("WeaponData is not assigned in the inspector.");
                return;
            }
            _zombieMask = LayerMask.NameToLayer("Zombie");
            _shootPoint = Camera.main!.transform;

            _setup = true;
        }

        private void OnEnable()
        {
            if (!_setup)
                return;
            _isShooting = false;
            _isReloading = false;
        }

        private void OnDisable()
        {
            if (!_setup)
                return;
            _isShooting = false;
            _isReloading = false;
        }

        private void Update()
        {
            if (_currentMagAmmo <= 0 && !_isReloading)
            {
                StartCoroutine(Reload());
            }
            if (_isShooting && !_isReloading && !_onCooldown)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            if (_currentMagAmmo <= 0)
                return;
            _currentMagAmmo--;

            if (
                Physics.Raycast(
                    _shootPoint.position,
                    _shootPoint.forward,
                    out var hit,
                    120f,
                    _zombieMask
                )
            )
            {
                var zombie = hit.collider.GetComponent<Zombie>();
                if (zombie)
                {
                    zombie.TakeDamageRpc(weaponData.Damage);
                }
            }
            StartCoroutine(CoolDown());
        }

        private IEnumerator CoolDown()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(weaponData.FireRate);
            _onCooldown = false;
        }

        private IEnumerator Reload()
        {
            if (_isReloading)
                yield break;
            _isReloading = true;
            yield return new WaitForSeconds(weaponData.ReloadTime);
        }
    }
}
