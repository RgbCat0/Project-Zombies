using TMPro;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class HitAnimation : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI hitText;

        [SerializeField]
        private float DestroyTime = 1f;

        public void ShowHitText(string text)
        {
            Destroy(gameObject, DestroyTime);
            if (hitText != null)
            {
                hitText.text = text;
                hitText.gameObject.SetActive(true);
            }
        }
    }
}
