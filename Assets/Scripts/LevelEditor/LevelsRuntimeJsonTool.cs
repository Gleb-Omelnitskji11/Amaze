using System.IO;
using Amaze.Configs;
using UnityEngine;

namespace Amaze.LevelEditor
{
    public static class LevelsRuntimeJsonTool
    {
        private const string FileName = "LevelsRuntime.json";

        private static string FilePath =>
            Path.Combine(Application.persistentDataPath, FileName);

        public static void ExportToJson(LevelData[] levels)
        {
            if (levels == null)
            {
                Debug.LogError("levels is null");
                return;
            }

            LevelsJsonContainer container = new LevelsJsonContainer
            {
                Levels = levels
            };

            string json = JsonUtility.ToJson(container, true);

            File.WriteAllText(FilePath, json);

            Debug.Log("Levels exported to: " + FilePath);
        }

        public static LevelData[] ImportFromJson()
        {
            string json = File.ReadAllText(FilePath);

            LevelsJsonContainer container =
                JsonUtility.FromJson<LevelsJsonContainer>(json);

            if (container == null || container.Levels == null)
            {
                Debug.LogError("Invalid JSON format");
                return null;
            }

            Debug.Log("Levels imported from JSON");
            return container.Levels;
        }
    }
}