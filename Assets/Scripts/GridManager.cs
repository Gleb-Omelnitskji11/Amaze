using UnityEngine;
using Amaze.Configs;
using System;

namespace Amaze
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private CellSpawner _cellSpawner;
        [SerializeField] private GameSettings _gameSettings;
    
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _spacing = 0.1f;

        private CellView[,] _grid = new CellView[0,0];

        public event Action OnCellPainted;

        public LevelData CurrentLevel { get; private set; }

        private void Start()
        {
            CellView.Setup(_gameSettings);
        }

        public int SetLevel(LevelData level, BallController ball)
        {
            CurrentLevel = level;
            int cellsCount = GenerateGrid();
            UpdateBallPosition(ball);
            return cellsCount;
        }

        public void UpdateBallPosition(BallController ball)
        {
            ball.SetPosition(CurrentLevel.StartPosition);
        }

        public void PaintStartCell()
        {
            if (IsEmpty(CurrentLevel.StartPosition))
            {
                Debug.LogWarning("Wrong Start Position");
                CurrentLevel.StartPosition = FindFirstExistCell();
            }
            GetCell(CurrentLevel.StartPosition).PaintFilled();
        }

        private Vector2Int FindFirstExistCell()
        {
            for (int i = 0; i < CurrentLevel.Cells.Length; i++)
            {
                if (CurrentLevel.Cells[i] == CellType.Exist)
                {
                    int x = i % CurrentLevel.Width;
                    int y = i / CurrentLevel.Width;
                    return new Vector2Int(x, y);
                }
            }

            Debug.LogWarning("No free cell found in level!");
            return Vector2Int.zero;
        }

        public void DestroyGrid()
        {
            if (_cellSpawner != null)
                _cellSpawner.DeactivateAll();

            _grid = new CellView[0, 0];
        }

        private int GenerateGrid()
        {
            int cellsCount = 0;
            _grid = new CellView[CurrentLevel.Width, CurrentLevel.Height];

            float totalWidth = CurrentLevel.Width * (_cellSize + _spacing) - _spacing;
            float totalHeight = CurrentLevel.Height * (_cellSize + _spacing) - _spacing;

            Vector3 originOffset = new Vector3(
                -totalWidth / 2f + _cellSize / 2f,
                -totalHeight / 2f + _cellSize / 2f,
                0f);

            if (_cellSpawner == null)
            {
                Debug.LogError("CellSpawner is not set in GridManager");
                return 0;
            }

            int requiredCells = CurrentLevel.Width * CurrentLevel.Height;
            _cellSpawner.EnsureCapacity(requiredCells);
            _cellSpawner.DeactivateRange(requiredCells);

            for (int x = 0; x < CurrentLevel.Width; x++)
            {
                for (int y = 0; y < CurrentLevel.Height; y++)
                {
                    int index = y * CurrentLevel.Width + x;
                    Vector3 position = new Vector3(
                        x * (_cellSize + _spacing),
                        y * (_cellSize + _spacing),
                        0f) + originOffset;

                    CellView cell = _cellSpawner.Get(index);
                    cell.transform.position = position;

                    cell.transform.localScale = Vector3.one * _cellSize;
                    var cellType = CurrentLevel.GetCell(x, y);
                    cell.Init(new Vector2Int(x, y), cellType);

                    cell.OnPainted -= HandleCellPainted;
                    cell.OnPainted += HandleCellPainted;

                    _grid[x, y] = cell;
                    if(cellType == CellType.Exist)
                    {
                        cellsCount++;
                    }
                }
            }

            return cellsCount;
        }

        private void HandleCellPainted(CellView cell)
        {
            OnCellPainted?.Invoke();
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
}