namespace BlazorTetris.Components;

public abstract class Tetromino
{
    public List<Cell> Cells { get; } = [];
    protected Rotation Rotation { get; set; } = Rotation.Up;

    public void MoveLeft()
    {
        Cells.ForEach(x => x.Column--);
    }
    public void MoveRight()
    {
        Cells.ForEach(x => x.Column++);
    }
    public void MoveDown()
    {
        Cells.ForEach(x => x.Row++);
    }
    public void MoveUp()
    {
        Cells.ForEach(x => x.Row--);
    }
    public abstract void Rotate();
}
