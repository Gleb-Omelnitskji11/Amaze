using UnityEngine;
using Amaze.Configs;

namespace Amaze
{
    public class LevelProgressor : MonoBehaviour
    {
        [SerializeField] private LevelsDataConfig _levelsConfig;
    
        private const string LevelKey = "Level";
        public int LevelIndex {get; private set;}

        private void Awake()
        {
            LevelIndex = PlayerPrefs.GetInt(LevelKey, 0);
        }

        public void CompleteLevel()
        {
            LevelIndex++;
            PlayerPrefs.SetInt(LevelKey, LevelIndex);
            PlayerPrefs.Save();
        }

        public LevelData GetNextLevel() => _levelsConfig.GetLevel(LevelIndex);
    }
}
