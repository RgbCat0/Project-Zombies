using System.Collections.Generic;
using UnityEngine;
using _Scripts.Player;

namespace _Scripts
{
    public class WeaponManager : MonoBehaviour
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
            Setup();
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

            foreach (GameObject obj in weapons)
                obj?.SetActive(false);

            weapons[index]?.SetActive(true);
            currentWeapon = weapons[index];
        }
    }
}
