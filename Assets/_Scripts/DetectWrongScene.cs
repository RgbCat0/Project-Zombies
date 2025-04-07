using UnityEngine;
using UnityEngine.SceneManagement;
using _Scripts;

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
