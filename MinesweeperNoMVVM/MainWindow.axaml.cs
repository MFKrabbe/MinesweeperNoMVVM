using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MinesweeperNoMVVM.Models;

namespace MinesweeperNoMVVM;


public partial class MainWindow : Window
{
    private GameBoard _gameBoard;
    private Button[,] _buttons;
    private Size _size;
    private Difficulty _difficulty;
    private Stopwatch _stopwatch =  new Stopwatch();
    private int _fontSize;

    public MainWindow()
    {
        InitializeComponent();
    }

    /*
     * Should take input from user and make a game board with selected rows, columns and Difficulty
     */
    private void BtnStartGame_OnClick(object? sender, RoutedEventArgs e)
    {
        GetGameParameters();
        SetUpGame();
    }
    
    private void CellButton_onClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (button.Tag is Cell cell)
            {
                if (cell.IsMine)
                {
                    HandleLose(button, cell);
                }
                else
                {
                    RevealCell(cell.Row, cell.Column);

                    if (_gameBoard.HasWon)
                    {
                        HandleWin();
                    }
                }
            }
        }
    }
    
    private void GetGameParameters()
    {
        GetGameSize();
        GetGameDifficulty();
    }
    
    
    private void GetGameSize()
    {
        if (CbGameBoardSize.SelectionBoxItem != null)
        {
            switch (CbGameBoardSize.Text)
            {
                case "Small":
                    _size = Size.Small;
                    _fontSize = 50;
                    break;

                case "Medium":
                    _size = Size.Medium;
                    _fontSize = 25;
                    break;

                case "Large":
                    _size = Size.Large;
                    _fontSize = 5;
                    break;
            }
        }
        else
        {
            TbSizeWarning.IsVisible = true;
            return;
        }
        TbSizeWarning.IsVisible = false;
    }
    
    private void GetGameDifficulty()
    {
        if (CbDifficultySelector.SelectionBoxItem != null)
        {
            switch (CbDifficultySelector.Text)
            {
                case "Easy":
                    _difficulty = Difficulty.Easy;
                    break;

                case "Medium":
                    _difficulty = Difficulty.Normal;
                    break;

                case "Hard":
                    _difficulty = Difficulty.Hard;
                    break;
            }
        }
        else
        {
            TbDifficultySelectorWarning.IsVisible = true;
            return;
        }
        TbDifficultySelectorWarning.IsVisible = false;   
    }

    private void SetUpGame()
    {
        _stopwatch.Restart();
        
        UgGameBoardHolder.Children.Clear();
        UgGameBoardHolder.IsEnabled = true;
        
        TbWinLoseText.Text = "";
        TbWinLoseText.IsVisible = false;
        
        _gameBoard = new GameBoard(_size, _difficulty);
        _buttons = new Button[_gameBoard.GameBoardRows, _gameBoard.GameBoardColumns];
        
        UgGameBoardHolder.Rows = _gameBoard.GameBoardRows;
        UgGameBoardHolder.Columns = _gameBoard.GameBoardColumns;
        
        for (int row = 0; row < _gameBoard.GameBoardRows; row++)
        {
            for (int column = 0; column < _gameBoard.GameBoardColumns; column++)
            {
                Button cellButton = new Button();
                cellButton.Classes.Add("mine-cell");
                cellButton.Click += CellButton_onClick;
                
                UgGameBoardHolder.Children.Add(cellButton);
                cellButton.Tag = _gameBoard.MineField[row, column];
                _buttons[row, column] = cellButton;
            }
        }
    }


    
    
    private void RevealCell(int row, int column)
    {
        if ((0 > row || row >= _gameBoard.GameBoardRows ) || (0 > column || column >= _gameBoard.GameBoardColumns ))
        {
            return;
        }
        
        Cell cell = _gameBoard.MineField[row, column];
        Button button = _buttons[row, column];

        if (cell.IsRevealed || cell.IsMine)
        {
            return;
        }
        
        cell.IsRevealed = true;
        _gameBoard.SafeCellRevealed();
        
        button.IsEnabled = false;
        
        if (cell.NearByMines > 0)
        {
            button.Content = cell.NearByMines;
        }
        
        if (cell.NearByMines != 0)
        {
            return;
        }

        for (int offsetRow = -1; offsetRow <= 1; offsetRow++)
        {
            for (int offsetColumn = -1; offsetColumn <= 1; offsetColumn++)
            {
                //skips current cell
                if(offsetRow == 0 && offsetColumn == 0) 
                {
                    continue;
                }
                RevealCell(row + offsetRow, column + offsetColumn);
            }
        }
    }

    private void HandleWin()
    {
        _stopwatch.Stop();
        
        UgGameBoardHolder.IsEnabled = false;
        TbWinLoseText.Text = $"You win, Your time was {_stopwatch.Elapsed.TotalSeconds:F2} seconds";
        TbWinLoseText.Foreground = Brushes.Green;
        TbWinLoseText.IsVisible = true;
    }
    
    private void HandleLose(Button button, Cell cell)
    {
        _stopwatch.Stop();
        
        button.Content = "💣";
        button.IsEnabled = false;
        cell.IsRevealed = true;
        
        UgGameBoardHolder.IsEnabled = false;

        TbWinLoseText.Text = $"You Lose, there were {_gameBoard.MinesSpawned} bombs left";
        TbWinLoseText.IsVisible = true;
        TbWinLoseText.Foreground = Brushes.Red;
    }
    
}