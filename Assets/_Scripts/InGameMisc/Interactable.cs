using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts
{
    public abstract class Interactable : NetworkBehaviour
    {
        public int price;
        public bool isInteractable = true;

        [SerializeField]
        protected Canvas canvas;

        public TextMeshProUGUI priceText;

        protected virtual void Start()
        {
            if (priceText != null)
            {
                priceText.text = price.ToString();
                priceText.gameObject.SetActive(false);
            }
        }

        public virtual void Buy() // is here to be overridden but has no default implementation
        {
            // Default implementation does nothing
        }
    }
}
