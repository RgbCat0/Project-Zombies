using System.Collections;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Zombies;

namespace _Scripts
{
    public class Weapon : MonoBehaviour
    {
        public WeaponData weaponData;

        private bool _setup;
        private bool _isShooting;
        private bool _isReloading;
        private bool _onCooldown;
        private int _currentMagAmmo;
        private int _currentTotalAmmo;
        private Transform _shootPoint;
        private bool _hasShot;

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
            if (!_isShooting)
                _hasShot = false;
            if (!_isShooting || _isReloading || _onCooldown)
            {
                return;
            }

            if (!weaponData.isAutomatic && !_hasShot)
            {
                _hasShot = true;
                Shoot();
            }
            else if (weaponData.isAutomatic)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            if (_currentMagAmmo <= 0)
                return;
            _currentMagAmmo--;
            int playerLayer = LayerMask.NameToLayer("Player");
            int ignorePlayerMask = ~(1 << playerLayer);
            var ray = new Ray(_shootPoint.position, _shootPoint.forward);

            if (Physics.Raycast(ray, out var hit, 120f, ignorePlayerMask))
            {
                var zombie = hit.collider.GetComponent<Zombie>();
                if (zombie)
                {
                    zombie.TakeDamageRpc(weaponData.damage);
                }
            }
            StartCoroutine(CoolDown());
        }

        private IEnumerator CoolDown()
        {
            if (_onCooldown)
                yield break;
            _onCooldown = true;
            yield return new WaitForSeconds(weaponData.FireRate);
            _onCooldown = false;
        }

        private IEnumerator Reload()
        {
            if (_isReloading || _currentTotalAmmo <= 0)
                yield break;
            _isReloading = true;
            if (_currentMagAmmo <= 0)
                yield return new WaitForSeconds(weaponData.reloadTimeEmpty);
            else
                yield return new WaitForSeconds(weaponData.reloadTime);

            _isReloading = false;
        }
    }
}
