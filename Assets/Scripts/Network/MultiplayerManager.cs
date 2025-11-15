using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Gameplay;
using ConnectFourMultiplayer.Main;
using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConnectFourMultiplayer.Network
{
    public class MultiplayerManager : NetworkBehaviour
    {
        public static MultiplayerManager Instance { get; private set; }
        public string PlayerUsername { get; private set; }

        public const int MAX_LOBBY_SIZE = 2;

        public NetworkVariable<PlayerTurnEnum> _gameWinner = new NetworkVariable<PlayerTurnEnum>(PlayerTurnEnum.None,
           NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayerUsername = PlayerPrefs.GetString(GameManager.UsernameKey).ToString();
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientHostDisconnected;
            NetworkManager.Singleton.StartHost();
            PlayerSessionDataManager.Instance.RegisterPlayerSessionData(NetworkManager.Singleton.LocalClientId, AuthenticationService.Instance.PlayerId, PlayerUsername, 0);
        }

        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            string activeSceneName = SceneManager.GetActiveScene().name.ToString();
            Enum.TryParse<SceneNameEnum>(activeSceneName, out var sceneEnumValue);

            if (NetworkManager.Singleton.IsServer && sceneEnumValue != SceneNameEnum.LobbyScene)
            {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game has already Started.";
                return;
            }
            if (NetworkManager.Singleton.ConnectedClients.Count >= MAX_LOBBY_SIZE)
            {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Lobby Capacity Full. No Available Slots.";
                return;
            }
            connectionApprovalResponse.Approved = true;
        }

        public void StartClient()
        {
            //EventBusManager.Instance.RaiseNoParams(EventNameEnum.TryingToJoinGame);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientHostDisconnected;
            NetworkManager.Singleton.StartClient();
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                RequestPlayerRegistrationServerRpc(clientId, AuthenticationService.Instance.PlayerId, PlayerUsername);
            }

            string activeSceneName = SceneManager.GetActiveScene().name.ToString();
            Enum.TryParse<SceneNameEnum>(activeSceneName, out var sceneEnumValue);
        }

        private void OnClientHostDisconnected(ulong clientId)
        {
            string activeSceneName = SceneManager.GetActiveScene().name.ToString();
            Enum.TryParse<SceneNameEnum>(activeSceneName, out var sceneEnumValue);

            if (sceneEnumValue == SceneNameEnum.LobbyScene)
            {
                //Play Player Left Audio
            }

            RequestPlayerDeregistration(clientId);
            EventBusManager.Instance.Raise(EventNameEnum.PlayerLeft, clientId);


            if (!NetworkManager.Singleton.IsServer && sceneEnumValue == SceneNameEnum.GameplayScene)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void RequestPlayerRegistrationServerRpc(ulong clientId, string playerId, string username)
        {
            PlayerSessionDataManager.Instance.RegisterPlayerSessionData(clientId, playerId, username, 0);
        }

        public void RequestPlayerDeregistration(ulong clientId)
        {
            PlayerSessionDataManager.Instance.DeregisterPlayerSessionData(clientId);
        }

        public void SetGameWinner(PlayerTurnEnum winner)
        {
            if (IsServer)
            {
                _gameWinner.Value = winner;
                Debug.Log(_gameWinner.Value);
            }
        }
    }
}