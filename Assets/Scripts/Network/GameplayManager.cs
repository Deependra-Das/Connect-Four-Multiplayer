using ConnectFourMultiplayer.Board;
using ConnectFourMultiplayer.Disk;
using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Gameplay;
using ConnectFourMultiplayer.Main;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ConnectFourMultiplayer.Network
{
    public class GameplayManager : NetworkBehaviour
    {
        [SerializeField] private Transform[] _spawnLocations;
        [SerializeField] private GameObject _highlightPrefab;

        private NetworkList<NetworkVector3Int> _gameTurnQueue;
        private GameObject[,] _spawnedDisks;
        private int _rowCount = 0;
        private int _colCount = 0;

        private NetworkVariable<PlayerTurnEnum> _currentTurn = new NetworkVariable<PlayerTurnEnum>(PlayerTurnEnum.None,
                NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private NetworkList<ulong> _readyPlayersNetworkList;

        private NetworkVariable<GameplayStateEnum> _gameplayState = new NetworkVariable<GameplayStateEnum>(GameplayStateEnum.WaitingForPlayers);

        public NetworkList<NetworkVector3Int> _winningDisksNetworkList;

        public static GameplayManager Instance { get; private set; }

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _currentTurn.OnValueChanged += OnTurnChanged;
        }

        private void UnsubscribeToEvents()
        {
            _currentTurn.OnValueChanged -= OnTurnChanged;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _readyPlayersNetworkList = new NetworkList<ulong>();
            _gameTurnQueue = new NetworkList<NetworkVector3Int>();
            _winningDisksNetworkList = new NetworkList<NetworkVector3Int>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                StartCoroutine(WaitForPlayersReady());
            }
        }

        private IEnumerator WaitForPlayersReady()
        {
            _gameplayState.Value = GameplayStateEnum.Initializing;
            InitializeClientRpc();

            while (_readyPlayersNetworkList.Count < NetworkManager.Singleton.ConnectedClients.Count)
            {
                yield return null;
            }

            _gameplayState.Value = GameplayStateEnum.Playing;

            if (IsServer)
                _currentTurn.Value = PlayerTurnEnum.Player1;
        }


        [ClientRpc]
        private void InitializeClientRpc()
        {
            GameManager.Instance.Get<BoardService>().InitializeBoard();
            _rowCount = GameManager.Instance.Get<BoardService>().RowCount;
            _colCount = GameManager.Instance.Get<BoardService>().ColumnCount;
            InitializeSpawnedDiskMatrix(_rowCount, _colCount);
            GameManager.Instance.Get<DiskPreviewService>().Initialize(_spawnLocations[0].position);
            NotifyReadyServerRpc(NetworkManager.Singleton.LocalClientId);
        }


        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void NotifyReadyServerRpc(ulong clientId, RpcParams rpcParams = default)
        {
            if (!_readyPlayersNetworkList.Contains(clientId))
                _readyPlayersNetworkList.Add(clientId);
        }

        private void OnTurnChanged(PlayerTurnEnum oldTurn, PlayerTurnEnum newTurn)
        {
            EventBusManager.Instance.Raise(EventNameEnum.ChangePlayerTurn, _currentTurn.Value, 2f);

            if (IsMyTurn())
            {
                EventBusManager.Instance.RaiseNoParams(EventNameEnum.EnableColumnInput);
            }
        }

        public void InitializeSpawnedDiskMatrix(int rowCount, int colCount)
        {
            _spawnedDisks = new GameObject[rowCount, colCount];
        }

        public bool TryTakeTurn(int colIndex)
        {
            if (_gameplayState.Value == GameplayStateEnum.Playing && IsMyTurn())
            {
                int rowIndex = GetRowForAvailableCell(colIndex);

                if (rowIndex != -1)
                {
                    TakeTurnServerRpc(new NetworkVector3Int(rowIndex, colIndex, (int)_currentTurn.Value));
                    return true;
                }
            }
            return false;
        }

        public void OnHoverOverColumn(int colIndex)
        {
            if (IsMyTurn())
            {
                OnHoverOverColumnServerRpc(colIndex);
            }       
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void OnHoverOverColumnServerRpc(int colIndex, RpcParams rpcParams = default)
        {
            ulong sender = rpcParams.Receive.SenderClientId;

            if (!IsPlayersTurn(sender))
            {
                return;
            }

            UpdateDiskPreviewClientRpc(colIndex);
        }

        [ClientRpc]
        private void UpdateDiskPreviewClientRpc(int colIndex)
        {
            GameManager.Instance.Get<DiskPreviewService>().HandleHoverOverColumnDiskPreview(_currentTurn.Value, _spawnLocations[colIndex].position);
        }

        private int GetRowForAvailableCell(int col)
        {
            return GameManager.Instance.Get<BoardService>().GetRowForAvailableCell(col);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void TakeTurnServerRpc(NetworkVector3Int turnData, RpcParams rpcParams = default)
        {
            ulong sender = rpcParams.Receive.SenderClientId;

            if (!IsPlayersTurn(sender))
            {
                return;
            }

            _gameTurnQueue.Add(turnData);
            UpdateBoardStateClientRpc(turnData);
            SpawnDiskClientRpc(turnData);

            CheckForWin(turnData.x, turnData.y);
            ToggleTurn();
        }

        [ClientRpc]
        private void UpdateBoardStateClientRpc(NetworkVector3Int turnData)
        {
            GameManager.Instance.Get<BoardService>().SetBoardCellValue(turnData.x, turnData.y, turnData.z);
            EventBusManager.Instance.Raise(EventNameEnum.TakeTurn, _currentTurn.Value);
        }

        [ClientRpc]
        private void SpawnDiskClientRpc(NetworkVector3Int turnData)
        {
            GameObject newDiskSpawned = null;

            switch (_currentTurn.Value)
            {
                case PlayerTurnEnum.Player1:
                    newDiskSpawned = GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskRed, _spawnLocations[turnData.y].position);
                    break;
                case PlayerTurnEnum.Player2:
                    newDiskSpawned = GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskYellow, _spawnLocations[turnData.y].position);
                    break;
            }

            _spawnedDisks[turnData.x, turnData.y] = newDiskSpawned;
        }

        private void CheckForWin(int rowIndex, int colIndex)
        {
            int playerValue = (int)_currentTurn.Value;
            List<Vector2Int> winningDiscs = new List<Vector2Int>();

            bool win = CheckDirection(rowIndex, colIndex, 0, 1, playerValue, winningDiscs) ||
                       CheckDirection(rowIndex, colIndex, 1, 0, playerValue, winningDiscs) ||
                       CheckDirection(rowIndex, colIndex, 1, 1, playerValue, winningDiscs) ||
                       CheckDirection(rowIndex, colIndex, 1, -1, playerValue, winningDiscs);

            if (win)
            {
                winningDiscs.Add(new Vector2Int(rowIndex, colIndex));

                UpdateWinningDiskList(winningDiscs);
                _gameplayState.Value = GameplayStateEnum.GameOver;
                NotifyWinningDisksClientRpc();
                NotifyGameOverServerRpc();
            }
        }

        private void UpdateWinningDiskList(List<Vector2Int> winningDiscs)
        {
            _winningDisksNetworkList.Clear();

            foreach (var v in winningDiscs)
            {
                _winningDisksNetworkList.Add(new NetworkVector3Int(v.x, v.y, 0));
            }
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

        [ClientRpc]
        private void NotifyWinningDisksClientRpc()
        {
            StartCoroutine(HighlightWinningDiscs());
        }

        private IEnumerator HighlightWinningDiscs()
        {
            yield return new WaitForSeconds(3f);

            foreach (var discPos in _winningDisksNetworkList)
            {
                Vector3 worldPosition = GetDiskPosition(discPos.x, discPos.y);
                GameObject highlightedDisk = Instantiate(_highlightPrefab, worldPosition, Quaternion.identity);
            }
        }

        public Vector3 GetDiskPosition(int row, int col)
        {
            return _spawnedDisks[row, col].gameObject.transform.position;
        }


        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void NotifyGameOverServerRpc(RpcParams rpcParams = default)
        {
            EventBusManager.Instance.Raise(EventNameEnum.GameOver, _currentTurn.Value);
            LoadGameOverScene();
        }

        private IEnumerator LoadGameOverScene()
        {
            yield return new WaitForSeconds(5f);
            SceneLoader.Instance.LoadScene(SceneNameEnum.GameOverScene, true);
        }

        private bool IsPlayersTurn(ulong clientId)
        {
            ulong host = NetworkManager.ServerClientId;

            if (_currentTurn.Value == PlayerTurnEnum.Player1)
                return clientId == host;

            if (_currentTurn.Value == PlayerTurnEnum.Player2)
                return clientId != host;

            return false;
        }

        private void ToggleTurn()
        {
            _currentTurn.Value = (_currentTurn.Value == PlayerTurnEnum.Player1) ? PlayerTurnEnum.Player2 : PlayerTurnEnum.Player1;
        }

        public bool IsMyTurn()
        {
            ulong me = NetworkManager.Singleton.LocalClientId;
            ulong host = NetworkManager.ServerClientId;

            if (_currentTurn.Value == PlayerTurnEnum.Player1)
                return me == host;

            if (_currentTurn.Value == PlayerTurnEnum.Player2)
                return me != host;

            return false;
        }
    }
}