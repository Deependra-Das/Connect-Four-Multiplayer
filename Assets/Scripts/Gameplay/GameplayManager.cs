using ConnectFourMultiplayer.Board;
using ConnectFourMultiplayer.Disk;
using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
using ConnectFourMultiplayer.Utilities;
using UnityEngine;

namespace ConnectFourMultiplayer.Gameplay
{
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
                        break;
                    case PlayerTurnEnum.Player2:
                        GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskYellow, _spawnLocations[colIndex].position);
                        break;
                }

                int rowIndex = GameManager.Instance.Get<BoardService>().LastAddedCellRow;
                if (CheckForWin(rowIndex, colIndex))
                {
                    HandleWin();
                    return;
                }

                _playerTurn = (_playerTurn == PlayerTurnEnum.Player1) ? PlayerTurnEnum.Player2 : PlayerTurnEnum.Player1;
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

        private bool CheckForWin(int rowIndex, int colIndex)
        {
            int playerValue = (int)_playerTurn;

            return CheckDirection(rowIndex, colIndex, 0, 1, playerValue) ||
                   CheckDirection(rowIndex, colIndex, 1, 0, playerValue) ||
                   CheckDirection(rowIndex, colIndex, 1, 1, playerValue) ||
                   CheckDirection(rowIndex, colIndex, 1, -1, playerValue);
        }

        private bool CheckDirection(int row, int col, int dirRow, int dirCol, int playerValue)
        {
            int count = 1;

            count += CountConsecutiveDiscs(row, col, dirRow, dirCol, playerValue);
            count += CountConsecutiveDiscs(row, col, -dirRow, -dirCol, playerValue);

            return count >= 4;
        }

        private int CountConsecutiveDiscs(int row, int col, int dirRow, int dirCol, int playerValue)
        {
            int count = 0;
            int currentRow = row + dirRow;
            int currentColumn = col + dirCol;
            int rowCount = GameManager.Instance.Get<BoardService>().RowCount;
            int colCount = GameManager.Instance.Get<BoardService>().ColumnCount;

            while (currentRow >= 0 && currentRow < rowCount && currentColumn >= 0 && currentColumn < colCount)
            {
                if (GameManager.Instance.Get<BoardService>().GetBoardCellValue(currentRow, currentColumn) == playerValue)
                {
                    count++;
                    currentRow += dirRow;
                    currentColumn += dirCol;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        private void HandleWin()
        {
            Debug.Log("Player " + _playerTurn + " Won.");
        }
    }
}
