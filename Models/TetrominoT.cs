namespace Tetris.Models;

public class TetrominoT : Tetromino
{
    public TetrominoT()
    {
        const string color = "purple";
        Cells.Add(new Cell(-1, 4, color));
        Cells.Add(new Cell(-1, 5, color));
        Cells.Add(new Cell(-1, 6, color));
        Cells.Add(new Cell(-2, 5, color));
    }

    public override void Rotate()
    {
        switch (Rotation)
        {
            case Rotation.Up:
                Cells[0].Column += 1;
                Cells[0].Row -= 1;
                Cells[2].Column -= 1;
                Cells[2].Row += 1;
                Cells[3].Column += 1;
                Cells[3].Row += 1;
                Rotation = Rotation.Right;
                break;
            case Rotation.Right:
                Cells[0].Column += 1;
                Cells[0].Row += 1;
                Cells[2].Column -= 1;
                Cells[2].Row -= 1;
                Cells[3].Column -= 1;
                Cells[3].Row += 1;
                Rotation = Rotation.Down;
                break;
            case Rotation.Down:
                Cells[0].Column -= 1;
                Cells[0].Row += 1;
                Cells[2].Column += 1;
                Cells[2].Row -= 1;
                Cells[3].Column -= 1;
                Cells[3].Row -= 1;
                Rotation = Rotation.Left;
                break;
            case Rotation.Left:
                Cells[0].Column -= 1;
                Cells[0].Row -= 1;
                Cells[2].Column += 1;
                Cells[2].Row += 1;
                Cells[3].Column += 1;
                Cells[3].Row -= 1;
                Rotation = Rotation.Up;
                break;
        }
    }
}