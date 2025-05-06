using System.Collections.Generic;
using _Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    public class WeaponManager : NetworkBehaviour
    {
        [SerializeField]
        private List<GameObject> weapons = new();

        [SerializeField]
        private int currentWeaponIndex;

        [SerializeField]
        private GameObject currentWeapon;

        [SerializeField]
        private Transform weaponHolder;

        private void Start()
        {
            if (!IsOwner)
            {
                foreach (var obj in weapons)
                {
                    obj.GetComponent<Weapon>().enabled = false;
                }
            }
            else
            {
                Setup();
            }
            EquipWeapon(currentWeaponIndex);
        }

        private void Setup()
        {
            PlayerInputs.PlayerActions playerActions = InputManager.Instance.InputActions.Player;
            playerActions.EquipWeapon0.performed += _ => EquipWeapon(0);
            playerActions.EquipWeapon1.performed += _ => EquipWeapon(1);
            playerActions.EquipWeapon2.performed += _ => EquipWeapon(2);
        }

        private void EquipWeapon(int index)
        {
            if (index < 0 || index >= weapons.Count)
            {
                Debug.LogError("Invalid weapon index");
                return;
            }
            ShowWeaponAllRpc(index);
        }

        public void ResetAmmo()
        {
            foreach (var obj in weapons)
            {
                obj.GetComponent<Weapon>().ResetAmmo();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void ShowWeaponAllRpc(int index)
        {
            foreach (GameObject obj in weapons)
                obj?.SetActive(false);

            weapons[index]?.SetActive(true);
            currentWeapon = weapons[index];
        }
    }
}
