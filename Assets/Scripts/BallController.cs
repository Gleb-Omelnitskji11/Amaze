using System.Collections.Generic;
using Amaze.Configs;
using DG.Tweening;
using UnityEngine;

namespace Amaze
{
    public class BallController : MonoBehaviour
    {
        private GameSettings _settings;
        private GridManager _grid;
        private InputController _inputController;

        private bool _isMoving;
        private Vector2Int _currentPos;
        private Sequence _sequence;

        public void Initialize(GameSettings settings, GridManager grid, InputController inputController)
        {
            _settings = settings;
            _grid = grid;
            _inputController = inputController;

            Subscribe();
        }

        private void Subscribe()
        {
            if (_inputController != null)
                _inputController.OnSwipeEvent += Move;
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
            if (_inputController != null)
                _inputController.OnSwipeEvent -= Move;
        }

        public void SetPosition(Vector2Int newPos)
        {
            _sequence?.Kill();
            transform.localScale = Vector3.one;
            _currentPos = newPos;
            var cell = _grid.GetCell(newPos);
            transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, 0);
        }

        private void Move(Vector2Int direction)
        {
            if (_isMoving) return;

            Vector2Int checkPos = _currentPos;
            List<CellView> path = GetPath(direction, ref checkPos);

            if (path.Count == 0)
                return;

            _isMoving = true;
            bool isVertical = direction.x == checkPos.x;
            _currentPos = checkPos;

            Vector3 targetPosition = path[^1].Position;

            float duration = Vector3.Distance(transform.position, targetPosition) / _settings.MoveSpeed;
            float scaleDuration = duration * 0.3f > 0.3f ? 0.3f : duration * 0.3f;

            int paintedIndex = 0;

            _sequence = DOTween.Sequence();
            
            Tween moveTween = transform.DOMove(targetPosition, duration)
                .SetEase(Ease.InOutSine);

            _sequence.Append(moveTween);

            int totalCells = path.Count;

            moveTween.OnUpdate(() =>
            {
                if (paintedIndex >= totalCells) return;

                float progress = moveTween.ElapsedPercentage();

                int expectedIndex = Mathf.FloorToInt(progress * totalCells);

                while (paintedIndex <= expectedIndex && paintedIndex < totalCells)
                {
                    PaintCell(path[paintedIndex]);
                    paintedIndex++;
                }
            });
            
            const float scaleDump = 0.15f;
            int dirScale = isVertical ? -1 : 1;
            _sequence.Join(
                transform.DOScale(new Vector3(1f + scaleDump * dirScale, 1f - scaleDump * dirScale, 1f), scaleDuration)
                    .SetEase(Ease.OutQuad)
            );

            _sequence.Append(
                transform.DOScale(Vector3.one, scaleDuration)
                    .SetEase(Ease.OutElastic)
            );

            _sequence.OnComplete(() => { _isMoving = false; });
        }

        private List<CellView> GetPath(Vector2Int direction, ref Vector2Int lastPos)
        {
            List<CellView> path = new List<CellView>();
            while (true)
            {
                Vector2Int nextPos = lastPos + direction;

                if (_grid.IsEmpty(nextPos))
                    break;

                lastPos = nextPos;
                path.Add(_grid.GetCell(lastPos));
            }

            return path;
        }

        private void PaintCell(CellView cell)
        {
            cell.PaintFilled();
        }
    }
}