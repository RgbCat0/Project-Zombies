using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> weapons = new();
        private List<GameObject> spawnedWeapons = new();
        [SerializeField]
        private int currentWeaponIndex;
        [SerializeField]
        private GameObject currentWeapon;
        [SerializeField]
        private Transform weaponHolder;
        
        private void Start()
        {
            
        }

        private void SpawnAllWeapons()
        {
            foreach (var obj in weapons)
            {
                GameObject newWeapon = Instantiate(obj, weaponHolder);
                spawnedWeapons.Add(newWeapon);
            }
        }

        private void EquipWeapon(int index)
        {

        }
    }
}
