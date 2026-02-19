using UnityEngine;

public class LevelProgressor : MonoBehaviour
{
    [SerializeField]
    private LevelsDataConfig _levelsConfig;
    
    private const string LevelKey = "Level";
    private int _levelIndex;

    private void Awake()
    {
        _levelIndex = PlayerPrefs.GetInt(LevelKey, 0);
    }

    public void CompleteLevel()
    {
        _levelIndex++;
        PlayerPrefs.SetInt(LevelKey, _levelIndex);
        PlayerPrefs.Save();
    }

    public LevelData GetNextLevel() => _levelsConfig.GetLevel(_levelIndex);
}
