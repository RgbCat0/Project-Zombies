using System;
using System.Collections;
using TMPro;
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
        private int _newDefaultHealth = 100; // used for UI

        [SerializeField]
        private int pointAmount = 10;

        [SerializeField]
        private ZombieMovement zombieMovement;

        [SerializeField]
        public NavMeshAgent agent;

        [SerializeField]
        private GameObject hitAnimationPrefab;

        [SerializeField]
        private RectTransform healthBar;

        [SerializeField]
        private RectTransform healthBarChange;

        [SerializeField]
        private GameObject healthBarParent;

        [SerializeField]
        private TextMeshProUGUI healthText;
        private float _healthBarWidth; // start width of the health bar

        [SerializeField]
        private float healthBarHideTime = 2f; // time to hide the health bar
        private float _healthBarHideTimer;

        private float changeWaitTime = 0.8f; // time to wait for the health bar to change

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
            var attackDamage = zombieInfo.attackDamage;
            GameObject modelObj = Instantiate(
                model,
                model.transform.position,
                model.transform.rotation
            );
            modelObj.transform.parent = transform;
            modelObj.transform.SetAsFirstSibling();
            health.Value = (int)(_defaultHealth * healthMulti);
            _newDefaultHealth = (int)(_defaultHealth * healthMulti);
            _healthBarWidth = healthBar.sizeDelta.x;
            transform.position = spawnPos;
            zombieMovement.speed = 3.5f * speedMulti; // default * speedMulti
            yield return new WaitForSeconds(0.5f);
            transform.GetChild(0).gameObject.SetActive(true); // enable the model
            transform.GetChild(1).gameObject.SetActive(true); // enable the collider
            transform.GetChild(2).gameObject.SetActive(true); // enable the collider
            GetComponentInChildren<ZombieAttack>().damage = attackDamage;
            agent.enabled = true;
            zombieMovement.enabled = true;
        }

        [Rpc(SendTo.Server)]
        public void TakeDamageRpc(int damage)
        {
            var oldHealth = health.Value;
            health.Value -= damage;
            PlayHitAnimation(damage);
            UpdateUI(oldHealth);

            if (health.Value <= 0)
                DieRpc();
        }

        private void Update()
        {
            if (healthBarParent.activeSelf)
            {
                _healthBarHideTimer += Time.deltaTime;
                if (_healthBarHideTimer >= healthBarHideTime)
                    healthBarParent.SetActive(false);
            }
        }

        private void UpdateUI(int oldHealth)
        {
            healthBarParent.SetActive(true);
            _healthBarHideTimer = 0;
            healthBar.sizeDelta = new Vector2(
                _healthBarWidth * ((float)health.Value / _newDefaultHealth),
                healthBar.sizeDelta.y
            );
            healthText.text = $"{health.Value}/{_newDefaultHealth}";

            StartCoroutine(HealthBarChange(oldHealth));
        }

        private IEnumerator HealthBarChange(int oldHealth)
        {
            // start the health change at the current health
            healthBarChange.anchoredPosition = new Vector2(
                healthBar.anchoredPosition.x + healthBar.sizeDelta.x,
                healthBar.anchoredPosition.y
            );
            // set the size to be how much has changed
            var changedAmount =
                _healthBarWidth * (((float)oldHealth - health.Value) / _newDefaultHealth);
            Debug.Log(changedAmount);
            healthBarChange.sizeDelta = new Vector2(changedAmount, healthBarChange.sizeDelta.y);
            yield return new WaitForSeconds(changeWaitTime);
            healthBarChange.sizeDelta = new Vector2(0, healthBarChange.sizeDelta.y);
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
