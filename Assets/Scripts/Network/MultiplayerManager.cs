using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }
    public string PlayerUsername { get; private set; }

    public const int MAX_LOBBY_SIZE = 2;

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
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        //string activeSceneName = SceneManager.GetActiveScene().name.ToString();
        //Enum.TryParse<SceneNameEnum>(activeSceneName, out var sceneEnumValue);

        //if (NetworkManager.Singleton.IsServer && sceneEnumValue != SceneNameEnum.LobbyScene)
        //{
        //    connectionApprovalResponse.Approved = false;
        //    connectionApprovalResponse.Reason = "Game has already Started.";
        //    return;
        //}
        //if (NetworkManager.Singleton.ConnectedClients.Count >= MAX_LOBBY_SIZE)
        //{
        //    connectionApprovalResponse.Approved = false;
        //    connectionApprovalResponse.Reason = "Lobby Capacity Full. No Available Slots.";
        //    return;
        //}
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
            //RequestPlayerRegistrationServerRpc(clientId, AuthenticationService.Instance.PlayerId, PlayerUsername);
        }
        string activeSceneName = SceneManager.GetActiveScene().name.ToString();
        Enum.TryParse<SceneNameEnum>(activeSceneName, out var sceneEnumValue);

        if (sceneEnumValue == SceneNameEnum.LobbyScene)
        {
         //Play Player Joined Audio
        }
    }

    private void OnClientHostDisconnected(ulong clientId)
    {
        string activeSceneName = SceneManager.GetActiveScene().name.ToString();
        Enum.TryParse<SceneNameEnum>(activeSceneName, out var sceneEnumValue);

        if (sceneEnumValue == SceneNameEnum.LobbyScene)
        {
            //Play Player Left Audio
        }

        if (NetworkManager.Singleton.IsServer && sceneEnumValue == SceneNameEnum.LobbyScene)
        {
            //RequestPlayerDeregistrationServerRpc(clientId);
        }

        if (!NetworkManager.Singleton.IsServer && sceneEnumValue == SceneNameEnum.GameplayScene)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    //[ServerRpc(RequireOwnership = false)]
    //public void RequestPlayerRegistrationServerRpc(ulong clientId, string playerId, string username)
    //{
    //    PlayerSessionManager.Instance.RegisterPlayer(clientId, playerId, username);
    //    ConfirmPlayerRegistrationClientRpc(clientId);
    //}

    //[ClientRpc]
    //private void ConfirmPlayerRegistrationClientRpc(ulong clientId)
    //{
    //    if (NetworkManager.Singleton.IsServer)
    //    {
    //        CharacterManager.Instance.HandleLateJoin(clientId);
    //    }
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void RequestPlayerDeregistrationServerRpc(ulong clientId)
    //{
    //    PlayerSessionManager.Instance.DeregisterPlayer(clientId);
    //}

}