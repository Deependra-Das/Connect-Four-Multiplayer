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
        TakeTurn,
        ChangePlayerTurn,
        GameOver,
        PlayerGiveUp,
    }
}