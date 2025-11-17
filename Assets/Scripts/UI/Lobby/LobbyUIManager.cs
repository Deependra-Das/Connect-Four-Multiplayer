using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.LobbyRelay;
using ConnectFourMultiplayer.Main;
using ConnectFourMultiplayer.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ConnectFourMultiplayer.UI
{
    public class LobbyUIManager : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _notReadyButton;
        [SerializeField] private Button _leaveLobbyButton;
        [SerializeField] private Transform _lobbyParticipantContainerTransform;
        [SerializeField] private GameObject _participantPrefab;
        [SerializeField] private TMP_Text _lobbyStatusText;

        [Header("Leave Lobby PopUp")]
        [SerializeField] private GameObject _leaveLobbyConfirmationPopUp;
        [SerializeField] private TMP_Text _hostLobbyNoticeText;
        [SerializeField] private Button _yesConfirmationButton;
        [SerializeField] private Button _noConfirmationButton;

        [Header("Disconnected PopUp")]
        [SerializeField] private GameObject _disconnectedPopUp;
        [SerializeField] private TMP_Text _disconnectedCountdownCharSelectUIText;
        [SerializeField] float _disconnectedCountdownTime = 5f;

        private Dictionary<ulong, GameObject> _lobbyParticipantDictionary;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _readyButton.onClick.AddListener(OnReadyButtonClicked);
            _notReadyButton.onClick.AddListener(OnNotReadyButtonClicked);
            _leaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClicked);
            _yesConfirmationButton.onClick.AddListener(OnYesButtonClicked);
            _noConfirmationButton.onClick.AddListener(OnNoButtonClicked);
            EventBusManager.Instance.Subscribe(EventNameEnum.PlayerJoined, HandlePlayerJoinedLobbyUI);
            EventBusManager.Instance.Subscribe(EventNameEnum.PlayerLeft, HandlePlayerLeftLobbyUI);
            EventBusManager.Instance.Subscribe(EventNameEnum.PlayerLobbyStateChanged, HandleLobbyPlayerStateChangeLobbyUI);

            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnectCallbackLobbyUI;
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnectCallbackLobbyUI;
        }

        private void UnsubscribeToEvents()
        {
            _readyButton.onClick.RemoveListener(OnReadyButtonClicked);
            _notReadyButton.onClick.RemoveListener(OnNotReadyButtonClicked);
            _leaveLobbyButton.onClick.RemoveListener(OnLeaveLobbyButtonClicked);
            _yesConfirmationButton.onClick.RemoveListener(OnYesButtonClicked);
            _noConfirmationButton.onClick.RemoveListener(OnNoButtonClicked);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.PlayerJoined, HandlePlayerJoinedLobbyUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.PlayerLeft, HandlePlayerLeftLobbyUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.PlayerLobbyStateChanged, HandleLobbyPlayerStateChangeLobbyUI);

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnectCallbackLobbyUI;
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnectCallbackLobbyUI;
            }
        }

        private void Awake()
        {
            _lobbyParticipantDictionary = new Dictionary<ulong, GameObject>();
        }

        void Start()
        {
            _notReadyButton.gameObject.SetActive(false);
            _readyButton.gameObject.SetActive(true);
            HideConfirmationPopup();
            SetLobbyInformation();
            SetHostLobbyNoticeText();
            SpawnAllConnectedClients();
        }

        private void OnReadyButtonClicked()
        {
            _readyButton.gameObject.SetActive(false);
            _notReadyButton.gameObject.SetActive(true);
            PlayerLobbyStateManager.Instance.SetPlayerReady();
        }

        private void OnNotReadyButtonClicked()
        {
            _notReadyButton.gameObject.SetActive(false);
            _readyButton.gameObject.SetActive(true);
            PlayerLobbyStateManager.Instance.SetPlayerNotReady();
        }

        private void OnLeaveLobbyButtonClicked()
        {
            ShowConfirmationPopup();
        }

        private void ShowConfirmationPopup()
        {
            _leaveLobbyConfirmationPopUp.SetActive(true);
        }
        private void HideConfirmationPopup()
        {
            _leaveLobbyConfirmationPopUp.SetActive(false);
        }

        private void OnYesButtonClicked()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                LobbyRelayManager.Instance.DeleteLobby();
            }
            else
            {
                LobbyRelayManager.Instance.LeaveLobby();
            }
            CleanUp();
            HideConfirmationPopup();
            SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
        }

        private void OnNoButtonClicked()
        {
            HideConfirmationPopup();
        }    

        private void HandlePlayerJoinedLobbyUI(object[] parameters)
        {
            SpawnAllConnectedClients();
        }

        private void SpawnAllConnectedClients()
        {
            var connectedClientIds = NetworkManager.Singleton.ConnectedClients.Keys.ToHashSet();

            foreach (var sessionData in PlayerSessionDataManager.Instance.playerSessionDataNetworkList)
            {
                if (connectedClientIds.Contains(sessionData.clientId))
                {
                    SpawnLobbyParticipant(sessionData.clientId);
                }
            }
        }

        private void SpawnLobbyParticipant(ulong clientId)
        {
            if (!_lobbyParticipantDictionary.ContainsKey(clientId))
            {
                GameObject participant = Instantiate(_participantPrefab, _lobbyParticipantContainerTransform);
                participant.name = $"PlayerCharacter_{clientId}";

                PlayerSessionData playerData = PlayerSessionDataManager.Instance.GetPlayerSessionData(clientId);
                participant.GetComponent<LobbyParticipantUIManager>().Initialize(playerData);
                _lobbyParticipantDictionary.Add(clientId, participant);
            }
        }

        private void HandlePlayerLeftLobbyUI(object[] parameters)
        {
            ulong clientId = (ulong)parameters[0];

            if (NetworkManager.Singleton.IsHost)
            {
                DespawnLobbyParticipant(clientId);
            }
            else
            {
                DespawnAllLobbyParticipants();
            }
        }

        private void DespawnAllLobbyParticipants()
        {
            foreach (var entry in _lobbyParticipantDictionary)
            {
                var character = entry.Value.gameObject;
                if (character != null && character.activeSelf)
                {
                    Destroy(character);
                }
            }

            _lobbyParticipantDictionary.Clear();
        }

        private void DespawnLobbyParticipant(ulong clientId)
        {
            if (_lobbyParticipantDictionary.ContainsKey(clientId))
            {
                GameObject participantToDespawn = _lobbyParticipantDictionary[clientId].gameObject;
                Destroy(participantToDespawn);
                _lobbyParticipantDictionary.Remove(clientId);
            }
        }

        private void HandleLobbyPlayerStateChangeLobbyUI(object[] parameters)
        {
            ulong clientId = (ulong)parameters[0];
            bool isReady = (bool)parameters[1];

            SetPlayerLobbyStatus(clientId, isReady);
        }


        public void SetPlayerLobbyStatus(ulong clientId, bool isReady)
        {
            GameObject playerToChangeState = _lobbyParticipantDictionary[clientId].gameObject;

            if (playerToChangeState != null)
            {
                playerToChangeState.GetComponent<LobbyParticipantUIManager>().SetPlayerLobbyState(isReady);
            }
        }

        private void HandleClientDisconnectCallbackLobbyUI(ulong clientID)
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogWarning("NetworkManager.Singleton is null.");
                return;
            }

            if ((NetworkManager.Singleton.IsServer && clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClients.Count <= 1) || !NetworkManager.Singleton.IsServer)
            {
                ShowDisconnectedNotification();
                StartCoroutine(DisconnectedCountdownSequence());
            }
            SetLobbyInformation();
        }

        private void HandleClientConnectCallbackLobbyUI(ulong clientID)
        {
            SetLobbyInformation();
        }

        private void ShowDisconnectedNotification()
        {
            _disconnectedPopUp.SetActive(true);
        }

        private IEnumerator DisconnectedCountdownSequence()
        {
            float currentTime = _disconnectedCountdownTime;

            while (currentTime > 0)
            {
                _disconnectedCountdownCharSelectUIText.text = "Returning To Main Menu In... " + Mathf.Ceil(currentTime).ToString() + "s";
                currentTime -= 1f;
                yield return new WaitForSeconds(1f);
            }

            CleanUp();
            HideDisconnectedNotification();
            SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
        }

        private void HideDisconnectedNotification()
        {
            _disconnectedPopUp.SetActive(false);
        }

        private void CleanUp()
        {
            GameManager.Instance.Get<CleanUpService>().CleanUpPlayerSessionDataManager();
            GameManager.Instance.Get<CleanUpService>().CleanUpLobbyRelay();
            GameManager.Instance.Get<CleanUpService>().ResetServices();
            GameManager.Instance.Get<CleanUpService>().CleanUpMultiplayerManager();
            GameManager.Instance.Get<CleanUpService>().CleanUpNetworkManager();
        }

        private void SetLobbyInformation()
        {
            if(NetworkManager.Singleton.ConnectedClients.Count == MultiplayerManager.MAX_LOBBY_SIZE)
            {
                _lobbyStatusText.text = "Waiting for Players to be Ready";
            }
            else
            {
                _lobbyStatusText.text = "Waiting for Players to Join";
            }
        }

        private void SetHostLobbyNoticeText()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                _hostLobbyNoticeText.gameObject.SetActive(true);
            }
            else
            {
                _hostLobbyNoticeText.gameObject.SetActive(false);
            }
        }
    }
}