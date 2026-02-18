using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelProgressor _levelProgressor;
    [SerializeField] private GridManager _grid;
    [SerializeField] private UIManager _ui;

    private static GameManager LocalInstance;
    private int _totalCells;
    private int _paintedCells;
    public static GameManager Instance => LocalInstance;

    private void Awake()
    {
        LocalInstance = this;
    }

    private void Start()
    {
        StartNewLevel();
    }

    public void StartNewLevel()
    {
        _paintedCells = 0;
        var level = _levelProgressor.GetNextLevel();
        _totalCells = _grid.SetLevel(level);
        _grid.PaintStartCell();
    }

    public void AddPaintedCell()
    {
        _paintedCells++;
        float percent = (float)_paintedCells / _totalCells * 100f;
        _ui.UpdateProgress(percent);

        if (percent >= 100f)
            _ui.ShowWin();
    }
}