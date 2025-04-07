using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Zombies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieMovement : MonoBehaviour // targets closest player from 4 players
    {
        private NavMeshAgent _agent;
        private Transform _player; //single player testing before multiplayer

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindWithTag("Player").transform;
            _agent.stoppingDistance = 1f;
        }

        private void FixedUpdate()
        {
            _agent.SetDestination(_player.position);
        }

        private Transform GetClosestPlayer()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            float closestDistance = Mathf.Infinity;
            Transform closestplayer = null;
            foreach (var player in players)
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestplayer = player.transform;
                }
            }

            return closestplayer;
        }
    }
}
