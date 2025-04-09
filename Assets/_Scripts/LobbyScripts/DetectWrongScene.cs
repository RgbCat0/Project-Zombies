using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.LobbyScripts
{
    public class DetectWrongScene : MonoBehaviour
    {
        private void Awake()
        {
            if (LobbyManager.Instance is null)
                SceneManager.LoadScene("Lobby");
            else
                Destroy(gameObject);
        }
    }
}
