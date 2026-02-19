using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Configs/LevelsDataConfig", order = 1)]
public class LevelsDataConfig : ScriptableObject
{
    [SerializeField] private LevelData[] _levels;

    public LevelData GetLevel(int index)
    {
        return _levels[index % _levels.Length];
    }
}