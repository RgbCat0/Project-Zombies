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

        private void Update()
        {
            _agent.SetDestination(_player.position);
        }
    }
}
