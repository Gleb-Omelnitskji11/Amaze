using UnityEngine;

namespace Amaze.Configs
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Configs/LevelsDataConfig", order = 1)]
    public class LevelsDataConfig : ScriptableObject
    {
        [SerializeField] private LevelData[] _levels;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public LevelData[] GetAllLevels()
        {
            LevelData[] levels = new LevelData[_levels.Length];
            for (int i = 0; i < _levels.Length; i++)
            {
                levels[i] = _levels[i].DeepCopy();
            }
            return levels;
        }
#endif
        public LevelData GetLevel(int index)
        {
            if (_levels == null || _levels.Length == 0)
                return null;

            return _levels[index % _levels.Length].DeepCopy();
        }
#if UNITY_EDITOR
        public void SetLevel(int index, LevelData level)
        {
            if (_levels == null || _levels.Length <= index)
            {
                System.Array.Resize(ref _levels, index + 1);
            }

            _levels[index] = level;
        }
#endif

        public int LevelsCount => _levels?.Length ?? 0;
    }
}