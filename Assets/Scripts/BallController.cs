using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private GameSettings _settings;
    private GridManager _grid;
    private InputController _inputController;

    private bool _isMoving;
    private Vector2Int _currentPos;
    private float _speed;

    public void Initialize(GameSettings settings, GridManager grid)
    {
        _settings = settings;
        _grid = grid;
        _inputController = InputController.Instance;
        _inputController.OnSwipeEvent += Move;
    }

    public void SetPosition(Vector2Int newPos)
    {
        _currentPos = newPos;
        var cell = _grid.GetCell(newPos);
        transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, 0);
    }

    private void Move(Vector2Int direction)
    {
        if (_isMoving) return;
        StartCoroutine(MoveRoutine(direction));
    }

    private IEnumerator MoveRoutine(Vector2Int direction)
    {
        _isMoving = true;

        while (_isMoving)
        {
            Vector2Int nextPos = _currentPos + direction;

            if (_grid.IsEmpty(nextPos))
                break;

            _currentPos = nextPos;
            CellView cell = _grid.GetCell(_currentPos);

            Vector3 target = cell.Position;

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    _settings.MoveSpeed * Time.deltaTime);

                yield return null;
            }

            PaintCell(_grid.GetCell(_currentPos));
        }

        _isMoving = false;
    }

    private void PaintCell(CellView cell)
    {
        cell.PaintFilled();
    }
}
