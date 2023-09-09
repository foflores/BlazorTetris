namespace Tetris.Models;

public class TetrominoS : Tetromino
{
    public TetrominoS()
    {
        const string color = "green";
        Cells.Add(new Cell(-1, 4, color));
        Cells.Add(new Cell(-1, 5, color));
        Cells.Add(new Cell(-2, 5, color));
        Cells.Add(new Cell(-2, 6, color));
    }

    public override void Rotate()
    {
        switch (Rotation)
        {
            case Rotation.Up:
            case Rotation.Down:
                Cells[0].Column += 1;
                Cells[1].Row -= 1;
                Cells[2].Column -= 1;
                Cells[3].Column -= 2;
                Cells[3].Row -= 1;
                Rotation = Rotation.Right;
                break;
            case Rotation.Right:
            case Rotation.Left:
                Cells[0].Column -= 1;
                Cells[1].Row += 1;
                Cells[2].Column += 1;
                Cells[3].Column += 2;
                Cells[3].Row += 1;
                Rotation = Rotation.Up;
                break;
        }
    }
}