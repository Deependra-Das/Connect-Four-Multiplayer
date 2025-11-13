using ConnectFourMultiplayer.Event;
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
        [SerializeField] private float _offsetY;

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
        }

        private void UnsubscribeToEvents()
        {
            _giveUpButton.onClick.RemoveListener(OnGiveUpButtonClicked);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.TakeTurn, HandleTakeTurnGameplayUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.ChangePlayerTurn, HandleChangePlayerTurnGameplayUI);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.GameOver, HandleGameOverGameplayUI);
        }

        void Start()
        {
            _topPos = _gameplayNotificationPanel.transform.position;
            _bottomPos = new Vector3(_topPos.x, _topPos.y - _offsetY, _topPos.z);
        }

        private void OnGiveUpButtonClicked()
        {
            EventBusManager.Instance.Raise(EventNameEnum.PlayerGiveUp);
            _giveUpButton.interactable = false;
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
            float waitDuration = (float)parameters[1];

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
            int playerTurn = (int)parameters[0];

            StartCoroutine(WinnerNotificationSlideDown(playerTurn, 1f));
        }

        IEnumerator WinnerNotificationSlideDown(int playerTurn, float waitDuration)
        {
            SlideUpPlayerTurnPanel();
            yield return new WaitForSeconds(waitDuration);
            _playerNotificationText.text = "Player " + playerTurn + " Won";
            SlideDownPlayerTurnPanel();
        }
    }
}