using UnityEngine;

public class GameplayManager : GenericMonoSingleton<GameplayManager>
{
    [SerializeField] private Transform[] _spawnLocations;

    private PlayerTurnEnum _playerTurn = PlayerTurnEnum.None;

    public void Initialize()
    {
        GameManager.Instance.Get<BoardService>().InitializeBoard();
        GameManager.Instance.Get<DiskPreviewService>().Initialize(_spawnLocations[0].position);
        _playerTurn = PlayerTurnEnum.Player1;
    }

    public void TakeTurn(int colIndex)
    {
        if (UpdateBoardState(colIndex, (int)_playerTurn))
        {
            EventBusManager.Instance.Raise(EventNameEnum.TakeTurn, _playerTurn);

            switch (_playerTurn)
            {
                case PlayerTurnEnum.Player1:
                    GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskRed, _spawnLocations[colIndex].position);
                    _playerTurn = PlayerTurnEnum.Player2;
                    break;
                case PlayerTurnEnum.Player2:
                    GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskYellow, _spawnLocations[colIndex].position);
                    _playerTurn = PlayerTurnEnum.Player1;
                    break;
            }
        }        
    }

    private bool UpdateBoardState(int col, int value)
    {
        return GameManager.Instance.Get<BoardService>().SetBoardCellValue(col, value);
    }

    public void OnHoverOverColumn(int colIndex)
    {
        GameManager.Instance.Get<DiskPreviewService>().HandleHoverOverColumnDiskPreview(_playerTurn, _spawnLocations[colIndex].position);
    }

}
