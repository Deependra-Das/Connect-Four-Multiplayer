using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Network;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectFourMultiplayer.UI
{
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _player1UsernameText;
        [SerializeField] private TMP_Text _player2UsernameText;
        [SerializeField] private TMP_Text _playerNotificationText;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private GameObject _messagePanel;
        [SerializeField] private TMP_Text _messageText;

        public HorizontalLayoutGroup _layoutGroup;
        public RectTransform _uiElementToSlide;
        public Image _uiElementImage;
        public float _slideDuration = 1f;

        public Color _leftColor = Color.red;
        public Color _rightColor = Color.blue;
        public Color _centerColor = Color.black;

        private RectTransform _layoutRectTransform;

        private void Awake()
        {
            _layoutRectTransform = _layoutGroup.GetComponent<RectTransform>();
        }

        private void OnEnable() => SubscribeToEvents();
        private void OnDisable() => UnsubscribeFromEvents();

        private void SubscribeToEvents()
        {
            _giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);
            EventBusManager.Instance.Subscribe(EventNameEnum.ChangePlayerTurn, HandleChangePlayerTurn);
            EventBusManager.Instance.Subscribe(EventNameEnum.GameOver, HandleGameOver);
            EventBusManager.Instance.Subscribe(EventNameEnum.PlayerGiveUp, HandlePlayerGiveUp);
        }

        private void UnsubscribeFromEvents()
        {
            _giveUpButton.onClick.RemoveListener(OnGiveUpButtonClicked);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.ChangePlayerTurn, HandleChangePlayerTurn);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.GameOver, HandleGameOver);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.PlayerGiveUp, HandlePlayerGiveUp);
        }

        void Start()
        {
            HideMessagePanel();
            SetPlayerUserName();
        }

        private void SetPlayerUserName()
        {
            if (NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                PlayerSessionData p1SessionData = PlayerSessionDataManager.Instance.GetPlayerSessionData(NetworkManager.Singleton.ConnectedClients[0].ClientId);
                PlayerSessionData p2SessionData = PlayerSessionDataManager.Instance.GetPlayerSessionData(NetworkManager.Singleton.ConnectedClients[1].ClientId);

                _player1UsernameText.text = p1SessionData.username.ToString();
                _player2UsernameText.text = p2SessionData.username.ToString();
            }
        }

        private void OnGiveUpButtonClicked()
        {
            _giveUpButton.interactable = false;
            GameplayManager.Instance.HandlePlayerGiveUpGameplay();
        }

        private void HandleChangePlayerTurn(object[] parameters)
        {
            int _currentPlayerTurn = (int)parameters[0];

            switch (_currentPlayerTurn)
            {
                case 1:
                    _playerNotificationText.text = "Player " + _currentPlayerTurn + " Turn";
                    SlideLeft();
                    break;

                case 2:
                    _playerNotificationText.text = "Player " + _currentPlayerTurn + " Turn";
                    SlideRight();
                    break;
            }
        }

        private void HandleGameOver(object[] parameters)
        {
            _giveUpButton.interactable = false;
            _playerNotificationText.text = "Game Over";
            SlideToCenter();

        }

        private void HandlePlayerGiveUp(object[] parameters)
        {
            int _playerWhoGaveUp = (int)parameters[0];
            _giveUpButton.interactable = false;
            _messageText.text = "Player " + _playerWhoGaveUp + " Gave Up";
            StartCoroutine(ShowHideMessagePanel(3f));
        }

        private void ShowMessagePanel()
        {
            _messagePanel.SetActive(true);
        }

        private void HideMessagePanel()
        {
            _messagePanel.SetActive(false);
        }

        IEnumerator ShowHideMessagePanel(float duration)
        {
            ShowMessagePanel();
            yield return new WaitForSeconds(duration);
            HideMessagePanel();
        }

        public void SlideLeft() => StartSlide(TextAnchor.MiddleLeft, _leftColor);
        public void SlideToCenter() => StartSlide(TextAnchor.MiddleCenter, _centerColor);  // Use center color here
        public void SlideRight() => StartSlide(TextAnchor.MiddleRight, _rightColor);

        private void StartSlide(TextAnchor alignment, Color targetColor)
        {
            Vector2 _startPosition = _uiElementToSlide.anchoredPosition;

            _layoutGroup.childAlignment = alignment;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutRectTransform);

            Vector2 _endPosition = _uiElementToSlide.anchoredPosition;

            LayoutElement _layoutElement = _uiElementToSlide.GetComponent<LayoutElement>();
            if (_layoutElement == null) _layoutElement = _uiElementToSlide.gameObject.AddComponent<LayoutElement>();
            _layoutElement.ignoreLayout = true;

            _uiElementToSlide.anchoredPosition = _startPosition;

            StopAllCoroutines();
            StartCoroutine(SlideToPosition(_startPosition, _endPosition, _layoutElement, targetColor));
        }

        private IEnumerator SlideToPosition(Vector2 start, Vector2 end, LayoutElement layoutElement, Color targetColor)
        {
            float _elapsedTime = 0f;

            while (_elapsedTime < _slideDuration)
            {
                _elapsedTime += Time.deltaTime;
                float _normalizedTime = Mathf.Clamp01(_elapsedTime / _slideDuration);
                _uiElementToSlide.anchoredPosition = Vector2.Lerp(start, end, _normalizedTime);

                if (_uiElementImage != null)
                {
                    _uiElementImage.color = Color.Lerp(_uiElementImage.color, targetColor, _normalizedTime);
                }

                yield return null;
            }

            _uiElementToSlide.anchoredPosition = end;
            layoutElement.ignoreLayout = false;

            if (_uiElementImage != null)
            {
                _uiElementImage.color = targetColor;
            }
        }
    }
}