using UnityEngine;

namespace ConnectFourMultiplayer.Event
{
    public enum EventNameEnum
    {
        ChangeGameState,
        CreateLobbyStarted,
        CreateLobbyFailed,
        JoinStarted,
        QuickJoinFailed,
        PlayerJoined,
        PlayerLeft,
        PlayerLobbyStateChanged,
        TakeTurn,
        EnableColumnInput,
        ChangePlayerTurn,
        GameOver,
        PlayerGiveUp,
    }
}