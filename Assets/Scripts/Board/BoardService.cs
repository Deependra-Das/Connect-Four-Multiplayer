using System;
using UnityEngine;

public class BoardService 
{
    private int[,] _board;
    private int _boardRowCount;
    private int _boardColumnCount;
    private int _lastAddedCellRow = -1;

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

    public bool SetBoardCellValue(int col, int value)
    {
        for(int row= _boardRowCount-1; row >= 0 ; row--)
        {
            if (_board[row, col] == 0)
            {
                _board[row, col] = value;
                _lastAddedCellRow = row;
                return true;
            }
        }
        return false;
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

    public int LastAddedCellRow { get { return _lastAddedCellRow; } }
    public int RowCount { get { return _boardRowCount; } }
    public int ColumnCount { get { return _boardColumnCount; } }
}
