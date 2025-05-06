using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    public class Door : Interactable
    {
        [SerializeField]
        private Animator animator;

        protected override void Start()
        {
            base.Start(); // added for consistency
        }

        public override void Buy()
        {
            OpenDoorRpc(); // Call the RPC to open the door
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
