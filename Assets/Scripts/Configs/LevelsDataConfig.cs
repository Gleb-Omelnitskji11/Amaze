using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Configs/LevelsDataConfig", order = 1)]

public class LevelsDataConfig : ScriptableObject
{
    [SerializeField] private LevelData _level;

    public LevelData GetLevel() => _level.DeepCopy();
}