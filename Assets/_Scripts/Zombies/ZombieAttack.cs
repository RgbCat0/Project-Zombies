using System;
using System.Collections;
using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class ZombieAttack : MonoBehaviour
    {
        public float damage = 10f;
        public float damageInterval = 1f; // seconds between damage
        public float dodgeInterval = 0.25f;
        private float _damageTimer;
        private Animator _animator;
        private GameObject _player;

        private void Start()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            _damageTimer = 0f;
            _player = other.gameObject;
            StartCoroutine(Attack());
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            _damageTimer += Time.deltaTime;

            if (_damageTimer >= damageInterval)
            {
                _damageTimer = 0f;
                StartCoroutine(Attack());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StopAllCoroutines();
                _player = null;
                _damageTimer = 0f;
            }
        }

        private IEnumerator Attack() // plays the animation but gives the player a chance to dodge
        {
            _animator.SetTrigger("Attack");
            yield return new WaitForSeconds(dodgeInterval); // also end of animation
            _player.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
