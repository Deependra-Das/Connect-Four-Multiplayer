using UnityEngine;

namespace ConnectFourMultiplayer.Event
{
    public enum EventNameEnum
    {
        ChangeGameState,
        SceneLoaded,
        CreateLobbyStarted,
        CreateLobbyFailed,
        TryingToJoinGame,
        FailedToJoinGame,
        JoinStarted,
        QuickJoinFailed,
        PlayerJoined,
        JoinFailed,
        PlayerLeft,
        PlayerLobbyStateChanged,
        TakeTurn,
        EnableColumnInput,
        DisableColumnInput,        
        ChangePlayerTurn,
        GameOver,
        PlayerGiveUp,
        SetWinnerOnGameOverUI,
        StartGameOverCountdown
    }
}