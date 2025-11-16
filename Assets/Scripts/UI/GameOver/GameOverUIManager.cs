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

        void Start()
        {
            SetWinnerDetails();
            EnableView();
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

            SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
        }
    }
}
