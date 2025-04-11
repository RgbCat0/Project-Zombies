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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _ammoText = GameObject.Find("currAmmo").GetComponent<TextMeshProUGUI>();
            _reserveText = GameObject.Find("reserveAmmo").GetComponent<TextMeshProUGUI>();
            _healthText = GameObject.Find("health").GetComponent<TextMeshProUGUI>();
            _pointsText = GameObject.Find("points").GetComponent<TextMeshProUGUI>();
        }

        public void UpdateAmmo(int current, int reserve)
        {
            _ammoText.text = current.ToString();
            _reserveText.text = reserve.ToString();
        }

        public void UpdatePoints(int points) => _pointsText.text = $"Score: {points}";

        public void UpdateHealth(int health) => _healthText.text = $"Health: {health}";
    }
}
