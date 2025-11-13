using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
using Unity.Netcode;
using UnityEngine;

public class PlayerSessionDataManager : NetworkBehaviour
{
    public static PlayerSessionDataManager Instance { get; private set; }

    public NetworkList<PlayerSessionData> playerSessionDataNetworkList;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerSessionDataNetworkList = new NetworkList<PlayerSessionData>();
    }

    private void OnEnable()
    {
        playerSessionDataNetworkList.OnListChanged += OnplayerSessionDataNetworkListChanged;
    }

    private void OnDisable()
    {
        playerSessionDataNetworkList.OnListChanged -= OnplayerSessionDataNetworkListChanged;
    }

    public void RegisterPlayerSessionData(ulong clientId, string playerId, string username, int winsCount)
    {
        if (!IsServer) return;

        playerSessionDataNetworkList.Add(new PlayerSessionData(clientId, playerId, username, winsCount));
    }

    public void DeregisterPlayerSessionData(ulong clientId)
    {
        if (!IsServer) return;

        for (int i = 0; i < playerSessionDataNetworkList.Count; i++)
        {
            if (playerSessionDataNetworkList[i].clientId == clientId)
            {
                playerSessionDataNetworkList.RemoveAt(i);
                break;
            }
        }
    }

    public PlayerSessionData GetPlayerSessionData(ulong clientId)
    {
        foreach (var sessionData in playerSessionDataNetworkList)
        {
            if (sessionData.clientId == clientId)
            {
                return sessionData;
            }
        }
        return default;
    }

    private void OnplayerSessionDataNetworkListChanged(NetworkListEvent<PlayerSessionData> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<PlayerSessionData>.EventType.Add:
                HandlePlayerJoined(changeEvent.Value);
                break;

            default:
                break;
        }

        EventBusManager.Instance.Raise(EventNameEnum.PlayerJoined);
    }

    private void HandlePlayerJoined(PlayerSessionData playerData)
    {
        EventBusManager.Instance.Raise(EventNameEnum.PlayerJoined, playerData.clientId);
    }

}

