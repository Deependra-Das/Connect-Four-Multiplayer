using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Gameplay;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _p1UsernameText;
    [SerializeField] private TMP_Text _p2UsernameText;
    [SerializeField] private GameObject _playerTurnPanel;
    [SerializeField] private TMP_Text _playerTurnText;
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
    }

    private void UnsubscribeToEvents()
    {
        _giveUpButton.onClick.RemoveListener(OnGiveUpButtonClicked);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.TakeTurn, HandleTakeTurnGameplayUI);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.ChangePlayerTurn, HandleChangePlayerTurnGameplayUI);
    }

    void Start()
    {
         _topPos = _playerTurnPanel.transform.position;
        _bottomPos = new Vector3(_topPos.x, _topPos.y - _offsetY, _topPos.z);
    }

    private void OnGiveUpButtonClicked()
    {
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
            _playerTurnPanel.transform.position = Vector3.Lerp(currentPos, newPos, elapsedTime / _slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _playerTurnPanel.transform.position = newPos;
    }

    private void HandleTakeTurnGameplayUI(object[] parameters)
    {
        SlideUpPlayerTurnPanel();
    }

    private void HandleChangePlayerTurnGameplayUI(object[] parameters)
    {
        int playerTurn = (int)parameters[0];
        float waitDuration = (float)parameters[1];

        StartCoroutine(WaitBeforeSlideDown(playerTurn,waitDuration));    
    }

    IEnumerator WaitBeforeSlideDown(int playerTurn, float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        SetPlayerTurnText(playerTurn);
        SlideDownPlayerTurnPanel();
    }

    private void SetPlayerTurnText(int playerTurn)
    {
        _playerTurnText.text = "Player " + playerTurn + " Turn";
    }
}
