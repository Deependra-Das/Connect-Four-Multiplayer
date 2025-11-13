using TMPro;
using UnityEngine;

public class LobbyParticipantUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _usernameText;
    [SerializeField] private TMP_Text _winsCountText;
    [SerializeField] private GameObject _readyPanel;
    [SerializeField] private GameObject _notReadyPanel;

    private ulong _clientId = 0;


    public void Initialize(PlayerSessionData playerSessionData)
    {
        SetClientId(playerSessionData.clientId);
        SetUsername(playerSessionData.username.ToString());
        SetWinsCount(playerSessionData.winsCount.ToString());
    }

    private void SetClientId(ulong assignedClientID)
    {
        _clientId = assignedClientID;
    }

    public void SetUsername(string username)
    {
        _usernameText.text = username;
    }

    private void SetWinsCount(string winsCount)
    {
        _winsCountText.text = winsCount;
    }

}
