using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerHealth : NetworkBehaviour
    {
        [SerializeField]
        private float maxHealth = 100f;

        public NetworkVariable<float> CurrentHealth { get; } =
            new(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void Start()
        {
            if (IsOwner)
                CurrentHealth.Value = maxHealth;
            CurrentHealth.OnValueChanged += OnHealthChanged;
        }

        public void TakeDamage(float damage)
        {
            if (!IsOwner) // Only the owner can take damage
                return;
            CurrentHealth.Value -= damage;
            if (CurrentHealth.Value <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Player has died.");
            // CurrentHealth = maxHealth;
        }

        private void OnHealthChanged(float previousValue, float newValue)
        {
            UIManager.Instance.UpdateHealth((int)newValue);
        }
    }
}
