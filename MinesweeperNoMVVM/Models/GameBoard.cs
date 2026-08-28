using System;

namespace MinesweeperNoMVVM.Models;

public enum Difficulty
{
    Easy = 10,
    Normal = 20, 
    Hard = 30
}

public enum Size
{
    Small = 5,
    Medium = 25,
    Large = 50
}

public class GameBoard
{
    public int GameBoardColumns { get;}
    public int GameBoardRows { get;}
    public int MinesSpawned { get; private set; }
    public bool HasWon { get; private set; }
    public Cell[,] MineField { get; private set; }
    private int RevealedSafeCells { get; set; }
    private int SafeCells { get; set; }
    private Difficulty Difficulty {get;}
    
    private Random _rdom = new Random();
    
    
    public GameBoard(Size size, Difficulty difficulty)
    {
        GameBoardColumns = (int)size;
        GameBoardRows = (int)size;
        Difficulty = difficulty;
        SetUpGameBoard();
    }
    

    private void SetUpGameBoard()
    {
        SetUpField();
        SetUpMineNumber();
    }
    
    private void SetUpField()
    {
        MineField = new Cell[GameBoardRows, GameBoardColumns];
        int maxCells = GameBoardColumns * GameBoardRows;
        
        for (int row = 0; row < GameBoardRows; row++)
        {
            for (int column = 0; column < GameBoardColumns; column++)
            {
                if (_rdom.Next(100) + 1 <= (int)Difficulty)
                {
                    MineField[row,column] = new Cell{IsMine =  true, Row = row, Column = column};
                    MinesSpawned++;
                }
                else
                {
                    MineField[row, column] = new Cell{Row = row, Column = column};
                }
            }
        }

        SafeCells = maxCells - MinesSpawned;
    }

    /*
     * [-1,-1]  [-1, 0]  [-1,+1]
     * [ 0,-1]  [ 0, 0]  [ 0,+1]
     * [+1,-1]  [+1, 0]  [+1,+1]
     */
    private void SetUpMineNumber()
    {
        for (int row = 0; row < GameBoardRows; row++)
        {
            for (int column = 0; column < GameBoardColumns; column++)
            {
                //skips if cell is a mine
                if (MineField[row, column].IsMine)
                {
                    continue;
                }
                
                int nearByMines = 0;
                for (int offsetRow = -1; offsetRow <= 1; offsetRow++)
                {
                    for (int offsetColumn = -1; offsetColumn <= 1; offsetColumn++)
                    {
                        //skips current cell
                        if(offsetRow == 0 && offsetColumn == 0) 
                        {
                            continue;
                        }
                        
                        //Checks to see if the offset Row or column cell would be inside the board area
                        if ((0 <= row + offsetRow && row + offsetRow <= GameBoardRows - 1) && (0 <= column + offsetColumn && column + offsetColumn <= GameBoardColumns - 1))
                        {
                            if (MineField[row + offsetRow, column + offsetColumn].IsMine)
                            {
                                nearByMines++;
                            }
                        }
                    }
                }
                MineField[row, column].NearByMines = nearByMines;
            }
        }
    }
    
    public void SafeCellRevealed()
    {
        RevealedSafeCells++;

        if (RevealedSafeCells == SafeCells)
        {
            HasWon = true;
        }
    }
    
    
}