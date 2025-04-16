using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Zombies
{
    public class Zombie : NetworkBehaviour // handles health and general
    {
        [SerializeField]
        private NetworkVariable<int> health = new(100);
        private int _defaultHealth = 100;

        [SerializeField]
        private int pointAmount = 10;

        [SerializeField]
        private ZombieMovement zombieMovement;

        [SerializeField]
        public NavMeshAgent agent;

        [SerializeField]
        private GameObject hitAnimationPrefab;

        [Rpc(SendTo.Everyone)]
        public void SetupRpc(
            // float healthMulti,
            // float speedMulti,
            // GameObject model
            string zombieInfoName,
            Vector3 spawnPos
        ) => StartCoroutine(Setup(zombieInfoName, spawnPos));

        private IEnumerator Setup(
            // float healthMulti,
            // Vector3 spawnPos,
            // float speedMulti,
            // GameObject model
            string zombieInfoName,
            Vector3 spawnPos
        )
        {
            // get the zombie info from the name
            ZombieInfo zombieInfo = WaveManager.Instance.zombies.Find(info =>
                info.zombieName == zombieInfoName
            );
            var healthMulti = zombieInfo.healthMultiplier;
            var speedMulti = zombieInfo.speedMultiplier;
            var model = zombieInfo.model;
            GameObject newObj = Instantiate(
                model,
                model.transform.position,
                model.transform.rotation
            );
            newObj.transform.parent = transform;
            newObj.transform.SetAsFirstSibling();
            health.Value = (int)(_defaultHealth * healthMulti);
            transform.position = spawnPos;
            zombieMovement.speed = 3.5f * speedMulti; // default * speedMulti
            yield return new WaitForSeconds(0.5f);
            transform.GetChild(0).gameObject.SetActive(true); // enable the model
            transform.GetChild(1).gameObject.SetActive(true); // enable the collider
            transform.GetChild(2).gameObject.SetActive(true); // enable the collider
            agent.enabled = true;
            zombieMovement.enabled = true;
        }

        [Rpc(SendTo.Server)]
        public void TakeDamageRpc(int damage)
        {
            health.Value -= damage;

            if (health.Value <= 0)
                DieRpc();
        }

        public void PlayHitAnimation(int damage)
        {
            // play hit animation
            GameObject hitAnimation = Instantiate(
                hitAnimationPrefab,
                transform.position,
                Quaternion.identity
            );
            hitAnimation.transform.parent = transform;
            hitAnimation.transform.SetAsLastSibling();
            hitAnimation.transform.localPosition = new Vector3(0, 0.8f, 0);
            hitAnimation.GetComponent<HitAnimation>().ShowHitText(damage.ToString());
        }

        [Rpc(SendTo.Server)]
        private void DieRpc()
        {
            // handle death
            // play animation
            // destroy object
            PointManager.Instance.AddPoints(pointAmount);
            WaveManager.Instance.EnemyDiedRpc();
            NetworkObject.Despawn();
        }
    }
}
