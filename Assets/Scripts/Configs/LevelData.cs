using System;
using UnityEngine;

namespace Amaze.Configs
{
    [Serializable]
    public class LevelData
    {
        public int Width;
        public int Height;
        public CellType[] Cells;
        public Vector2Int StartPosition;

        public CellType GetCell(int x, int y)
        {
            return Cells[y * Width + x];
        }

        //Method for lvl editor
        public void SetCell(int x, int y, CellType type)
        {
            if (Cells == null || Cells.Length != Width * Height)
            {
                Debug.LogError("LevelData.Cells is null or has invalid length");
                return;
            }

            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                Debug.LogError("Coordinates out of bounds");
                return;
            }

            Cells[y * Width + x] = type;
        }

        public LevelData DeepCopy()
        {
            LevelData levelData = new LevelData();
            levelData.Width = Width;
            levelData.Height = Height;
            levelData.Cells = new CellType[Cells.Length];
            for (int i = 0; i < Cells.Length; i++)
            {
                levelData.Cells[i] = Cells[i];
            }
            levelData.StartPosition = StartPosition;
            return levelData;
        }
    }

    public enum CellType
    {
        Exist,
        Empty
    }
}