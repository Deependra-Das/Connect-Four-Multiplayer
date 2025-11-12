using ConnectFourMultiplayer.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _notReadyButton;
    [SerializeField] private Button _leaveLobbyButton;

    [Header("Leave Lobby PopUp")]
    [SerializeField] private GameObject _leaveLobbyConfirmationPopUp;
    [SerializeField] private TMP_Text _hostLobbyNoticeText;
    [SerializeField] private Button _yesConfirmationButton;
    [SerializeField] private Button _noConfirmationButton;


    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();

    private void SubscribeToEvents()
    {
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _notReadyButton.onClick.AddListener(OnNotReadyButtonClicked);
        _leaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClicked);
        _yesConfirmationButton.onClick.AddListener(OnYesButtonClicked);
        _noConfirmationButton.onClick.AddListener(OnNoButtonClicked);
    }

    private void UnsubscribeToEvents()
    {
        _readyButton.onClick.RemoveListener(OnReadyButtonClicked);
        _notReadyButton.onClick.RemoveListener(OnNotReadyButtonClicked);
        _leaveLobbyButton.onClick.RemoveListener(OnLeaveLobbyButtonClicked);
        _yesConfirmationButton.onClick.RemoveListener(OnYesButtonClicked);
        _noConfirmationButton.onClick.RemoveListener(OnNoButtonClicked);
    }

    void Start()
    {
        _notReadyButton.gameObject.SetActive(false);
        _readyButton.gameObject.SetActive(true);
        HideBackToMainMenuConfirmationPopup();
    }

    private void OnReadyButtonClicked()
    {
        _readyButton.gameObject.SetActive(false);
        _notReadyButton.gameObject.SetActive(true);

        SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene, true);
    }

    private void OnNotReadyButtonClicked()
    {
        _notReadyButton.gameObject.SetActive(false);
        _readyButton.gameObject.SetActive(true);
    }

    private void OnLeaveLobbyButtonClicked()
    {
        ShowBackToMainMenuConfirmationPopup();
    }

    private void ShowBackToMainMenuConfirmationPopup()
    {
        _leaveLobbyConfirmationPopUp.SetActive(true);
    }

    private void OnYesButtonClicked()
    {
        HideBackToMainMenuConfirmationPopup();
        SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
    }

    private void OnNoButtonClicked()
    {
        HideBackToMainMenuConfirmationPopup();
    }
    private void HideBackToMainMenuConfirmationPopup()
    {
        _leaveLobbyConfirmationPopUp.SetActive(false);
    }
}
