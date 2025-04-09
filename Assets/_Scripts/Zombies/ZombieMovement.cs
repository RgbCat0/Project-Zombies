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

        [SerializeField]
        private List<Transform> _players = new();

        private void Start()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = 1.5f;
            foreach (var player in GameManager.Instance.playerMovements)
            {
                _players.Add(player.transform);
            }
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
            foreach (Transform player in _players)
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (distance >= closestDistance)
                    continue;

                closestDistance = distance;
                closestPlayer = player.position;
            }
            return closestPlayer;
        }
    }
}
