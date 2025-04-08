using UnityEngine;

namespace _Scripts.Player
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public PlayerInputs InputActions;
        
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
            InputActions = new PlayerInputs();
            InputActions.Player.Enable();
        }
    }
}