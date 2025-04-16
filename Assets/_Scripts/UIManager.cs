using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private TextMeshProUGUI _ammoText;
        private TextMeshProUGUI _reserveText;
        private TextMeshProUGUI _healthText;
        private TextMeshProUGUI _pointsText;
        private TextMeshProUGUI _currWaveText;
        private TextMeshProUGUI _currEnemies;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            _ammoText = GameObject.Find("currAmmo").GetComponent<TextMeshProUGUI>();
            _reserveText = GameObject.Find("reserveAmmo").GetComponent<TextMeshProUGUI>();
            _healthText = GameObject.Find("health").GetComponent<TextMeshProUGUI>();
            _pointsText = GameObject.Find("points").GetComponent<TextMeshProUGUI>();
            _currWaveText = GameObject.Find("currWave").GetComponent<TextMeshProUGUI>();
            _currEnemies = GameObject.Find("currEnemies").GetComponent<TextMeshProUGUI>();
        }

        public void UpdateAmmo(int current, int reserve)
        {
            _ammoText.text = current.ToString();
            _reserveText.text = reserve.ToString();
        }

        public void UpdatePoints(int points) => _pointsText.text = $"Score: {points}";

        public void UpdateHealth(int health) => _healthText.text = $"Health: {health}";

        public void UpdateWave(int wave) => _currWaveText.text = $"Wave: {wave}";

        public void UpdateEnemies(int enemies) => _currEnemies.text = $"Enemies: {enemies}";
    }
}
