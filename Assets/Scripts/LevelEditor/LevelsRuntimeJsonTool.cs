using System;
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
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning("Levels JSON not found: " + FilePath);
                return Array.Empty<LevelData>();
            }

            string json;
            try
            {
                json = File.ReadAllText(FilePath);
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to read levels JSON: " + e.Message);
                return Array.Empty<LevelData>();
            }

            LevelsJsonContainer container;
            try
            {
                container = JsonUtility.FromJson<LevelsJsonContainer>(json);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse levels JSON: " + e.Message);
                return Array.Empty<LevelData>();
            }

            if (container == null || container.Levels == null)
            {
                Debug.LogError("Invalid JSON format");
                return Array.Empty<LevelData>();
            }

            Debug.Log("Levels imported from JSON");
            return container.Levels;
        }
    }
}