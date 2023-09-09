namespace Tetris.Models;

public class TetrominoO : Tetromino
{
    public TetrominoO()
    {
        const string color = "yellow";
        Cells.Add(new Cell(-1, 5, color));
        Cells.Add(new Cell(-2, 5, color));
        Cells.Add(new Cell(-1, 6, color));
        Cells.Add(new Cell(-2, 6, color));
    }

    public override void Rotate() { }
}