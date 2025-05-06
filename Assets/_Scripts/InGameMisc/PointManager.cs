using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    public class PointManager : NetworkBehaviour
    {
        public static PointManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [SerializeField]
        private NetworkVariable<int> points = new();

        private void Start()
        {
            points.OnValueChanged += OnPointsChanged;
        }

        public void AddPoints(int amount)
        {
            if (!IsOwner) // Only the owner can add points
                return;
            points.Value += amount;
        }

        public void RemovePoints(int amount)
        {
            if (!IsOwner) // Only the owner can remove points
                return;
            points.Value -= amount;
        }

        public void ResetPoints()
        {
            if (!IsOwner) // Only the owner can reset points
                return;
            points.Value = 0;
        }

        public int GetPoints() => points.Value;

        private void OnPointsChanged(int previousValue, int newValue)
        {
            UIManager.Instance.UpdatePoints(newValue);
        }
    }
}
