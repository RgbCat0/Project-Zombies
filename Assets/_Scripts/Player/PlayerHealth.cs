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
            if (!IsOwner)
                return;
            Debug.Log("Player has died.");
            GameManager.Instance.PlayerDiedRpc(OwnerClientId);
            // set the camera to follow another player
        }

        public void Respawn()
        {
            if (!IsOwner)
                return;

            CurrentHealth.Value = maxHealth;
        }

        private void OnHealthChanged(float previousValue, float newValue)
        {
            if (IsOwner)
                UIManager.Instance.UpdateHealth((int)newValue);
        }
    }
}
