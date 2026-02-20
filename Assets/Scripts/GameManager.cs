using UnityEngine;

namespace Amaze
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelProgressor _levelProgressor;
        [SerializeField] private GridManager _grid;
        [SerializeField] private UIManager _ui;
        [SerializeField] private BallSpawner _ballSpawner;

        private int _totalCells;
        private int _paintedCells;

        private void Awake()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            _grid.OnCellPainted += AddPaintedCell;
            _ui.OnRestartClicked += StartNewLevel;
        }

        private void OnDestroy()
        {
            _grid.OnCellPainted -= AddPaintedCell;
        }

        private void Start()
        {
            _ballSpawner.Spawn(_grid);
            StartLevel();
        }

        public void StartNewLevel()
        {
            _grid.DestroyGrid();
            StartLevel();
        }

        private void StartLevel()
        {
            _paintedCells = 0;
            var level = _levelProgressor.GetNextLevel();
            _totalCells = _grid.SetLevel(level, _ballSpawner.Ball);
            _grid.PaintStartCell();
            _ui.SetLevel(_levelProgressor.LevelIndex + 1);
        }

        private void AddPaintedCell()
        {
            _paintedCells++;
            float percent = (float)_paintedCells / _totalCells * 100f;
            _ui.UpdateProgress(percent);

            if (percent >= 100f)
            {
                _levelProgressor.CompleteLevel();
                _ui.ShowWin();
            }
        }
    }
}