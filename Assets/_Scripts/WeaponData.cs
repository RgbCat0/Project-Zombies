namespace _Scripts
{
    [System.Serializable]
    public class WeaponData
    {
        public int Damage { get; private set; }
        public float HeadMultiplier { get; private set; }
        public int FireRateRpm { get; private set; } // rounds per minute;
        public float FireRate => 60f / FireRateRpm; // in seconds
        public float ReloadTime { get; private set; } // in seconds
        public float ReloadTimeEmpty { get; private set; } // in seconds
        public int MagazineSize { get; private set; } // in rounds
        public int MaxAmmo { get; private set; } // in rounds
    }
}
