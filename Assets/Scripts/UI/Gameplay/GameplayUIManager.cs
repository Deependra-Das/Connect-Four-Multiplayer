using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Network;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectFourMultiplayer.UI
{
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _p1UsernameText;
        [SerializeField] private TMP_Text _p2UsernameText;
        [SerializeField] private GameObject _gameplayNotificationPanel;
        [SerializeField] private TMP_Text _playerNotificationText;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private float _slideDuration;
        [SerializeField] private float waitDuration; 
        [SerializeField] private float _offsetY;
        [SerializeField] private GameObject _messagePanelGameplayUI;
        [SerializeField] private TMP_Text _messageTextGameplayUI;

        private Vector3 _topPos;
        private Vector3 _bottomPos;
        private Coroutine _currentAnimation;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);
            EventBusManager.Instance.Subscribe(EventNameEnum.TakeTurn, HandleTakeTurnGameplayUI);
            EventBusManager.Instance.Subscribe(EventNameEnum.ChangePlayerTurn, HandleChangePlayerTurnGameplayUI);
            EventBusManager.Instance.Subscribe(EventNameEnum.GameOver, HandleGameOverGameplayUI);
            EventBusManager.Instance.Subscribe(EventNameEnum.PlayerGiveUp, HandlePlayerGiveUpGameplayUI);
        }

        private void UnsubscribeToEvents()
        {
            _giveUpButton.onClick.RemoveListener(OnGiveUpButtonClicked);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.TakeTurn, HandleTakeTurnGameplayUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.ChangePlayerTurn, HandleChangePlayerTurnGameplayUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.GameOver, HandleGameOverGameplayUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.PlayerGiveUp, HandlePlayerGiveUpGameplayUI);
        }

        void Start()
        {
            _topPos = _gameplayNotificationPanel.transform.position;
            _bottomPos = new Vector3(_topPos.x, _topPos.y - _offsetY, _topPos.z);
            HideMessageUI();
        }

        private void OnGiveUpButtonClicked()
        {
            _giveUpButton.interactable = false;
            GameplayManager.Instance.HandlePlayerGiveUpGameplay();
        }

        public void SlideDownPlayerTurnPanel()
        {
            if (_currentAnimation != null) StopCoroutine(_currentAnimation);
            _currentAnimation = StartCoroutine(AnimatePlayerTurnPanelPosition(_topPos, _bottomPos));
        }

        public void SlideUpPlayerTurnPanel()
        {
            if (_currentAnimation != null) StopCoroutine(_currentAnimation);
            _currentAnimation = StartCoroutine(AnimatePlayerTurnPanelPosition(_bottomPos, _topPos));
        }

        IEnumerator AnimatePlayerTurnPanelPosition(Vector3 currentPos, Vector3 newPos)
        {
            float elapsedTime = 0f;
            while (elapsedTime < _slideDuration)
            {
                _gameplayNotificationPanel.transform.position = Vector3.Lerp(currentPos, newPos, elapsedTime / _slideDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _gameplayNotificationPanel.transform.position = newPos;
        }

        private void HandleTakeTurnGameplayUI(object[] parameters)
        {
            SlideUpPlayerTurnPanel();
        }

        private void HandleChangePlayerTurnGameplayUI(object[] parameters)
        {
            int playerTurn = (int)parameters[0];

            StartCoroutine(WaitBeforeSlideDown(playerTurn, waitDuration));
        }

        IEnumerator WaitBeforeSlideDown(int playerTurn, float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            SetPlayerTurnText(playerTurn);
            SlideDownPlayerTurnPanel();
        }

        private void SetPlayerTurnText(int playerTurn)
        {
            _playerNotificationText.text = "Player " + playerTurn + " Turn";
        }

        private void HandleGameOverGameplayUI(object[] parameters)
        {
            _giveUpButton.interactable = false;
        }

        private void HandlePlayerGiveUpGameplayUI(object[] parameters)
        {
            int playerGaveUp = (int)parameters[0];
            _giveUpButton.interactable = false;
            _messageTextGameplayUI.text = "Player " + playerGaveUp + " Gave Up";
            StartCoroutine(ShowHideMesageUI(3f));
        }

        private void ShowMessageUI()
        {
            _messagePanelGameplayUI.SetActive(true);
        }

        private void HideMessageUI()
        {
            _messagePanelGameplayUI.SetActive(false);
        }

        IEnumerator ShowHideMesageUI(float waitDuration)
        {
            ShowMessageUI();
            yield return new WaitForSeconds(waitDuration);
            HideMessageUI();
        }
    }
}