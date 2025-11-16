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
            SetWinnerDetails();
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

        private void SetWinnerDetails()
        {
            switch(MultiplayerManager.Instance.winnerPlayer.Value)
            {
                case PlayerTurnEnum.Player1:
                    _winnerUserNameText.text = PlayerSessionDataManager.Instance.GetPlayerSessionData(NetworkManager.Singleton.ConnectedClients[0].ClientId).username.ToString();
                    break;
                case PlayerTurnEnum.Player2:
                    _winnerUserNameText.text = PlayerSessionDataManager.Instance.GetPlayerSessionData(NetworkManager.Singleton.ConnectedClients[1].ClientId).username.ToString();
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
