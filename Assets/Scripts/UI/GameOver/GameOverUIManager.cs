using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Gameplay;
using ConnectFourMultiplayer.Main;
using ConnectFourMultiplayer.Network;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace ConnectFourMultiplayer.UI
{
    public class GameOverUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _winnerUserNameText;
        [SerializeField] private TMP_Text _gameOverCountdownText;
        [SerializeField] private float _gameOverCountdownValue = 5f;
        [SerializeField] private GameObject _winnerDeclarationPanel;
        [SerializeField] private GameObject _drawDeclarationPanel;

        [Header("Disconnected PopUp")]
        [SerializeField] private GameObject _disconnectedPopUp;
        [SerializeField] private TMP_Text _disconnectedCountdownCharSelectUIText;
        [SerializeField] float _disconnectedCountdownTime = 5f;

        private bool _isGameResultSet = false;
        private bool _isCountDownOngoing = false;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventNameEnum.SetWinnerOnGameOverUI, HandleWinnerDetailsOnGameOverUI);
            EventBusManager.Instance.Subscribe(EventNameEnum.StartGameOverCountdown, HandleStartGameOverCountdownUI);
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventNameEnum.SetWinnerOnGameOverUI, HandleWinnerDetailsOnGameOverUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.StartGameOverCountdown, HandleStartGameOverCountdownUI);
            
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            }
        }

        private void HandleWinnerDetailsOnGameOverUI(object[] parameters)
        {
            SetGameResult();
            _isGameResultSet = true;
            EnableView();
        }

        private void HandleStartGameOverCountdownUI(object[] parameters)
        {
            StartCoroutine(GameOverCountdownSequence());
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        private void SetGameResult()
        {
            if(MultiplayerManager.Instance.gameResult.Value)
            {
                SetWinnerDetails();
                _winnerDeclarationPanel.SetActive(true);
                _drawDeclarationPanel.SetActive(false);
            }
            else
            {
                _winnerDeclarationPanel.SetActive(false);
                _drawDeclarationPanel.SetActive(true);
            }
        }

        private void SetWinnerDetails()
        {
            switch(MultiplayerManager.Instance.winnerPlayer.Value)
            {
                case PlayerTurnEnum.Player1:
                    _winnerUserNameText.text = PlayerSessionDataManager.Instance.GetPlayerSessionData(NetworkManager.Singleton.ConnectedClients[0].ClientId).username.ToString();
                    _winnerDeclarationPanel.SetActive(true);
                    break;
                case PlayerTurnEnum.Player2:
                    _winnerUserNameText.text = PlayerSessionDataManager.Instance.GetPlayerSessionData(NetworkManager.Singleton.ConnectedClients[1].ClientId).username.ToString();
                    _winnerDeclarationPanel.SetActive(true);
                    break;
            }
        }

        private IEnumerator GameOverCountdownSequence()
        {
            _isCountDownOngoing = true;
            float currentTime = _gameOverCountdownValue;

            while (currentTime > 0)
            {
                _gameOverCountdownText.text = Mathf.Ceil(currentTime).ToString() + "s";
                currentTime -= 1f;
                yield return new WaitForSeconds(1f);
            }

            CleanUp();
            SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
        }

        private void CleanUp()
        {
            GameManager.Instance.Get<CleanUpService>().CleanUpMultiplayerManager();
            GameManager.Instance.Get<CleanUpService>().CleanUpNetworkManager();
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            if (_isGameResultSet)
            {
                if(!_isCountDownOngoing)
                {
                    StartCoroutine(GameOverCountdownSequence());
                }
            }
            else
            {
                GameManager.Instance.Get<CleanUpService>().CleanUpPlayerSessionDataManager();
                GameManager.Instance.Get<CleanUpService>().CleanUpLobbyRelay();
                GameManager.Instance.Get<CleanUpService>().ResetServices();
                ShowDisconnectedNotification();
                StartCoroutine(DisconnectedCountdownSequence());
            }

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

            HideDisconnectedNotification();
            SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
        }

        private void HideDisconnectedNotification()
        {
            _disconnectedPopUp.SetActive(false);
        }
    }
}
