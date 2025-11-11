using ConnectFourMultiplayer.Board;
using ConnectFourMultiplayer.Disk;
using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
using ConnectFourMultiplayer.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectFourMultiplayer.Gameplay
{
    public class GameplayManager : GenericMonoSingleton<GameplayManager>
    {
        [SerializeField] private Transform[] _spawnLocations;
        [SerializeField] private GameObject _highlightPrefab;

        private PlayerTurnEnum _playerTurn = PlayerTurnEnum.None;
        private GameObject[,] spawnedDisks;
        private int _rowCount = 0;
        private int _colCount = 0;

        private void Start()
        {
            GameManager.Instance.Get<BoardService>().InitializeBoard();
            _rowCount = GameManager.Instance.Get<BoardService>().RowCount;
            _colCount = GameManager.Instance.Get<BoardService>().ColumnCount;
            InitializeSpawnedDiskMatrix(_rowCount, _colCount);
            GameManager.Instance.Get<DiskPreviewService>().Initialize(_spawnLocations[0].position);
            _playerTurn = PlayerTurnEnum.Player1;
            EventBusManager.Instance.Raise(EventNameEnum.ChangePlayerTurn, _playerTurn, 0f);
        }

        public void TakeTurn(int colIndex)
        {
            if (UpdateBoardState(colIndex, (int)_playerTurn))
            {
                EventBusManager.Instance.Raise(EventNameEnum.TakeTurn, _playerTurn);
                GameObject newDiskSpawned= null;

                switch (_playerTurn)
                {
                    case PlayerTurnEnum.Player1:
                        newDiskSpawned = GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskRed, _spawnLocations[colIndex].position);
                        break;
                    case PlayerTurnEnum.Player2:
                        newDiskSpawned= GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskYellow, _spawnLocations[colIndex].position);
                        break;
                }
                
                int rowIndex = GameManager.Instance.Get<BoardService>().LastAddedCellRow;
                AddSpawnedDiskInMatrix(newDiskSpawned, rowIndex, colIndex);

                if (CheckForWin(rowIndex, colIndex))
                {
                    HandleWin();
                    return;
                }

                ChangePlayerTurn();
            }
        }

        private void ChangePlayerTurn()
        {
            _playerTurn = (_playerTurn == PlayerTurnEnum.Player1) ? PlayerTurnEnum.Player2 : PlayerTurnEnum.Player1;
            EventBusManager.Instance.Raise(EventNameEnum.ChangePlayerTurn, _playerTurn, 2f);
        }

        private void AddSpawnedDiskInMatrix(GameObject newDiskSpawned, int rowIndex, int colIndex)
        {
            spawnedDisks[rowIndex, colIndex] = newDiskSpawned;
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
            List<Vector2Int> winningDiscs = new List<Vector2Int>();

            bool win = CheckDirection(rowIndex, colIndex, 0, 1, playerValue, winningDiscs) ||
                       CheckDirection(rowIndex, colIndex, 1, 0, playerValue, winningDiscs) ||
                       CheckDirection(rowIndex, colIndex, 1, 1, playerValue, winningDiscs) ||
                       CheckDirection(rowIndex, colIndex, 1, -1, playerValue, winningDiscs);

            if (win)
            {
                winningDiscs.Add(new Vector2Int(rowIndex, colIndex));
                StartCoroutine(HighlightWinningDiscs(winningDiscs));
            }

            return win;
        }

        private bool CheckDirection(int row, int col, int dirRow, int dirCol, int playerValue, List<Vector2Int> winningDiscs)
        {
            int count = 1;
            winningDiscs.Clear();

            count += CountConsecutiveDiscs(row, col, dirRow, dirCol, playerValue, winningDiscs);
            count += CountConsecutiveDiscs(row, col, -dirRow, -dirCol, playerValue, winningDiscs);

            return count >= 4;
        }

        private int CountConsecutiveDiscs(int row, int col, int dirRow, int dirCol, int playerValue, List<Vector2Int> winningDiscs)
        {
            int count = 0;
            int currentRow = row + dirRow;
            int currentColumn = col + dirCol;

            while (currentRow >= 0 && currentRow < _rowCount && currentColumn >= 0 && currentColumn < _colCount)
            {
                if (GameManager.Instance.Get<BoardService>().GetBoardCellValue(currentRow, currentColumn) == playerValue)
                {
                    count++;
                    winningDiscs.Add(new Vector2Int(currentRow, currentColumn));
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

        private IEnumerator HighlightWinningDiscs(List<Vector2Int> winningDiscs)
        {
            yield return new WaitForSeconds(3f);

            foreach (var discPos in winningDiscs)
            {
                Vector3 worldPosition = GetDiskPosition(discPos.x, discPos.y);
                GameObject highlightedDisk = Instantiate(_highlightPrefab, worldPosition, Quaternion.identity);
            }
        }

        private void HandleWin()
        {
            Debug.Log("Player " + _playerTurn + " Won.");
        }

        public void InitializeSpawnedDiskMatrix(int rowCount, int colCount)
        {
            spawnedDisks = new GameObject[rowCount, colCount];
        }

        public Vector3 GetDiskPosition(int row, int col)
        {
            return spawnedDisks[row, col].gameObject.transform.position;
        }
    }
}
