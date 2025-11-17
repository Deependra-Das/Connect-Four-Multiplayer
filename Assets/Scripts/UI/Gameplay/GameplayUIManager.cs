using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
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

        [Header("Notification PopUp")]
        [SerializeField] private GameObject _notificationPopUp;
        [SerializeField] private TMP_Text _notificationMessageText;

        [Header("Disconnected PopUp")]
        [SerializeField] private GameObject _disconnectedPopUp;
        [SerializeField] private TMP_Text _disconnectedCountdownCharSelectUIText;
        [SerializeField] float _disconnectedCountdownTime = 5f;

        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        [SerializeField] private RectTransform _uiElementToSlide;
        [SerializeField] private Image _uiElementImage;
        [SerializeField] private float _slideDuration = 1f;
        [SerializeField] private float _messageDisplayDuration = 3f;

        [SerializeField] private Color _leftColor = Color.red;
        [SerializeField] private Color _rightColor = Color.blue;
        [SerializeField] private Color _centerColor = Color.black;

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
            EventBusManager.Instance.Subscribe(EventNameEnum.GameDraw, HandleGameDraw);
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnectCallbackGameplayUI;
        }

        private void UnsubscribeFromEvents()
        {
            _giveUpButton.onClick.RemoveListener(OnGiveUpButtonClicked);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.ChangePlayerTurn, HandleChangePlayerTurn);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.GameOver, HandleGameOver);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.PlayerGiveUp, HandlePlayerGiveUp);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.GameDraw, HandleGameDraw);

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnectCallbackGameplayUI;
            }
        }

        void Start()
        {
            HideMessagePanel();
            SetPlayerUserName();
        }

        private void SetPlayerUserName()
        {
            if (NetworkManager.Singleton.ConnectedClients.Count == MultiplayerManager.MAX_LOBBY_SIZE)
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
            _notificationMessageText.text = "Player " + _playerWhoGaveUp + " Gave Up";
            StartCoroutine(ShowHideMessagePanel(_messageDisplayDuration));
        }

        private void HandleGameDraw(object[] parameters)
        {
            _notificationMessageText.text = "The board is full - No more turns left";
            StartCoroutine(ShowHideMessagePanel(_messageDisplayDuration));
        }

        private void ShowMessagePanel()
        {
            _notificationPopUp.SetActive(true);
        }

        private void HideMessagePanel()
        {
            _notificationPopUp.SetActive(false);
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

        private void HandleClientDisconnectCallbackGameplayUI(ulong clientID)
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogWarning("NetworkManager.Singleton is null.");
                return;
            }

            if ((NetworkManager.Singleton.IsServer && clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClients.Count <= 1) || !NetworkManager.Singleton.IsServer)
            {
                _giveUpButton.interactable = false;
                ShowDisconnectedNotification();
                StartCoroutine(DisconnectedCountdownSequence());
            }
            else if(NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count <= 1)
            {
                _giveUpButton.interactable = false;
                 string disconnectedPlayer = PlayerSessionDataManager.Instance.GetPlayerSessionData(clientID).username.ToString();
                _notificationMessageText.text = disconnectedPlayer + " Disconnected from the Game";
                StartCoroutine(ShowHideMessagePanel(3f));
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
    }
}