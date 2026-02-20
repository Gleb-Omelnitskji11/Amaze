using Amaze.Configs;
using UnityEngine;

namespace Amaze
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private BallController _ballPrefab;
        [SerializeField] private GameSettings _gameSettings;
        [SerializeField] private InputController _input;

        private BallController _ball;

        public BallController Ball => _ball;

        public void Spawn(GridManager grid)
        {
            if (_ball == null)
                _ball = Instantiate(_ballPrefab);

            _ball.Initialize(_gameSettings, grid, _input);
        }
    }
}
