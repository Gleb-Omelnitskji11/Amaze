using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Configs/LevelsDataConfig", order = 1)]
public class LevelsDataConfig : ScriptableObject
{
    [SerializeField] private LevelData[] _levels;

    public LevelData GetLevel(int index)
    {
        if (_levels == null || _levels.Length == 0)
            return null;

        return _levels[index % _levels.Length];
    }
    
    public void SetLevel(int index, LevelData level)
    {
        if (_levels == null || _levels.Length <= index)
        {
            System.Array.Resize(ref _levels, index + 1);
        }

        _levels[index] = level;
    }

    public int LevelsCount => _levels?.Length ?? 0;
}