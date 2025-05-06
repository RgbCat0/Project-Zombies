using TMPro;
using UnityEngine;

namespace _Scripts.Zombies
{
    public class HitAnimation : MonoBehaviour // shows how much damage the player dealt.
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
