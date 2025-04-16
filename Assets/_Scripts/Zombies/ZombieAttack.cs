using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class ZombieAttack : MonoBehaviour
    {
        public float damage = 10f;
        public float damageInterval = 1f; // seconds between damage
        private float _damageTimer;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            _damageTimer = 0f;
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            _damageTimer += Time.deltaTime;

            if (_damageTimer >= damageInterval)
            {
                other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
                _damageTimer = 0f;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _damageTimer = 0f;
            }
        }
    }
}
