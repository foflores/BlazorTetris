namespace Tetris.Application;

public class TetrominoI : Tetromino
{
    public TetrominoI()
    {
        const string color = "cyan";
        Cells.Add(new Cell(-4, 5, color));
        Cells.Add(new Cell(-3, 5, color));
        Cells.Add(new Cell(-2, 5, color));
        Cells.Add(new Cell(-1, 5, color));
    }

    public override void Rotate()
    {
        switch (Rotation)
        {
            case Rotation.Up:
            case Rotation.Down:
                Cells[0].Row += 2;
                Cells[0].Column += 2;
                Cells[1].Row += 1;
                Cells[1].Column += 1;
                Cells[3].Row -= 1;
                Cells[3].Column -= 1;
                Rotation = Rotation.Right;
                break;
            case Rotation.Right:
            case Rotation.Left:
                Cells[0].Row -= 2;
                Cells[0].Column -= 2;
                Cells[1].Row -= 1;
                Cells[1].Column -= 1;
                Cells[3].Row += 1;
                Cells[3].Column += 1;
                Rotation = Rotation.Up;
                break;
        }
    }
}