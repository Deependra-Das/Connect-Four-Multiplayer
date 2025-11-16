using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.LobbyRelay;
using ConnectFourMultiplayer.Main;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ConnectFourMultiplayer.Network
{
    public class PlayerLobbyStateManager : NetworkBehaviour
    {
        public static PlayerLobbyStateManager Instance { get; private set; }

        private Dictionary<ulong, bool> _playerStateDictionary;

        private void Awake()
        {
            Instance = this;
            _playerStateDictionary = new Dictionary<ulong, bool>();
        }

        public override void OnNetworkSpawn()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkSpawn();
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            Dispose();
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (_playerStateDictionary.ContainsKey(clientId))
            {
                _playerStateDictionary.Remove(clientId);
            }
        }

        public void SetPlayerReady()
        {
            SetPlayerReadyServerRpc();
        }

        public void SetPlayerNotReady()
        {
            SetPlayerNotReadyServerRpc();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void SetPlayerReadyServerRpc(RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;          

            PlayerSessionDataManager.Instance.SetPlayerStatusServerRpc(clientId, true);   
            SetPlayerLobbyStateClientRpc(clientId, true);

            if (NetworkManager.Singleton.ConnectedClients.Count == MultiplayerManager.MAX_LOBBY_SIZE)
            {
                bool allClientsReady = true;

                foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    if (!_playerStateDictionary.ContainsKey(clientID) || !_playerStateDictionary[clientID])
                    {
                        allClientsReady = false;
                        break;
                    }
                }

                if (allClientsReady)
                {
                    LobbyRelayManager.Instance.DeleteLobby();
                    SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene, true);
                }
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void SetPlayerNotReadyServerRpc(RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;

            PlayerSessionDataManager.Instance.SetPlayerStatusServerRpc(clientId, false);
            SetPlayerLobbyStateClientRpc(clientId, false);
        }

        [ClientRpc]
        public void SetPlayerLobbyStateClientRpc(ulong clientId, bool isReady)
        {
            _playerStateDictionary[clientId] = isReady;
            EventBusManager.Instance.Raise(EventNameEnum.PlayerLobbyStateChanged, clientId, isReady);
        }

        private void Dispose()
        {
            _playerStateDictionary.Clear();
        }
    }
}