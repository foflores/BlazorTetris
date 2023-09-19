namespace Tetris.Models;

public class State
{
    private readonly Grid _grid;
    private readonly List<Cell> _cells;
    private Tetromino _currentTetromino;
    private Tetromino _nextTetromino;
    private readonly System.Timers.Timer _timer;
    private int _rowsCleared;
    private readonly int _rows;
    private readonly int _columns;

    public int Score => _rowsCleared * 100;
    public int Level => (int)Math.Floor((double)_rowsCleared / 10 + 1);
    public bool IsGameOver => _cells.Any(x => x.Row < 0);
    public bool IsPaused { get; private set; }
    public event EventHandler? StatsUpdatedEvent;

    public State(Grid grid, int rows, int columns)
    {
        _grid = grid;
        _cells = new List<Cell>();
        _currentTetromino = GenerateNewTetromino();
        _nextTetromino = GenerateNewTetromino();
        _timer = new System.Timers.Timer();
        _timer.Elapsed += async (s, e) => await MoveDown();
        _rows = rows;
        _columns = columns;

        _rowsCleared = 0;
        IsPaused = true;

        _grid.ClearGrid();
        _grid.DrawNextPiece(_nextTetromino);
    }

    private static Tetromino GenerateNewTetromino()
    {
        Random random = new();
        var type = (TetrominoType)random.Next(0, 7);
        return type switch
        {
            TetrominoType.I => new TetrominoI(),
            TetrominoType.J => new TetrominoJ(),
            TetrominoType.L => new TetrominoL(),
            TetrominoType.O => new TetrominoO(),
            TetrominoType.S => new TetrominoS(),
            TetrominoType.T => new TetrominoT(),
            TetrominoType.Z => new TetrominoZ(),
            _ => new TetrominoI()
        };
    }
    private async Task ClearRows()
    {
        for (var i = 0; i <= 19; i++)
        {
            if (_cells.Count(x => x.Row == i) < 10)
                continue;

            var row = _cells.Where(x => x.Row == i).ToList();
            await _grid.DeleteCells(row);

            _cells.RemoveAll(x => x.Row == i);
            var cellsToMove = _cells.Where(x => x.Row < i).ToList();
            await _grid.DeleteCells(cellsToMove);
            cellsToMove.ForEach(x => x.Row++);
            await _grid.DrawCells(cellsToMove);

            _rowsCleared++;
            StatsUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
    private bool CanMoveDown()
    {
        var canMoveDown = true;
        _currentTetromino.MoveDown();

        if (_currentTetromino.Cells.Any(x => x.Row >= _rows))
            canMoveDown = false;

        foreach (var cell in _currentTetromino.Cells)
        {
            if (_cells.Any(y => y.Row == cell.Row && y.Column == cell.Column))
                canMoveDown = false;
        }

        _currentTetromino.MoveUp();
        return canMoveDown;
    }
    private bool CanMoveLeft()
    {
        var canMoveLeft = true;
        _currentTetromino.MoveLeft();

        if (_currentTetromino.Cells.Any(x => x.Column < 0))
            canMoveLeft = false;

        foreach (var cell in _currentTetromino.Cells)
        {
            if (_cells.Any(y => y.Row == cell.Row && y.Column == cell.Column))
                canMoveLeft = false;
        }

        _currentTetromino.MoveRight();
        return canMoveLeft;
    }
    private bool CanMoveRight()
    {
        var canMoveRight = true;
        _currentTetromino.MoveRight();

        if (_currentTetromino.Cells.Any(x => x.Column >= _columns))
            canMoveRight = false;

        foreach (var cell in _currentTetromino.Cells)
        {
            if (_cells.Any(y => y.Row == cell.Row && y.Column == cell.Column))
                canMoveRight = false;
        }

        _currentTetromino.MoveLeft();
        return canMoveRight;
    }

    public async Task MoveLeft()
    {
        if (IsPaused || IsGameOver || !CanMoveLeft())
            return;

        await _grid.DeleteTetromino(_currentTetromino);
        _currentTetromino.MoveLeft();
        await _grid.DrawTetromino(_currentTetromino);
    }
    public async Task MoveRight()
    {
        if (IsPaused || IsGameOver || !CanMoveRight())
            return;

        await _grid.DeleteTetromino(_currentTetromino);
        _currentTetromino.MoveRight();
        await _grid.DrawTetromino(_currentTetromino);
    }
    public async Task MoveDown()
    {
        if (IsPaused)
            return;

        if (IsGameOver)
        {
            IsPaused = true;
            StatsUpdatedEvent?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (!CanMoveDown())
        {
            _currentTetromino.Cells.ForEach(_cells.Add);
            await ClearRows();
            _timer.Interval = (double)1000 / Level;
            _currentTetromino = _nextTetromino;
            _nextTetromino = GenerateNewTetromino();
            await _grid.DrawNextPiece(_nextTetromino);
            return;
        }

        await _grid.DeleteTetromino(_currentTetromino);
        _currentTetromino.MoveDown();
        await _grid.DrawTetromino(_currentTetromino);
    }
    public async Task JumpDown()
    {
        if (IsPaused || IsGameOver)
            return;

        while (CanMoveDown())
            await MoveDown();
    }
    public async Task Rotate()
    {
        if (IsPaused || IsGameOver)
            return;

        await _grid.DeleteTetromino(_currentTetromino);
        _currentTetromino.Rotate();

        while (_currentTetromino.Cells.Any(x => x.Column < 0))
            _currentTetromino.MoveRight();
        while (_currentTetromino.Cells.Any(x => x.Column >= _columns))
            _currentTetromino.MoveLeft();
        while (_currentTetromino.Cells.Any(x => _cells.Any(y => y.Row == x.Row && y.Column == x.Column)))
            _currentTetromino.MoveUp();

        await _grid.DrawTetromino(_currentTetromino);
    }
    public async Task StartGame()
    {
        if (IsGameOver)
        {
            await Restart();
            return;
        }

        IsPaused = false;
        _timer.Interval = (double)1000 / Level;
        _timer.Start();
    }
    public void PauseGame()
    {
        IsPaused = true;
        _timer.Stop();
    }
    public async Task Restart()
    {
        PauseGame();
        await _grid.ClearGrid();
        _cells.Clear();
        _currentTetromino = GenerateNewTetromino();
        _rowsCleared = 0;
        _timer.Interval = (double)1000 / Level;
        await StartGame();
    }
}