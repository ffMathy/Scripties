using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.MapGeneration.Strategies;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Assets.Code.MapGeneration
{
    [ExecuteInEditMode]
    public class TilemapController : MonoBehaviour
    {
        public int Width;
        public int Height;
        public int TileSize;

        [SerializeField] 
        private MapGenerationStrategy[] Strategies;

        [SerializeField] 
        private TileType[] TileTypes;

        public MapTile[] Tiles;
        
        private Tilemap _unityTilemap;
        private Dictionary<GroundTileType, Tile> _unityTileTypeDictionary;

        public MapTile GetTile(int x, int y)
        {
            return InBounds(x, y) ? Tiles[y * Width + x] : null;
        }

        public void SetTile(int x, int y, MapTile value)
        {
            if (!InBounds(x, y)) 
                return;
            
            value.X = x;
            value.Y = y;
                
            Tiles[y * Width + x] = value;
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void RenderAllTiles()
        {
            var positionsArray = new Vector3Int[Width * Height];
            var tilesArray = new Tile[Width * Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    positionsArray[x * Width + y] = new Vector3Int(x, y, 0);

                    var tile = GetTile(x, y);
                    tilesArray[x * Width + y] = _unityTileTypeDictionary[tile.GroundTileType];
                }
            }

            _unityTilemap.SetTiles(positionsArray, tilesArray);
            _unityTilemap.RefreshAllTiles();
        }

        private void OnDestroy()
        {
            foreach (var strategy in Strategies)
            {
                strategy.OnDestroy();
            }
        }

        private void Awake()
        {
            _unityTilemap = GetComponent<Tilemap>();
            Tiles = new MapTile[Width * Height];

            _unityTileTypeDictionary = new Dictionary<GroundTileType, Tile>();

            var tileSprite = Sprite.Create(
                new Texture2D(TileSize, TileSize),
                new Rect(0, 0, TileSize, TileSize),
                new Vector2(0.5f, 0.5f), TileSize);

            foreach (var tileType in TileTypes)
            {
                var tile = ScriptableObject.CreateInstance<Tile>();
                tile.color = tileType.Color;
                tile.sprite = tileSprite;

                _unityTileTypeDictionary.Add(tileType.GroundTile, tile);
            }

            foreach (var strategy in Strategies)
            {
                strategy.Apply(this);
            }

            RenderAllTiles();
        }

        [Serializable]
        class TileType
        {
            public GroundTileType GroundTile;
            public Color Color;
        }
    }

    public class MapTile
    {
        public GroundTileType GroundTileType { get; }
            
        public float SmallestDistanceToNextGroundTileType { get; }
        public float HighestDistanceToNextGroundTileType { get;  }
        
        public int X { get; internal set; }
        public int Y { get; internal set; }

        public MapTile(
            GroundTileType groundTileType, 
            float smallestDistanceToNextGroundTileType, 
            float highestDistanceToNextGroundTileType)
        {
            GroundTileType = groundTileType;
            SmallestDistanceToNextGroundTileType = smallestDistanceToNextGroundTileType;
            HighestDistanceToNextGroundTileType = highestDistanceToNextGroundTileType;
        }
    }
}