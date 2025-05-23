using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Zombies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieMovement : NetworkBehaviour // targets closest player from 4 players
    {
        private NavMeshAgent _agent;

        public float speed = 3.2f;

        public static List<Transform> players = new();
        private Animator _animator;

        private void Start()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }

            _animator = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = 0.4f;
        }

        private void FixedUpdate()
        {
            _agent.SetDestination(GetClosestPlayer());
            if (_agent.velocity.magnitude > 0.1f)
            {
                _animator.SetBool("Walking", true);
            }
            else
            {
                _animator.SetBool("Walking", false);
            }
            _agent.speed = speed;
        }

        private Vector3 GetClosestPlayer()
        {
            float closestDistance = Mathf.Infinity;
            Vector3 closestPlayer = Vector3.positiveInfinity;
            foreach (Transform player in players)
            {
                if (!player.gameObject.activeInHierarchy)
                    continue;
                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (distance >= closestDistance)
                    continue;

                closestDistance = distance;
                closestPlayer = player.position;
            }
            if (closestPlayer == Vector3.positiveInfinity)
            {
                // No players found
                Debug.Log("No players found");
                return transform.position;
            }

            return closestPlayer;
        }
    }
}
