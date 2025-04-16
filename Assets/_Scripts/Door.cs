using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    public class Door : NetworkBehaviour
    {
        [SerializeField]
        private Animator animator;
        public int doorPrice = 1000;

        [SerializeField]
        private Canvas canvas;

        public TextMeshProUGUI priceText;

        private void Start()
        {
            if (priceText != null)
            {
                priceText.text = doorPrice.ToString();
            }
        }

        [Rpc(SendTo.Everyone)]
        public void OpenDoorRpc()
        {
            // Logic to open the door
            canvas.gameObject.SetActive(false);
            animator.SetTrigger("Open");
            Debug.Log("Door opened.");
            GetComponent<Collider>().enabled = false;
            enabled = false;
        }
    }
}
