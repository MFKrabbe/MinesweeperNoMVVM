namespace MinesweeperNoMVVM.Models;

public class Cell
{
    public bool IsMine { get; set;}
    public bool IsFlagged { get; set;}
    public int NearByMines { get; set;}
    
    public bool IsRevealed{ get; set;} 
    
    public int Row { get; set; }
    public int Column { get; set; }
    
}