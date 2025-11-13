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

    public void OnEnable()
    {
        playerSessionDataNetworkList.OnListChanged += OnPlayerListChanged;
    }

    private void OnDisable()
    {
        if (playerSessionDataNetworkList != null)
        {
            playerSessionDataNetworkList.OnListChanged -= OnPlayerListChanged;
        }
    }

    private void OnPlayerListChanged(NetworkListEvent<PlayerSessionData> changeEvent)
    {
        if (playerSessionDataNetworkList != null)
        {
            PrintPlayerSessionDataList();
        }
    }

    public void PrintPlayerSessionDataList()
    {
        foreach (var playerData in playerSessionDataNetworkList)
        {
            Debug.Log($"ClientId: {playerData.clientId}, Username: {playerData.username}, Wins: {playerData.winsCount}");
        }
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
}

