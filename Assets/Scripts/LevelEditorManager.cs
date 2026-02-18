using UnityEngine;

public class LevelEditorManager : MonoBehaviour
{
    public LevelData editingLevel;
    public GridManager grid;

    private Vector2Int? startPos;

    public void ToggleWall(Vector2Int pos)
    {
        if (editingLevel.GetCell(pos.x, pos.y) == CellType.Empty)
            editingLevel.SetCell(pos.x, pos.y, CellType.Empty);
        else
            editingLevel.SetCell(pos.x, pos.y, CellType.Exist);

        grid.GenerateGrid();
    }

    public void SetStartPosition(Vector2Int pos)
    {
        startPos = pos;
        editingLevel.StartPosition = pos;
    }

    public void SaveToJSON()
    {
        string json = JsonUtility.ToJson(editingLevel);
        System.IO.File.WriteAllText(Application.dataPath + "/level.json", json);
    }
}
