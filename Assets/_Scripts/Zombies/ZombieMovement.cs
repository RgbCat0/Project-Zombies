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

        [SerializeField]
        private float speed = 3.2f;

        public static List<Transform> players = new();

        private void Start()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = 1.5f;
        }

        private void FixedUpdate()
        {
            _agent.SetDestination(GetClosestPlayer());
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
