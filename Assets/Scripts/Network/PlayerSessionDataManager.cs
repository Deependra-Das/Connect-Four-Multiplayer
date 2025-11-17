using ConnectFourMultiplayer.Event;
using Unity.Netcode;
using UnityEngine;

namespace ConnectFourMultiplayer.Network
{
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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (playerSessionDataNetworkList != null)
            {
                playerSessionDataNetworkList.OnListChanged += OnplayerSessionDataNetworkListChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (playerSessionDataNetworkList != null)
            {
                playerSessionDataNetworkList.OnListChanged -= OnplayerSessionDataNetworkListChanged;
            }

            Dispose();
        }

        public void RegisterPlayerSessionData(ulong clientId, string playerId, string username, int winsCount)
        {
            if (!IsServer) return;

            if (playerSessionDataNetworkList != null)
            {
                playerSessionDataNetworkList.Add(new PlayerSessionData(clientId, playerId, username, winsCount, false));
            }
        }

        public void DeregisterPlayerSessionData(ulong clientId)
        {
            if (!IsServer || playerSessionDataNetworkList == null) return;

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
            if (playerSessionDataNetworkList != null)
            {
                foreach (var sessionData in playerSessionDataNetworkList)
                {
                    if (sessionData.clientId == clientId)
                    {
                        return sessionData;
                    }
                }
            }
            return default;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SetPlayerStatusServerRpc(ulong clientId, bool isReady)
        {
            if (playerSessionDataNetworkList != null)
            {
                for (int i = 0; i < playerSessionDataNetworkList.Count; i++)
                {
                    var sessionData = playerSessionDataNetworkList[i];

                    if (sessionData.clientId == clientId)
                    {
                        sessionData.isReady = isReady;
                        playerSessionDataNetworkList[i] = sessionData;
                        return;
                    }
                }
            }
        }

        private void OnplayerSessionDataNetworkListChanged(NetworkListEvent<PlayerSessionData> changeEvent)
        {
            if (changeEvent.Type == NetworkListEvent<PlayerSessionData>.EventType.Add)
            {
                HandlePlayerJoined(changeEvent.Value);
            }

            EventBusManager.Instance.Raise(EventNameEnum.PlayerJoined);
        }

        private void HandlePlayerJoined(PlayerSessionData playerData)
        {
            EventBusManager.Instance.Raise(EventNameEnum.PlayerJoined, playerData.clientId);
        }

        public void Dispose()
        {
            if (IsServer && playerSessionDataNetworkList != null)
            {
                playerSessionDataNetworkList.Clear();
            }
        }
    }
}