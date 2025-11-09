using UnityEngine;

public class GameplayManager : GenericMonoSingleton<GameplayManager>
{
    [SerializeField] private Transform[] _spawnLocations;

    PlayerTurnEnum playerTurn = PlayerTurnEnum.None;

    public void Initialize()
    {
        GameManager.Instance.Get<BoardService>().InitializeBoard();
        playerTurn = PlayerTurnEnum.Player1;
    }

    public void TakeTurn(int colIndex)
    {
        if (UpdateBoardState(colIndex, (int)playerTurn))
            {
            switch (playerTurn)
            {
                case PlayerTurnEnum.Player1:
                    GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskRed, _spawnLocations[colIndex].position);
                    playerTurn = PlayerTurnEnum.Player2;
                    break;
                case PlayerTurnEnum.Player2:
                    GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskYellow, _spawnLocations[colIndex].position);
                    playerTurn = PlayerTurnEnum.Player1;
                    break;
            }
        }        
    }

    private bool UpdateBoardState(int col, int value)
    {
        return GameManager.Instance.Get<BoardService>().SetBoardCellValue(col, value);
    }
}
