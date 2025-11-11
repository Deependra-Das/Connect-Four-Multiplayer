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

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeToEvents();

    private void SubscribeToEvents()
    {
        _giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);
    }

    private void UnsubscribeToEvents()
    {
        _giveUpButton.onClick.RemoveListener(OnGiveUpButtonClicked);    }


    void Start()
    {
        
    }

    private void OnGiveUpButtonClicked()
    {
    }
}
