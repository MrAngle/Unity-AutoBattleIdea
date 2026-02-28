using System;
using UnityEngine;

namespace MageFactory.Shared.Model {
    public enum GridDirection {
        None = 0,

        Up,
        Right,
        Down,
        Left,

        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }

    public static class GridDirectionSets {
        public static readonly GridDirection[] Orthogonal = {
            GridDirection.Up,
            GridDirection.Right,
            GridDirection.Down,
            GridDirection.Left
        };

        public static readonly GridDirection[] AllExcludeNone = {
            GridDirection.Up,
            GridDirection.Right,
            GridDirection.Down,
            GridDirection.Left,
            GridDirection.UpRight,
            GridDirection.UpLeft,
            GridDirection.DownRight,
            GridDirection.DownLeft
        };

        public static readonly GridDirection[] All = {
            GridDirection.None,
            GridDirection.Up,
            GridDirection.Right,
            GridDirection.Down,
            GridDirection.Left,
            GridDirection.UpRight,
            GridDirection.UpLeft,
            GridDirection.DownRight,
            GridDirection.DownLeft
        };
    }

    public static class GridDirectionExtensions {
        public static Vector2Int toVector2Int(this GridDirection dir) {
            return dir switch {
                GridDirection.None => Vector2Int.zero,
                GridDirection.Up => Vector2Int.up,
                GridDirection.Right => Vector2Int.right,
                GridDirection.Down => Vector2Int.down,
                GridDirection.Left => Vector2Int.left,
                GridDirection.UpRight => new Vector2Int(1, 1),
                GridDirection.UpLeft => new Vector2Int(-1, 1),
                GridDirection.DownRight => new Vector2Int(1, -1),
                GridDirection.DownLeft => new Vector2Int(-1, -1),
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };
        }
    }
}