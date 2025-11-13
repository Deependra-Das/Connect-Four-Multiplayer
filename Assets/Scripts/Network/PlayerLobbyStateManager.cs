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

        private Dictionary<ulong, (string playerName, bool isReady)> _playerStateDictionary;

        private void Awake()
        {
            Instance = this;
            _playerStateDictionary = new Dictionary<ulong, (string, bool)>();
        }

        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
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

            if (!_playerStateDictionary.ContainsKey(clientId))
            {
                _playerStateDictionary.Add(clientId, ("Player" + clientId, true));
            }
            else
            {
                _playerStateDictionary[clientId] = (_playerStateDictionary[clientId].playerName, true);
            }

            PlayerSessionDataManager.Instance.SetPlayerStatusServerRpc(clientId, true);   
            NotifyPlayerLobbyStateChangeClientRpc(clientId, true);

            if (NetworkManager.Singleton.ConnectedClients.Count == MultiplayerManager.MAX_LOBBY_SIZE)
            {
                bool allClientsReady = true;

                foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    if (!_playerStateDictionary.ContainsKey(clientID) || !_playerStateDictionary[clientID].isReady)
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

            if (_playerStateDictionary.ContainsKey(clientId))
            {
                _playerStateDictionary[clientId] = (_playerStateDictionary[clientId].playerName, false);
            }

            PlayerSessionDataManager.Instance.SetPlayerStatusServerRpc(clientId, false);
            NotifyPlayerLobbyStateChangeClientRpc(clientId, false);
        }

        [ClientRpc]
        public void NotifyPlayerLobbyStateChangeClientRpc(ulong clientId, bool isReady)
        {
            EventBusManager.Instance.Raise(EventNameEnum.PlayerLobbyStateChanged, clientId, isReady);
        }
    }
}