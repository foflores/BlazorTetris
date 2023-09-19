using Excubo.Blazor.Canvas.Contexts;

namespace Tetris.Models;

public class Grid
{
    private readonly Context2D _context;
    private readonly Context2D _nextPieceContext;
    private readonly double _canvasHeight;
    private readonly double _canvasWidth;
    private readonly double _cellWidth;
    private readonly double _cellHeight;
    private readonly double _widthOffset;
    private readonly double _heightOffset;

    public Grid (
        Context2D context,
        Context2D nextPieceContext,
        double canvasHeight,
        double canvasWidth,
        int rows,
        int columns
    )
    {
        _context = context;
        _nextPieceContext = nextPieceContext;
        _canvasHeight = canvasHeight;
        _canvasWidth = canvasWidth;
        _cellWidth = canvasWidth / columns;
        _cellHeight = canvasHeight / rows;
        _widthOffset = 1;
        _heightOffset = 1;
    }

    public async Task ClearGrid()
    {
        await _context.FillStyleAsync("white");
        await _context.FillRectAsync(0, 0, _canvasWidth, _canvasHeight);
        await _context.FillStyleAsync("black");
        await _context.LineWidthAsync(2);
        for (double i = 0; i <= _canvasHeight; i += _cellWidth)
        {
            for (double j = 0; j <= _canvasWidth; j += _cellWidth)
            {
                await _context.StrokeRectAsync(j, i, _cellWidth, _cellWidth);
            }
        }
    }
    public async Task DrawCell(Cell cell)
    {
        await _context.FillStyleAsync(cell.Color);
        await _context.FillRectAsync(
            cell.Column * _cellWidth + _widthOffset,
            cell.Row * _cellHeight + _heightOffset,
            _cellWidth - 2 * _widthOffset,
            _cellHeight - 2 * _heightOffset
        );
    }
    public async Task DrawCells(IEnumerable<Cell> cells)
    {
        foreach (var cell in cells)
        {
            await DrawCell(cell);
        }
    }
    public async Task DeleteCell(Cell cell)
    {
        await _context.FillStyleAsync("white");
        await _context.FillRectAsync(
            cell.Column * _cellWidth,
            cell.Row * _cellHeight,
            _cellWidth,
            _cellHeight
        );
        await _context.FillStyleAsync("black");
        await _context.StrokeRectAsync(
            cell.Column * _cellWidth,
            cell.Row * _cellHeight,
            _cellWidth,
            _cellHeight
        );
    }
    public async Task DeleteCells(IEnumerable<Cell> cells)
    {
        foreach (var cell in cells)
        {
            await DeleteCell(cell);
        }
    }
    public async Task DrawTetromino(Tetromino tetromino)
    {
        await DrawCells(tetromino.Cells);
    }
    public async Task DeleteTetromino(Tetromino tetromino)
    {
        await DeleteCells(tetromino.Cells);
    }

    public async Task DrawNextPiece(Tetromino tetromino)
    {
        await _nextPieceContext.FillStyleAsync("white");
        await _nextPieceContext.FillRectAsync(0, 0, _canvasHeight / 4, _canvasHeight / 4);
        await _nextPieceContext.FillStyleAsync("black");
        await _nextPieceContext.LineWidthAsync(2);
        for (double i = 0; i <= _canvasHeight / 4; i += _canvasHeight / 20)
        {
            for (double j = 0; j <= _canvasHeight / 4; j += _canvasHeight / 20)
            {
                await _nextPieceContext.StrokeRectAsync(j, i, _canvasHeight / 20, _canvasHeight / 20);
            }
        }
        foreach (var cell in tetromino.Cells)
        {
            await _nextPieceContext.FillStyleAsync(cell.Color);
            await _nextPieceContext.FillRectAsync(
                (cell.Column - 3) * _canvasHeight / 20 + 1,
                (cell.Row + 4) * _canvasHeight / 20 + 1,
                _canvasHeight / 20 - 2,
                _canvasHeight / 20 - 2
            );
        }
    }
}