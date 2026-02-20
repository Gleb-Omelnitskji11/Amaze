using System;
using System.Collections.Generic;
using Amaze.Configs;
using UnityEngine;

namespace Amaze.LevelEditor
{
    public static class LevelValidator
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        public static bool Validate(LevelData level, out string error)
        {
            if (!ValidateMinimal(level, out error))
                return false;

            if (!ValidateSolvable(level, out error))
                return false;

            error = null;
            return true;
        }

        public static bool ValidateMinimal(LevelData level, out string error)
        {
            if (level == null)
            {
                error = "Level is null";
                return false;
            }

            if (level.Width <= 0 || level.Height <= 0)
            {
                error = "Width/Height must be > 0";
                return false;
            }

            int expected = level.Width * level.Height;
            if (level.Cells == null)
            {
                error = "Cells is null";
                return false;
            }

            if (level.Cells.Length != expected)
            {
                error = $"Cells length mismatch. Expected {expected}, got {level.Cells.Length}";
                return false;
            }

            if (!IsInside(level, level.StartPosition))
            {
                error = "StartPosition is out of bounds";
                return false;
            }

            if (GetCellSafe(level, level.StartPosition) != CellType.Exist)
            {
                error = "StartPosition must be on an Exist cell";
                return false;
            }

            bool hasAnyExist = false;
            for (int i = 0; i < level.Cells.Length; i++)
            {
                if (level.Cells[i] == CellType.Exist)
                {
                    hasAnyExist = true;
                    break;
                }
            }

            if (!hasAnyExist)
            {
                error = "Level has no Exist cells";
                return false;
            }

            error = null;
            return true;
        }

        public static bool ValidateSolvable(LevelData level, out string error)
        {
            HashSet<Vector2Int> reachableStops = new HashSet<Vector2Int>();
            HashSet<Vector2Int> paintableCells = new HashSet<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            reachableStops.Add(level.StartPosition);
            paintableCells.Add(level.StartPosition);
            queue.Enqueue(level.StartPosition);

            while (queue.Count > 0)
            {
                Vector2Int pos = queue.Dequeue();

                for (int i = 0; i < Directions.Length; i++)
                {
                    Vector2Int endPos;
                    if (!TrySlide(level, pos, Directions[i], out endPos, paintableCells))
                        continue;

                    if (reachableStops.Add(endPos))
                        queue.Enqueue(endPos);
                }
            }

            for (int y = 0; y < level.Height; y++)
            {
                for (int x = 0; x < level.Width; x++)
                {
                    Vector2Int cellPos = new Vector2Int(x, y);
                    if (level.GetCell(x, y) != CellType.Exist)
                        continue;

                    if (!paintableCells.Contains(cellPos))
                    {
                        error = "Level is not solvable: not all Exist cells are reachable/paintable from StartPosition";
                        return false;
                    }
                }
            }

            error = null;
            return true;
        }

        private static bool TrySlide(LevelData level, Vector2Int start, Vector2Int direction, out Vector2Int end, HashSet<Vector2Int> painted)
        {
            end = start;
            bool moved = false;

            while (true)
            {
                Vector2Int next = end + direction;
                if (!IsInside(level, next))
                    break;

                if (GetCellSafe(level, next) == CellType.Empty)
                    break;

                end = next;
                painted.Add(end);
                moved = true;
            }

            return moved;
        }

        private static bool IsInside(LevelData level, Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < level.Width && pos.y < level.Height;
        }

        private static CellType GetCellSafe(LevelData level, Vector2Int pos)
        {
            int index = pos.y * level.Width + pos.x;
            if (index < 0 || level.Cells == null || index >= level.Cells.Length)
                return CellType.Empty;

            return level.Cells[index];
        }
    }
}
