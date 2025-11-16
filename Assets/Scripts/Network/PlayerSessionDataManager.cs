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
            playerSessionDataNetworkList.OnListChanged += OnplayerSessionDataNetworkListChanged;
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

            playerSessionDataNetworkList.Add(new PlayerSessionData(clientId, playerId, username, winsCount, false));
        }

        public void DeregisterPlayerSessionData(ulong clientId)
        {
            if (!IsServer) return;

            if (playerSessionDataNetworkList != null)
            {
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
            PlayerSessionData sessionDataToUpdate;

            if (playerSessionDataNetworkList != null)
            {
                foreach (var sessionData in playerSessionDataNetworkList)
                {
                    if (sessionData.clientId == clientId)
                    {
                        sessionDataToUpdate = sessionData;
                        break;
                    }
                }
                sessionDataToUpdate.isReady = isReady;
            }
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

        public void Dispose()
        {
            if (IsServer)
            {
                if (playerSessionDataNetworkList != null)
                {
                    playerSessionDataNetworkList.Clear();
                }

                playerSessionDataNetworkList = null;
            }
        }
    }

}