﻿using TableDungeon.Dungeon;
using UnityEngine;
using Random = System.Random;

namespace TableDungeon.Maze
{
    public class TableScript : MonoBehaviour
    {
        [Header("Board Settings")]
        [Min(1)] public int rows = 1;
        [Min(1)] public int cols = 1;
        [Space]
        public float cellSize = 0.5F;
        public GameObject tilePrefab;
        
        [Header("Generation Settings")]
        public float horizontalChance = 0.25F;
        public float verticalChance = 0.25F;
        
        [Header("Dungeon Settings")]
        public RoomScript dungeon;
        public Transform figurine;
        private Vector2Int _figurinePosition;
        
        private Room[,] _grid;
        private Renderer[,] _tileRenderers;
        private Texture2D _tileTexture;

        private void Awake()
        {
            _tileRenderers = new Renderer[rows, cols];
            _tileTexture = Resources.Load<Texture2D>("Sprites/Tiles");
            
            var container = new GameObject("Tile Container");
            container.transform.parent = transform;
            
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var center = FromGridToWorld(i, j);
                    var obj = Instantiate(tilePrefab, center, Quaternion.identity, container.transform);
                    
                    obj.name = $"Tile [{i}, {j}]";
                    _tileRenderers[i, j] = obj.GetComponentInChildren<Renderer>();
                }
            }
            
            Generate();
        }

        private void Start()
        {
            dungeon.onPlayerMoved += direction => SetFigurinePosition(_figurinePosition + direction.GetIntVector());
        }

        private void SetFigurinePosition(Vector2Int value)
        {
            figurine.position = FromGridToWorld(value.y, value.x);
            _figurinePosition = value;
        }

        public void Generate()
        {
            var generator = new Generator(rows, cols, horizontalChance, verticalChance, new Random());
            _grid = generator.Generate();
            
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    // Calculating sprite index
                    var room = _grid[i, j];
                    var index = 0;
                    Directions.Values.ForEach((direction, k) =>
                    {
                        if (room?.doors[direction] != null) index += 1 << k;
                    });

                    var x = (index % 4) / 4.0F;
                    var y = (index / 4) / 4.0F;

                    // Setting sprite
                    var rdr = _tileRenderers[i, j];
                    rdr.material = new Material(rdr.material)
                    {
                        mainTexture = _tileTexture,
                        mainTextureOffset = new Vector2(x, y)
                    };
                }
            }

            dungeon.SetRoom(_grid[1, 1]);
            SetFigurinePosition(new Vector2Int(1, 1));
        }

        private Vector3 FromGridToWorld(int i, int j)
        {
            var x = 0.5F + j - (float) cols / 2;
            var z = 0.5F + i - (float) rows / 2;
            return new Vector3(x * cellSize, 0, z * -cellSize);
        }

        private void OnDrawGizmos()
        {
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var size = new Vector3(cellSize, 0, cellSize);
                    var center = FromGridToWorld(i, j) + Vector3.up * 0.25F;
                    
                    Gizmos.color = new Color(1, 1, 1, 0.05F);
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    }
}