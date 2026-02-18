using UnityEngine;

public class LevelProgressor : MonoBehaviour
{
    [SerializeField]
    private LevelsDataConfig _levelsConfig;

    public LevelData GetNextLevel() => _levelsConfig.GetLevel();
}
