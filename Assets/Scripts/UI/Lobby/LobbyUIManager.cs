using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _notReadyButton;
    [SerializeField] private Button _leaveLobbyButton;
    [SerializeField] private Transform _lobbyParticipantContainerTransform;
    [SerializeField] private GameObject _participantPrefab;

    [Header("Leave Lobby PopUp")]
    [SerializeField] private GameObject _leaveLobbyConfirmationPopUp;
    [SerializeField] private TMP_Text _hostLobbyNoticeText;
    [SerializeField] private Button _yesConfirmationButton;
    [SerializeField] private Button _noConfirmationButton;

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
    }

    private void Awake()
    {
        _lobbyParticipantDictionary = new Dictionary<ulong, GameObject>();
    }

    void Start()
    {
        _notReadyButton.gameObject.SetActive(false);
        _readyButton.gameObject.SetActive(true);
        HideBackToMainMenuConfirmationPopup();
        SpawnAllConnectedClients();
    }

    private void OnReadyButtonClicked()
    {
        _readyButton.gameObject.SetActive(false);
        _notReadyButton.gameObject.SetActive(true);

        SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene, true);
    }

    private void OnNotReadyButtonClicked()
    {
        _notReadyButton.gameObject.SetActive(false);
        _readyButton.gameObject.SetActive(true);
    }

    private void OnLeaveLobbyButtonClicked()
    {
        ShowBackToMainMenuConfirmationPopup();
    }

    private void ShowBackToMainMenuConfirmationPopup()
    {
        _leaveLobbyConfirmationPopUp.SetActive(true);
    }

    private void OnYesButtonClicked()
    {
        HideBackToMainMenuConfirmationPopup();
        SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
    }

    private void OnNoButtonClicked()
    {
        HideBackToMainMenuConfirmationPopup();
    }

    private void HideBackToMainMenuConfirmationPopup()
    {
        _leaveLobbyConfirmationPopUp.SetActive(false);
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

        Debug.Log(clientId);
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
}