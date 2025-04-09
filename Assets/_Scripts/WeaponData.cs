using UnityEngine.Serialization;

namespace _Scripts
{
    [System.Serializable]
    public class WeaponData
    {
        public int damage;
        public float headMultiplier;
        public int fireRateRpm; // rounds per minute;
        public float FireRate => 60f / fireRateRpm; // in seconds
        public float reloadTime; // in seconds
        public float reloadTimeEmpty; // in seconds
        public int magazineSize; // in rounds
        public int maxAmmo; // in rounds
        public bool isAutomatic; // is the weapon automatic
    }
}
