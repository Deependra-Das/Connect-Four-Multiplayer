using ConnectFourMultiplayer.Main;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Main Menu Content")]
    [SerializeField] private GameObject _topBar;
    [SerializeField] private TMP_Text _usernameDisplayText;
    [SerializeField] private Button _changeUsernameButton;
    [SerializeField] private Button _hostGameButton;
    [SerializeField] private Button _joinGameButton;
    [SerializeField] private Button _howToPlayButton;
    [SerializeField] private Button _quitGameButton;

    [Header("Notification PopUp")]
    [SerializeField] private GameObject _notificationPopup;
    [SerializeField] private TMP_Text _notificationMessageText;
    [SerializeField] private Button _okButton;

    [Header("Change Username PopUp")]
    [SerializeField] private Button _saveUsernameButton;
    [SerializeField] private Button _cancelUsernameChangeButton;
    [SerializeField] private GameObject _usernameInputPopup;
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_Text _errorMessageText;

    private const int _minUserNameLength = 3;
    private const int _maxUserNameLength = 15;
    private const string _defaultUsername = "Player";

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();

    private void SubscribeToEvents()
    {
        _hostGameButton.onClick.AddListener(OnHostGameButtonClicked);
        _joinGameButton.onClick.AddListener(OnJoinGameButtonClicked);
        _howToPlayButton.onClick.AddListener(OnHowToPlayButonClicked);
        _quitGameButton.onClick.AddListener(OnQuitGameButtonClicked);
        _changeUsernameButton.onClick.AddListener(OnChangeUsernameButtonClicked);
        _saveUsernameButton.onClick.AddListener(OnSaveUsernameButtonClicked);
        _cancelUsernameChangeButton.onClick.AddListener(OnCancelUsernameChangeButtonClicked);
        _okButton.onClick.AddListener(OnOkButtonClicked);
    }

    private void UnsubscribeToEvents()
    {
        _hostGameButton.onClick.RemoveListener(OnHostGameButtonClicked);
        _joinGameButton.onClick.RemoveListener(OnJoinGameButtonClicked);    
        _howToPlayButton.onClick.RemoveListener(OnHowToPlayButonClicked);
        _quitGameButton.onClick.RemoveListener(OnQuitGameButtonClicked);
        _changeUsernameButton.onClick.RemoveListener(OnChangeUsernameButtonClicked);
        _saveUsernameButton.onClick.RemoveListener(OnSaveUsernameButtonClicked);
        _cancelUsernameChangeButton.onClick.RemoveListener(OnCancelUsernameChangeButtonClicked);
        _okButton.onClick.RemoveListener(OnOkButtonClicked);
    }

    private void Start()
    {
        CheckPlayerNameExists();
        EnableView();
    }

    public void EnableView()
    {
        gameObject.SetActive(true);
    }
    public void DisableView()
    {
        gameObject.SetActive(false);
    }

    private void OnHostGameButtonClicked()
    {
        GameManager.Instance.Get<GameStateService>().ChangeState(GameStateEnum.Lobby);
    }

    private void OnJoinGameButtonClicked()
    {
        GameManager.Instance.Get<GameStateService>().ChangeState(GameStateEnum.Lobby);
    }

    private void OnQuitGameButtonClicked()
    {
        Application.Quit();
    }

    private void OnHowToPlayButonClicked()
    {
    }
    private void OnOkButtonClicked()
    {
        HideNotificationPopup();
    }

    void CheckPlayerNameExists()
    {
        string username = PlayerPrefs.GetString(GameManager.UsernameKey).ToString();

        if (string.IsNullOrEmpty(username))
        {
            _topBar.SetActive(false);
            ShowUsernameInputPopup();
        }
        else
        {
            _usernameDisplayText.text = username;
            _topBar.SetActive(true);
        }
    }

    private void OnSaveUsernameButtonClicked()
    {
        string username = _usernameInputField.text;

        if (string.IsNullOrEmpty(username))
        {
            _errorMessageText.text = "Username cannot be empty!";
            return;
        }
        if (username.Length < _minUserNameLength || username.Length > _maxUserNameLength)
        {
            _errorMessageText.text = "Username must be 3-16 characters long!";
            return;
        }

        if (!IsValidPlayerName(username))
        {
            _errorMessageText.text = "Username contains invalid characters!";
            return;
        }

        PlayerPrefs.SetString(GameManager.UsernameKey, username);
        PlayerPrefs.Save();

        _errorMessageText.text = string.Empty;

        HideUsernameInputPopup();
        CheckPlayerNameExists();
        _notificationMessageText.text = "Username Saved Successfully!";
        ShowNotificationPopup();
    }

    private bool IsValidPlayerName(string username)
    {
        string pattern = "^[A-Za-z0-9]+$";
        return Regex.IsMatch(username, pattern);
    }

    private void OnChangeUsernameButtonClicked()
    {
        ShowUsernameInputPopup();
        HideNotificationPopup();
    }

    private void ShowUsernameInputPopup()
    {
        _usernameInputField.text = string.Empty;
        _errorMessageText.text = string.Empty;
        _usernameInputPopup.SetActive(true);
    }
    private void OnCancelUsernameChangeButtonClicked()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(GameManager.UsernameKey).ToString()))
        {
            PlayerPrefs.SetString(GameManager.UsernameKey, _defaultUsername);
            PlayerPrefs.Save();
            _usernameDisplayText.text = _defaultUsername;
            _topBar.SetActive(true);
        }
        HideUsernameInputPopup();
    }

    private void HideUsernameInputPopup()
    {
        _errorMessageText.text = string.Empty;
        _usernameInputPopup.SetActive(false);
    }

    private void ShowNotificationPopup()
    {
        _notificationPopup.SetActive(true);
    }

    private void HideNotificationPopup()
    {
        _notificationPopup.SetActive(false);
    }

}
