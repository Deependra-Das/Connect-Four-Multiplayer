using UnityEngine;

namespace ConnectFourMultiplayer.Board
{
    public class BoardService
    {
        private int[,] _board;
        private int _boardRowCount = 0;
        private int _boardColumnCount = 0;

        public BoardService(BoardScriptableObject boardScriptableObject)
        {
            _boardRowCount = boardScriptableObject.boardRowCount;
            _boardColumnCount = boardScriptableObject.boardColumnCount;
        }

        ~BoardService()
        {
            _board = null;
        }

        public void InitializeBoard()
        {
            _board = new int[_boardRowCount, _boardColumnCount];
        }

        public void SetBoardCellValue(int row, int col, int value)
        {
            _board[row, col] = value;
        }

        public int GetRowForAvailableCell(int col)
        {
            int availableRow = -1;

            for (int row = _boardRowCount - 1; row >= 0; row--)
            {
                if (_board[row, col] == 0)
                {
                    availableRow = row;
                }
            }
            return availableRow;
        }

        public void PrintBoardLog()
        {
            int rows = _board.GetLength(0);
            int cols = _board.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                string rowValues = "";
                for (int j = 0; j < cols; j++)
                {
                    rowValues += _board[i, j] + " ";
                }
                Debug.Log(rowValues);
            }
        }

        public int GetBoardCellValue(int row, int col)
        {
            return _board[row, col];
        }

        public int RowCount { get { return _boardRowCount; } }

        public int ColumnCount { get { return _boardColumnCount; } }

        public void Reset()
        {
            _board = null;
            _boardRowCount = 0;
            _boardColumnCount = 0;
        }
    }
}
