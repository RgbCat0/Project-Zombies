using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private TextMeshProUGUI _ammoText;
        private TextMeshProUGUI _reserveText;

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
        }

        public void UpdateAmmo(int current, int reserve)
        {
            _ammoText.text = current.ToString();
            _reserveText.text = reserve.ToString();
        }
    }
}
