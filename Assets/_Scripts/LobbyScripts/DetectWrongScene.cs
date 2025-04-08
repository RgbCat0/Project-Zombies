using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.LobbyScripts
{
    public class DetectWrongScene : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (LobbyManager.Instance is null)
                SceneManager.LoadScene("Lobby");
            else
                Destroy(gameObject);
        }
    }
}
