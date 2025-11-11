using ConnectFourMultiplayer.Main;
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
    }

    private void UnsubscribeToEvents()
    {
        _hostGameButton.onClick.RemoveListener(OnHostGameButtonClicked);
        _joinGameButton.onClick.RemoveListener(OnJoinGameButtonClicked);    
        _howToPlayButton.onClick.RemoveListener(OnHowToPlayButonClicked);
        _quitGameButton.onClick.RemoveListener(OnQuitGameButtonClicked);
        _changeUsernameButton.onClick.RemoveListener(OnChangeUsernameButtonClicked);
    }

    private void Start()
    {
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
        SceneLoader.Instance.LoadScene(SceneNameEnum.LobbyScene, false);
    }

    private void OnJoinGameButtonClicked()
    {
        SceneLoader.Instance.LoadScene(SceneNameEnum.LobbyScene, false);
    }

    private void OnQuitGameButtonClicked()
    {
        Application.Quit();
    }

    private void OnHowToPlayButonClicked()
    {
    }

    private void OnChangeUsernameButtonClicked()
    {
    }
}
