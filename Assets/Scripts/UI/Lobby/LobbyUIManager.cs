using ConnectFourMultiplayer.Main;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _notReadyButton;
    [SerializeField] private Button _leaveLobbyButton;


    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();

    private void SubscribeToEvents()
    {
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _notReadyButton.onClick.AddListener(OnNotReadyButtonClicked);
        _leaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClicked);
    }

    private void UnsubscribeToEvents()
    {
        _readyButton.onClick.RemoveListener(OnReadyButtonClicked);
        _notReadyButton.onClick.RemoveListener(OnNotReadyButtonClicked);
        _leaveLobbyButton.onClick.RemoveListener(OnLeaveLobbyButtonClicked);
    }

    void Start()
    {


    }

    private void OnReadyButtonClicked()
    {
        _readyButton.gameObject.SetActive(false);
        _notReadyButton.gameObject.SetActive(true);

        SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene, false);
    }

    private void OnNotReadyButtonClicked()
    {
        _notReadyButton.gameObject.SetActive(false);
        _readyButton.gameObject.SetActive(true);
    }

    private void OnLeaveLobbyButtonClicked()
    {
    }

}
