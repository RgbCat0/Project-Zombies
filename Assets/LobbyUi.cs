using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using _Scripts;

public class LobbyUi : MonoBehaviour
{
    [Header("Parent Objects")]
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject hostMenu;

    [SerializeField]
    private GameObject joinMenu;

    [SerializeField]
    private GameObject lobbyMenu;

    // references to all the UI elements
    [Header("Buttons")]
    [SerializeField]
    private Button mainHost;

    [SerializeField]
    private Button mainJoin;

    [SerializeField]
    private Button hostCreate;

    [SerializeField]
    private Button joinJoin;

    [SerializeField]
    private Button copyCode;

    [Header("Input Fields")]
    [SerializeField]
    private TMP_InputField mainName;

    [SerializeField]
    private TMP_InputField hostLobbyName;

    [SerializeField]
    private TMP_InputField joinLobbyCode;

    [SerializeField]
    private GameObject newLobbyPrefab;

    private void Start() // Always starts in main menu
    {
        mainMenu.SetActive(true);
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        mainHost.interactable = false;
        mainJoin.interactable = false;
        SetupInputFields();
    }

    private void SetupInputFields()
    {
        mainName.text = "";
        hostLobbyName.text = "";
        // joinLobbyCode.text = "";
        mainName.onValueChanged.AddListener(
            delegate
            {
                CheckName();
            }
        );
    }

    private void CheckName()
    {
        if (!string.IsNullOrEmpty(mainName.text))
        {
            mainHost.interactable = true;
            mainJoin.interactable = true;
        }
        else
        {
            mainHost.interactable = false;
            mainJoin.interactable = false;
        }
    }

    public async void HostMenu()
    {
        await LobbyManager.Instance.ChangeName(mainName.text);
        ChangeView(hostMenu);
    }

    public async void JoinMenu()
    {
        await LobbyManager.Instance.ChangeName(mainName.text);
        ChangeView(joinMenu);
        GetLobbies();
    }

    private void ChangeView(GameObject view)
    {
        mainMenu.SetActive(false);
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        view.SetActive(true);
    }

    private async void GetLobbies()
    {
        QueryResponse lobbies = await LobbyManager.Instance.GetLobbies();
        List<Lobby> test = lobbies.Results;
        if (lobbies.Results.Count == 0)
        {
            Debug.Log("No lobbies found");
            return;
        }
        lobbies.Results.ForEach(lobby =>
        {
            Debug.Log($"Lobby: {lobby.Name} - {lobby.Id}");
            CreateLobbyUi(lobby);
        });
    }

    private void CreateLobbyUi(Lobby lobby)
    {
        GameObject newLobby = Instantiate(newLobbyPrefab, joinMenu.transform);
        var hostname = lobby.HostId;
        foreach (Player player in lobby.Players.Where(player => player.Id == hostname))
        {
            hostname = player.Profile.Name;
        }

        newLobby.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            $"{lobby.Name}\n{hostname}"; // should be lobby name
        newLobby
            .transform.GetChild(0)
            .GetChild(1)
            .GetComponent<Button>()
            .onClick.AddListener(() => JoinLobby(lobby));
    }

    private void JoinLobby(Lobby lobby)
    {
        Debug.Log($"Joining lobby: {lobby.Name}");
        LobbyManager.Instance.JoinLobby(lobby.Id);
    }

    public void CreateGame()
    {
        LobbyManager.Instance.CreateLobby(hostLobbyName.text);
    }
}
