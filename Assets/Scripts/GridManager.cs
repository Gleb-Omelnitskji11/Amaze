using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private CellView _cellPrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private BallController _ballPrefab;
    [SerializeField] private GameSettings _gameSettings;
    
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private float _spacing = 0.1f;

    private CellView[,] _grid;
    private BallController _ball;

    public LevelData CurrentLevel { get; private set; }

    private void Start()
    {
        CellView.Setup(_gameSettings);
        _ball = Instantiate(_ballPrefab);
        _ball.Initialize(_gameSettings, this);
    }

    public int SetLevel(LevelData level)
    {
        CurrentLevel = level;
        int cellsCount = GenerateGrid();
        _ball.SetPosition(CurrentLevel.StartPosition);
        return cellsCount;
    }

    public void PaintStartCell()
    {
        GetCell(CurrentLevel.StartPosition).PaintFilled();
    }

    public int GenerateGrid()
    {
        int cellsCount = 0;
        _grid = new CellView[CurrentLevel.Width, CurrentLevel.Height];

        float totalWidth = CurrentLevel.Width * (_cellSize + _spacing) - _spacing;
        float totalHeight = CurrentLevel.Height * (_cellSize + _spacing) - _spacing;

        Vector3 originOffset = new Vector3(
            -totalWidth / 2f + _cellSize / 2f,
            -totalHeight / 2f + _cellSize / 2f,
            0f);

        for (int x = 0; x < CurrentLevel.Width; x++)
        {
            for (int y = 0; y < CurrentLevel.Height; y++)
            {
                Vector3 position = new Vector3(
                    x * (_cellSize + _spacing),
                    y * (_cellSize + _spacing),
                    0f) + originOffset;

                CellView cell = Instantiate(_cellPrefab, position, Quaternion.identity, _gridParent);

                cell.transform.localScale = Vector3.one * _cellSize;
                var cellType = CurrentLevel.GetCell(x, y);
                cell.Init(new Vector2Int(x, y), cellType);

                _grid[x, y] = cell;
                if(cellType == CellType.Exist)
                {
                    cellsCount++;
                }
            }
        }

        return cellsCount;
    }

    public bool IsEmpty(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 ||
            pos.x >= CurrentLevel.Width || pos.y >= CurrentLevel.Height)
            return true;

        return CurrentLevel.GetCell(pos.x, pos.y) == CellType.Empty;
    }

    public CellView GetCell(Vector2Int pos)
    {
        return _grid[pos.x, pos.y];
    }
}