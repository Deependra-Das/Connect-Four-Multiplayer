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

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventNameEnum.SetWinnerOnGameOverUI, HandleWinnerDetailsOnGameOverUI);
            EventBusManager.Instance.Subscribe(EventNameEnum.StartGameOverCountdown, HandleStartGameOverCountdownUI);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventNameEnum.SetWinnerOnGameOverUI, HandleWinnerDetailsOnGameOverUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.StartGameOverCountdown, HandleStartGameOverCountdownUI);
        }

        private void HandleWinnerDetailsOnGameOverUI(object[] parameters)
        {
            SetGameResult();
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
    }
}
