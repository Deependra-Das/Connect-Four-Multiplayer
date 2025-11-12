using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace BattleRoyale.LobbyModule
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
        private Lobby _joinedLobby;
        private float _heartbeatTimer = 0;
        private float _heartbeatTimerMax = 15f;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUnityAuthentication();
        }

        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new InitializationOptions();
                options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

                await UnityServices.InitializeAsync(options);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private void Update()
        {
            HandleHeartbeat();
        }

        private void HandleHeartbeat()
        {
            if (isLobbyHost())
            {
                _heartbeatTimer -= Time.deltaTime;

                if (_heartbeatTimer <= 0f)
                {
                    _heartbeatTimer = _heartbeatTimerMax;
                    LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
                }
            }
        }

        private bool isLobbyHost()
        {
            return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                int maxConnections = 1;

                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        private async Task<string> GetRelateJoinCode(Allocation allocation)
        {
            try
            {
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        private async Task<JoinAllocation> JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                return joinAllocation;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }


        public async void CreateLobby()
        {
            string lobbyName = "TestLobby2";
            int lobbySize = 2;
            bool isPrivate = false;

            EventBusManager.Instance.RaiseNoParams(EventNameEnum.CreateLobbyStarted);
            try
            {
                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbySize,
                new CreateLobbyOptions { IsPrivate = isPrivate, });

                Allocation allocation = await AllocateRelay();
                string relayJoinCode = await GetRelateJoinCode(allocation);

                await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
                });

                var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                MultiplayerManager.Instance.StartHost();
                SceneLoader.Instance.LoadScene(SceneNameEnum.LobbyScene, true);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                EventBusManager.Instance.RaiseNoParams(EventNameEnum.CreateLobbyFailed);
            }
        }

        public async void QuickJoin()
        {
            EventBusManager.Instance.RaiseNoParams(EventNameEnum.JoinStarted);
            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                MultiplayerManager.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                EventBusManager.Instance.RaiseNoParams(EventNameEnum.QuickJoinFailed);
            }
        }

        public async void DeleteLobby()
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);

                    _joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        public async void LeaveLobby()
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                    _joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        public Lobby GetLobby()
        {
            if (_joinedLobby != null)
            {
                return _joinedLobby;
            }

            return null;
        }
    }
}