using System.IO;
using UnityEditor;
using UnityEngine;

public static class LevelsConfigJsonTool
{
    private const string DefaultFileName = "LevelsExport.json";

    [MenuItem("Tools/Levels/Export Config To JSON")]
    public static void ExportToJson()
    {
        LevelsDataConfig config = Selection.activeObject as LevelsDataConfig;

        if (config == null)
        {
            Debug.LogError("Select LevelsDataConfig asset first!");
            return;
        }

        LevelsJsonContainer container = new LevelsJsonContainer
        {
            Levels = GetLevels(config)
        };

        string json = JsonUtility.ToJson(container, true);

        string path = EditorUtility.SaveFilePanel(
            "Save Levels JSON",
            Application.dataPath,
            DefaultFileName,
            "json");

        if (string.IsNullOrEmpty(path))
            return;

        File.WriteAllText(path, json);

        Debug.Log("Levels exported to JSON:\n" + path);
    }

    [MenuItem("Tools/Levels/Import JSON To Config")]
    public static void ImportFromJson()
    {
        LevelsDataConfig config = Selection.activeObject as LevelsDataConfig;

        if (config == null)
        {
            Debug.LogError("Select LevelsDataConfig asset first!");
            return;
        }

        string path = EditorUtility.OpenFilePanel(
            "Select Levels JSON",
            Application.dataPath,
            "json");

        if (string.IsNullOrEmpty(path))
            return;

        string json = File.ReadAllText(path);

        LevelsJsonContainer container =
            JsonUtility.FromJson<LevelsJsonContainer>(json);

        if (container == null || container.Levels == null)
        {
            Debug.LogError("Invalid JSON format");
            return;
        }

        SetLevels(config, container.Levels);

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

        Debug.Log("Levels imported into Config");
    }

    // --- Helpers ---

    private static LevelData[] GetLevels(LevelsDataConfig config)
    {
        var field = typeof(LevelsDataConfig)
            .GetField("_levels",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        return (LevelData[])field.GetValue(config);
    }

    private static void SetLevels(LevelsDataConfig config, LevelData[] levels)
    {
        var field = typeof(LevelsDataConfig)
            .GetField("_levels",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        field.SetValue(config, levels);
    }
}