using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.LobbyScripts
{
    public class LobbyUi : MonoBehaviour // add status to the in game UI (for when clicking on create lobby etc.)
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

        [SerializeField]
        private Button startGame;

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
        private List<GameObject> _playerList = new();
        private List<GameObject> _lobbies = new();

        [SerializeField]
        private TextMeshProUGUI statusText;

        private void Start() // Always starts in main menu
        {
            ChangeView();
            if (LobbyManager.Instance.IsSignedIn == false)
            {
                ChangeStatus("Signing in...");
                StartCoroutine(WaitForSignIn());
                return;
            }
            ChangeView(mainMenu);
            mainHost.interactable = false;
            mainJoin.interactable = false;
            SetupInputFields();
            ChangeStatus();
            mainName.onValueChanged.AddListener(Call);
        }

        private void Call(string arg0)
        {
            //removes all spaces from the name
            mainName.text = arg0.Replace(" ", "");
            if (mainName.text.Length > 20)
                mainName.text = mainName.text[..20];
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
                ChangeStatus("Updating name...");
                await ChangeName(mainName.text);
                ChangeView(hostMenu);
                ChangeStatus();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to change lobby: {e.Message}");
                ChangeStatus("Failed to change name.", Color.red);
            }
        }

        public async void JoinMenu()
        {
            try
            {
                ChangeStatus("Updating name...");
                await ChangeName(mainName.text);
                ChangeView(joinMenu);
                ChangeStatus("Getting lobbies...");
                GetLobbies();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join lobby: {e.Message}");
                ChangeStatus("Failed to change name.", Color.red);
            }
        }

        private async Task ChangeName(string newName)
        {
            try
            {
                await LobbyUtil.ChangeName(newName);
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

        public async void GetLobbies()
        {
            try
            {
                QueryResponse lobbies = await LobbyManager.Instance.GetLobbies();
                if (lobbies.Results.Count == 0)
                {
                    Debug.Log("No lobbies found");
                    return;
                }
                if (_lobbies != null)
                {
                    foreach (GameObject obj in _lobbies)
                    {
                        Destroy(obj);
                    }
                }
                _currentYDownAmount = 0;
                lobbies.Results.ForEach(lobby =>
                {
                    GameObject real = CreateLobbyUi(lobby);
                    _lobbies.Add(real);
                    _currentYDownAmount -= YDownAmount;
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get lobbies: {e.Message}");
                ChangeStatus("Failed to get lobbies.", Color.red);
            }
        }

        /// <summary>
        /// This method creates a new UI for each lobby.
        /// This adds a listener to the button to join the lobby.
        /// </summary>
        /// <param name="lobby">Specifies which lobby is being used.</param>
        /// <exception cref="Exception">Thrown when the UI fails to create a lobby.</exception>
        private GameObject CreateLobbyUi(Lobby lobby) // Spawns a new Prefab for each lobby
        {
            try
            {
                GameObject newLobby = Instantiate(newLobbyPrefab, joinMenu.transform);
                newLobby.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    $"{lobby.Name}\n{lobby.Players[0].Data["PlayerName"].Value}"; // should be lobby name
                newLobby.GetComponent<RectTransform>().anchoredPosition = new Vector3(
                    0,
                    _currentYDownAmount,
                    0
                );
                newLobby
                    .transform.GetChild(0)
                    .GetChild(1)
                    .GetComponent<Button>()
                    .onClick.AddListener(() => JoinLobby(lobby));
                return newLobby;
            }
            catch (Exception e)
            {
                ChangeStatus("Failed to get new player.", Color.red);
                Debug.LogError($"Failed to create a UI for a lobby: {lobby.Name}");
                Debug.LogException(e);
            }
            return null;
        }

        private void JoinLobby(Lobby lobby)
        {
            Debug.Log($"Joining lobby: {lobby.Name}");
            LobbyManager.Instance.JoinLobby(lobby.Id);
        }

        public void CreateGame() => LobbyManager.Instance.CreateLobby(hostLobbyName.text);

        public void OnNewPlayer()
        {
            var playerList = LobbyManager.Instance.Lobby.Players;
            foreach (GameObject obj in _playerList)
            {
                Destroy(obj);
            }

            _currentYDownAmount = 0;
            foreach (Unity.Services.Lobbies.Models.Player player in playerList)
            {
                GameObject newPlayer = Instantiate(newPlayerPrefab, lobbyMenu.transform);
                newPlayer.GetComponent<RectTransform>().anchoredPosition = new Vector3(
                    0,
                    _currentYDownAmount,
                    0
                );
                newPlayer.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    player.Data["PlayerName"].Value;
                _currentYDownAmount -= YDownAmount;
                _playerList.Add(newPlayer);
            }
        }

        public void GoToLobby()
        {
            ChangeView(lobbyMenu);
            bool isServer = NetworkManager.Singleton.IsServer;
            lobbyDelete.gameObject.SetActive(isServer);
            lobbyDisconnect.gameObject.SetActive(!isServer);
            startGame.gameObject.SetActive(isServer);
        }

        public void ChangeStatus(string status = "", Color color = default)
        {
            if (color == default)
                color = Color.white;
            statusText.text = status;
            statusText.color = color;
        }
    }
}
