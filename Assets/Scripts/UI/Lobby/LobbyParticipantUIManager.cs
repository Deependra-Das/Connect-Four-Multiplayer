using TMPro;
using UnityEngine;

namespace ConnectFourMultiplayer.UI
{
    public class LobbyParticipantUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private TMP_Text _winsCountText;
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private GameObject _notReadyPanel;

        private ulong _clientId = 0;
        private bool _isReady = false;

        public void Initialize(PlayerSessionData playerSessionData)
        {
            SetClientId(playerSessionData.clientId);
            SetUsername(playerSessionData.username.ToString());
            SetWinsCount(playerSessionData.winsCount.ToString());
            SetPlayerLobbyState(playerSessionData.isReady);
        }

        private void SetClientId(ulong assignedClientID)
        {
            _clientId = assignedClientID;
        }

        public void SetUsername(string username)
        {
            _usernameText.text = username;
        }

        private void SetWinsCount(string winsCount)
        {
            _winsCountText.text = winsCount;
        }

        public void SetPlayerLobbyState(bool isReady)
        {
            _isReady = isReady;
            TogglePlayerLobbyState();
        }

        private void TogglePlayerLobbyState()
        {
            _readyPanel.SetActive(_isReady);
            _notReadyPanel.SetActive(!_isReady);
        }
    }
}
