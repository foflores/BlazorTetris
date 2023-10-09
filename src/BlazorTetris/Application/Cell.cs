namespace BlazorTetris.Application;

public class Cell
{
    public int Row { get; set; }
    public int Column { get; set; }
    public string Color { get; }

    public Cell(int row, int column, string color)
    {
        Row = row;
        Column = column;
        Color = color;
    }
}