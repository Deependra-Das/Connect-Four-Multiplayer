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
        TakeTurn,
        ChangePlayerTurn,
        GameOver,
        PlayerGiveUp,
    }
}