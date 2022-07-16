using System.Collections.Generic;
using System.Linq;
using TableDungeon.Dungeon;
using UnityEngine;
using Random = System.Random;

// Algorithm was taken from this article. Thanks a lot, Jamis Buck!
// https://weblog.jamisbuck.org/2010/12/29/maze-generation-eller-s-algorithm
namespace TableDungeon.Maze
{
    public class Generator
    {
        private readonly int _rows, _cols;
        private readonly float _hChange, _vChance;
        private readonly Room[,] _grid;
        private readonly int[] _sets;
        private readonly Random _random;

        public Generator(int rows, int cols, float horizontalChance, float verticalChance, Random random)
        {
            Debug.Assert(rows > 0 || cols > 0);

            _rows = rows;
            _cols = cols;
            _hChange = horizontalChance;
            _vChance = verticalChance;
            _grid = new Room[rows, cols];
            _sets = new int[cols];
            _random = random;
        }

        public Room[,] Generate()
        {
            // Populate the first row
            PopulateRow(0);

            for (var i = 0; i < _rows - 1; i++)
            {
                // Randomly connect unconnected rooms
                for (var j = 0; j < _cols - 1; j++)
                {
                    if (_sets[j] == _sets[j + 1]) continue;
                    if (_random.NextDouble() > _hChange) continue;

                    ConnectRooms((i, j), (i, j + 1), Direction.East, _sets[j], _sets[j + 1]);
                    _sets[j + 1] = _sets[j];
                }

                // Randomly connect rooms vertically
                var usedSets = new HashSet<int>();
                var range = Enumerable.Range(0, _cols);
                
                foreach (var j in range.OrderBy(_ => _random.Next()))
                {
                    if (!usedSets.Add(_sets[j]) && _random.NextDouble() > _vChance) continue;

                    _grid[i + 1, j] = new Room();
                    ConnectRooms((i, j), (i + 1, j), Direction.South, _sets[j], -1);
                }

                // Populating next row
                PopulateRow(i + 1);
            }

            // Connect remaining sets in the last row
            var row = _rows - 1;

            for (var j = 0; j < _cols - 1; j++)
            {
                if (_sets[j] == _sets[j + 1]) continue;
                ConnectRooms((row, j), (row, j + 1), Direction.East, _sets[j], _sets[j + 1]);
            }

            return _grid;
        }

        private void ConnectRooms((int x, int y) a, (int x, int y) b, Direction direction, int set1, int set2)
        {
            // Connecting rooms
            var room1 = _grid[a.x, a.y];
            var room2 = _grid[b.x, b.y];

            room1.doors[direction] = room2;
            room2.doors[direction.Opposite()] = room1;

            // Combining sets
            if (set1 >= 0 && set2 >= 0)
            {
                for (var index = 0; index < _sets.Length; index++)
                {
                    if (_sets[index] == set2) _sets[index] = set1;
                }
            }
        }

        private void PopulateRow(int row)
        {
            for (var j = 0; j < _cols; j++)
            {
                if (_grid[row, j] != null) continue;

                var room = new Room();
                var set = Enumerable.Range(0, int.MaxValue).First(x => !_sets.Contains(x));

                _sets[j] = set;
                _grid[row, j] = room;
            }
        }
    }
}