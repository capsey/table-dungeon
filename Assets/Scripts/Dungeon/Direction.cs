using System;
using System.ComponentModel;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public enum Direction
    {
        North, South, West, East
    }

    public static class Directions
    {
        public static readonly Direction[] Values = Utilities.GetEnumValues<Direction>();

        public static Vector2 GetVector(this Direction direction)
        {
            return direction switch {
                Direction.North => Vector2.up,
                Direction.South => Vector2.down,
                Direction.West => Vector2.left,
                Direction.East => Vector2.right,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
        
        public static Vector2Int GetIntVector(this Direction direction)
        {
            return direction switch {
                Direction.North => Vector2Int.down,
                Direction.South => Vector2Int.up,
                Direction.West => Vector2Int.left,
                Direction.East => Vector2Int.right,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static Direction Opposite(this Direction direction)
        {
            return direction switch {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                Direction.East => Direction.West,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
    
    [Serializable]
    public class DirectionMap<T>
    {
        [SerializeField] private T north;
        [SerializeField] private T south;
        [SerializeField] private T west;
        [SerializeField] private T east;

        public T this[Direction key]
        {
            get
            {
                return key switch
                {
                    Direction.North => north,
                    Direction.South => south,
                    Direction.West => west,
                    Direction.East => east,
                    _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
                };
            }
            set
            {
                switch (key)
                {
                    case Direction.North:
                        north = value;
                        break;
                    case Direction.South:
                        south = value;
                        break;
                    case Direction.West:
                        west = value;
                        break;
                    case Direction.East:
                        east = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, null);
                }
            }
        }
    }
}