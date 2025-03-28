using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using _Scripts;
using Player = Unity.Services.Lobbies.Models.Player;

public class LobbyUi : MonoBehaviour // add status to the in game UI (for when clicking on create lobby etc)
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
    private Button lobbyDelete;

    [SerializeField]
    private Button lobbyDisconnect;

    [Header("Input Fields")]
    [SerializeField]
    private TMP_InputField mainName;

    [SerializeField]
    private TMP_InputField hostLobbyName;

    [SerializeField]
    private GameObject newLobbyPrefab;

    [SerializeField]
    private GameObject newPlayerPrefab;

    private const int YDownAmount = 100;
    private int _currentYDownAmount;
    private List<GameObject> _playerList = new List<GameObject>();

    private void Start() // Always starts in main menu
    {
        ChangeView();
        if (LobbyManager.Instance.IsSignedIn == false)
        {
            StartCoroutine(WaitForSignIn());
            return;
        }
        ChangeView(mainMenu);
        mainHost.interactable = false;
        mainJoin.interactable = false;
        SetupInputFields();
    }

    private IEnumerator WaitForSignIn()
    {
        while (LobbyManager.Instance.IsSignedIn == false)
        {
            yield return null;
        }
        Start();
    }

    private void SetupInputFields()
    {
        mainName.text = "";
        hostLobbyName.text = "";
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
        try
        {
            await LobbyMisc.ChangeName(mainName.text);
            ChangeView(hostMenu);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to change lobby: {e.Message}");
        }
    }

    public async void JoinMenu()
    {
        try
        {
            await ChangeName(mainName.text);
            ChangeView(joinMenu);
            GetLobbies();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
        }
    }

    private async Task ChangeName(string newName)
    {
        try
        {
            await LobbyMisc.ChangeName(newName);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to change name: {e.Message}");
        }
    }

    private void ChangeView(GameObject view = null)
    {
        mainMenu.SetActive(false);
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        view?.SetActive(true);
    }

    private async void GetLobbies()
    {
        try
        {
            QueryResponse lobbies = await LobbyManager.Instance.GetLobbies();
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
        catch (Exception e)
        {
            Debug.LogError($"Failed to get lobbies: {e.Message}");
        }
    }

    private void CreateLobbyUi(Lobby lobby)
    {
        try
        {
            GameObject newLobby = Instantiate(newLobbyPrefab, joinMenu.transform);
            newLobby.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{lobby.Name}\n{lobby.Players[0].Data["PlayerName"].Value}"; // should be lobby name
            newLobby
                .transform.GetChild(0)
                .GetChild(1)
                .GetComponent<Button>()
                .onClick.AddListener(() => JoinLobby(lobby));
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create a UI for a lobby: {lobby.Name}");
            Debug.LogException(e);
        }
    }

    private void JoinLobby(Lobby lobby)
    {
        Debug.Log($"Joining lobby: {lobby.Name}");
        LobbyManager.Instance.JoinLobby(lobby.Id);
    }

    public void CreateGame() => LobbyManager.Instance.CreateLobby(hostLobbyName.text);

    public void OnNewPlayer(List<Player> playerList)
    {
        foreach (GameObject obj in _playerList)
        {
            Destroy(obj);
        }

        _currentYDownAmount = 0;
        foreach (Player player in playerList)
        {
            GameObject newPlayer = Instantiate(newPlayerPrefab, lobbyMenu.transform);
            newPlayer.GetComponent<RectTransform>().anchoredPosition = new Vector3(
                0,
                _currentYDownAmount,
                0
            );
            newPlayer.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                player.Data["PlayerName"].Value;
            _currentYDownAmount -= _yDownAmount;
            _playerList.Add(newPlayer);
        }
    }

    public void GoToLobby() => ChangeView(lobbyMenu);
}
