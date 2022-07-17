using System;
using System.Collections.Generic;
using System.Linq;
using TableDungeon.Dungeon;
using UnityEngine;
using Random = System.Random;

namespace TableDungeon.Maze
{
    public class TableScript : MonoBehaviour
    {
        [Header("Board Settings")]
        [Min(1)] public int rows = 1;
        [Min(1)] public int cols = 1;
        public float cellSize = 0.5F;
        [Space]
        public GameObject tilePrefab;
        public GameObject iconPrefab;
        public Texture2D unvisitedTileTexture;
        public Texture2D visitedTileTexture;
        
        [Header("Generation Settings")]
        [Range(0, 1)] public float horizontalChance = 0.5F;
        [Range(0, 1)] public float verticalChance = 0.5F;
        [Range(0, 1)] public float lootChance = 0.1F;

        [Header("UI Settings")]
        public Transform zonePreview;
        
        [Header("Dungeon Settings")]
        public RoomScript dungeon;
        public Transform figurine;
        private Vector2Int _playerPosition;
        private Vector2Int _basePosition;
            
        private Room[,] _grid;
        private Material[,] _tileMaterials;
        private Dictionary<(int, int), Renderer> _iconRenderers;

        private int _zonesLeft;
        private Vector2Int _zoneSize = new Vector2Int(2, 4);

        private Dictionary<Item.Type, int> _inventory;

        private const float VariantChance = 0.2F;

        private void Awake()
        {
            _tileMaterials = new Material[rows, cols];
            _iconRenderers = new Dictionary<(int, int), Renderer>();
            zonePreview.gameObject.SetActive(false);

            _inventory = new Dictionary<Item.Type, int>();
            Utilities.GetEnumValues<Item.Type>().ForEach(type => _inventory.Add(type, 0));

            var container = new GameObject("Tile Container");
            container.transform.parent = transform;
            
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var center = FromGridToWorld(i, j);
                    var obj = Instantiate(tilePrefab, center, Quaternion.identity, container.transform);
                    var rdr = obj.GetComponentInChildren<Renderer>();
                    var material = new Material(rdr.material);

                    obj.name = $"Tile [{i}, {j}]";
                    rdr.material = material;
                    _tileMaterials[i, j] = material;
                }
            }
            
            Generate();
        }

        private void Start()
        {
            dungeon.OnPlayerMoved += direction => SetFigurinePosition(_playerPosition + direction.GetIntVector());
            dungeon.OnPlayerCollected += item => _inventory[item.type] += 1;

            var manager = FindObjectOfType<GameManager>();
            manager.Controls.Table.Mouse.performed += ctx =>
                OnMouseMoved(ctx.ReadValue<Vector2>(), manager.TableCamera);
            manager.Controls.Table.RotateZone.performed += _ =>
                _zoneSize = new Vector2Int(_zoneSize.y, _zoneSize.x);
            manager.Controls.Table.Accept.performed += _ =>
                ApplyZone(manager.Controls.Table.Mouse.ReadValue<Vector2>(), manager.TableCamera);
            manager.Controls.Table.RollDice.performed += _ => RollDice();
        }

        private void RollDice()
        {
            var random = new Random();
            _zonesLeft += 1;
            _zoneSize = new Vector2Int(random.Next(1, 7), random.Next(1, 7));
        }

        private void ApplyZone(Vector2 mouse, Camera cam)
        {
            if (_zonesLeft <= 0) return;
            
            var ray = cam.ScreenPointToRay(mouse);
            var mask = LayerMask.GetMask("Table Mesh");

            if (Physics.Raycast(ray, out var result, 100.0F, mask))
            {
                var (i, j) = FromWorldToGrid(result.point);
                i = Math.Clamp(i, _zoneSize.y - 1, rows - 1);
                j = Math.Clamp(j, _zoneSize.x - 1, cols - 1);

                for (var x = i; x > i - _zoneSize.y; x--)
                {
                    for (var y = j; y > j - _zoneSize.x; y--)
                    {
                        if (_grid[x, y].state != Room.State.Unreachable) continue;
                        var variant = _grid[x, y].Random.NextDouble() > VariantChance ? 1 : 0;
                        
                        _grid[x, y].state = Room.State.Unvisited;
                        _tileMaterials[x, y].mainTextureOffset = new Vector2(0.5F, 0.5F * variant);
                    }
                }
            }

            zonePreview.gameObject.SetActive(--_zonesLeft > 0);
        }

        private void OnMouseMoved(Vector2 mouse, Camera cam)
        {
            if (_zonesLeft <= 0) return;
            
            zonePreview.gameObject.SetActive(true);

            var ray = cam.ScreenPointToRay(mouse);
            var mask = LayerMask.GetMask("Table Mesh");

            if (Physics.Raycast(ray, out var result, 100.0F, mask))
            {
                var (i, j) = FromWorldToGrid(result.point);
                i = Math.Clamp(i, _zoneSize.y - 1, rows - 1);
                j = Math.Clamp(j, _zoneSize.x - 1, cols - 1);

                // Zone Preview
                var offset = new Vector3(0.25F, 0, -0.25F);
                var pos = FromGridToWorld(i, j);
                zonePreview.position = pos + offset;
                zonePreview.localScale = new Vector3(_zoneSize.x, 1, _zoneSize.y);
            }
        }

        private void SetFigurinePosition(Vector2Int value)
        {
            var (i, j) = (value.y, value.x);
            _playerPosition = value;
            
            // Moving figurine
            var pos = FromGridToWorld(i, j);
            figurine.position = pos;
            
            var room = _grid[i, j];

            if (room.state != Room.State.Visited)
            {
                // Calculating sprite offset
                var index = 0;
                Directions.Values.ForEach((direction, k) =>
                {
                    if (room.doors[direction] != null) index += 1 << k;
                });

                var x = (float) (index % 4) / 4;
                var y = (float) (index / 4) / 4;

                // Setting sprite
                var tileMaterial = _tileMaterials[i, j];
                tileMaterial.mainTexture = visitedTileTexture;
                tileMaterial.mainTextureScale = new Vector2(0.25F, 0.25F);
                tileMaterial.mainTextureOffset = new Vector2(x, y);
            
                // Creating icon if needed
                if (!_iconRenderers.ContainsKey((i, j)) && room.chests.Any(item => item != null))
                {
                    CreateIcon(i, j, 4);
                }
            }
            
            room.state = Room.State.Visited;
        }

        public void Generate()
        {
            var random = new Random();
            var generator = new Generator(rows, cols, horizontalChance, verticalChance, lootChance, random);
            _grid = generator.Generate();
            _iconRenderers.Values.ForEach(x => Destroy(x.transform.parent.gameObject));
            _iconRenderers.Clear();
            
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var material = _tileMaterials[i, j];
                    var variant = _grid[i, j].Random.NextDouble() > VariantChance ? 1 : 0;
                    material.mainTexture = unvisitedTileTexture;
                    material.mainTextureOffset = new Vector2(0, 0.5F * variant);
                    material.mainTextureScale = new Vector2(0.5F, 0.5F);
                }
            }

            // Generate base
            _basePosition = new Vector2Int(random.Next(0, 4), random.Next(0, 4));
            dungeon.SetRoom(_grid[_basePosition.y, _basePosition.x]);
            CreateIcon(_basePosition.y, _basePosition.x, 0);
            SetFigurinePosition(_basePosition);
        }

        private void CreateIcon(int i, int j, int index)
        {
            var pos = FromGridToWorld(i, j);
            var obj = Instantiate(iconPrefab, pos, Quaternion.identity, transform);
            var iconRenderer = obj.GetComponentInChildren<Renderer>();
            obj.name = $"Icon #{index} [{i}, {j}]";
            iconRenderer.material.mainTextureOffset = Vector2.right * (index / 6.0F);
            _iconRenderers.Add((i, j), iconRenderer);
        }

        private Vector3 FromGridToWorld(int i, int j)
        {
            var x = 0.5F + j - (float) cols / 2;
            var z = 0.5F + i - (float) rows / 2;
            return new Vector3(x * cellSize, 0, z * -cellSize);
        }
        
        private (int i, int j) FromWorldToGrid(Vector3 pos)
        {
            var j = (pos.x / cellSize) - 0.5F + (float) cols / 2;
            var i = (pos.z / -cellSize) - 0.5F + (float) rows / 2;
            return (Mathf.RoundToInt(i), Mathf.RoundToInt(j));
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