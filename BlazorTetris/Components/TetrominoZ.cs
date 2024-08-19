namespace BlazorTetris.Components;

public class TetrominoZ : Tetromino
{
    public TetrominoZ()
    {
        const string color = "red";
        Cells.Add(new Cell(-2, 4, color));
        Cells.Add(new Cell(-2, 5, color));
        Cells.Add(new Cell(-1, 5, color));
        Cells.Add(new Cell(-1, 6, color));
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
                Cells[3].Row -= 1;
                Cells[3].Column -= 2;
                Rotation = Rotation.Right;
                break;
            case Rotation.Right:
            case Rotation.Left:
                Cells[0].Column -= 1;
                Cells[1].Row += 1;
                Cells[2].Column += 1;
                Cells[3].Row += 1;
                Cells[3].Column += 2;
                Rotation = Rotation.Up;
                break;
        }
    }
}