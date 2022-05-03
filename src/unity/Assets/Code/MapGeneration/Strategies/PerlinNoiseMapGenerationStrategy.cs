using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Code.MapGeneration.Strategies
{
    [CreateAssetMenu(fileName = nameof(PerlinNoiseMapGenerationStrategy), menuName = "MapGeneration/Strategies/PerlinNoise")]
    public class PerlinNoiseMapGenerationStrategy : MapGenerationStrategy
    {
        [SerializeField] 
        private NoiseValues[] TileTypes;

        [Header("Noise settings")] 
        public int Octaves;

        [Range(0, 1)] 
        public float Persistance;
        public float Lacunarity;
        public float Scale;

        public bool ShouldApplyRoomGradient;

        public int Seed;

        public Vector2 Offset;

        public override void Apply(TilemapController tilemap)
        {
            TileTypes = TileTypes.OrderBy(a => a.Height).ToArray();

            var noiseMap = GenerateNoiseMap(
                tilemap.Width,
                tilemap.Height);

            if (ShouldApplyRoomGradient)
            {
                var roomGradient = GenerateRoomGradientMap(
                    tilemap.Width,
                    tilemap.Height);
                for (int x = 0, y; x < tilemap.Width; x++)
                {
                    for (y = 0; y < tilemap.Height; y++)
                    {
                        var subtractedValue = noiseMap[y * tilemap.Width + x] - roomGradient[y * tilemap.Width + x];
                        noiseMap[y * tilemap.Width + x] = Mathf.Clamp01(subtractedValue);
                    }
                }
            }

            for (var x = 0; x < tilemap.Width; x++)
            {
                for (var y = 0; y < tilemap.Height; y++)
                {
                    var currentHeight = GetNoiseMapHeight(noiseMap, tilemap, y, x);
                    
                    var previousHeightTileType = 
                        TileTypes.FirstOrDefault(tileType => tileType.Height <= currentHeight) ??
                        TileTypes.First();
                    float? distanceToPreviousHeight = previousHeightTileType != null ? 
                        Math.Abs(currentHeight - previousHeightTileType.Height) : 
                        null;
                    
                    var nextHeightTileType = TileTypes.FirstOrDefault(tileType => tileType.Height > currentHeight);
                    float? distanceToNextHeight = nextHeightTileType != null ?
                        Math.Abs(currentHeight - nextHeightTileType.Height) :
                        null;
                    
                    var currentHeightTileType = distanceToNextHeight < distanceToPreviousHeight ? 
                        nextHeightTileType : 
                        previousHeightTileType;

                    tilemap.SetTile(x, y, new MapTile(
                        currentHeightTileType.GroundTile,
                        Math.Min(distanceToNextHeight ?? float.MaxValue, distanceToPreviousHeight ?? float.MaxValue),
                        Math.Max(distanceToNextHeight ?? float.MinValue, distanceToPreviousHeight ?? float.MinValue)));
                }
            }
        }

        private static float GetNoiseMapHeight(float[] noiseMap, TilemapController tilemap, int y, int x)
        {
            return noiseMap[y * tilemap.Width + x];
        }

        public override void OnDestroy()
        {
            
        }

        private float[] GenerateNoiseMap(
            int mapWidth,
            int mapHeight)
        {
            var noiseMap = new float[mapWidth * mapHeight];

            var random = new System.Random(Seed);
            if (Octaves < 1)
            {
                Octaves = 1;
            }

            var octaveOffsets = new Vector2[Octaves];
            for (var i = 0; i < Octaves; i++)
            {
                var offsetX = random.Next(-100000, 100000) + Offset.x;
                var offsetY = random.Next(-100000, 100000) + Offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (Scale <= 0f)
            {
                Scale = 0.0001f;
            }

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;

            for (int x = 0, y; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    for (var i = 0; i < Octaves; i++)
                    {
                        var sampleX = (x - halfWidth) / Scale * frequency + octaveOffsets[i].x;
                        var sampleY = (y - halfHeight) / Scale * frequency + octaveOffsets[i].y;

                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += perlinValue * amplitude;
                        amplitude *= Persistance;
                        frequency *= Lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[y * mapWidth + x] = noiseHeight;
                }
            }

            for (var x = 0; x < mapWidth; x++)
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    noiseMap[y * mapWidth + x] =
                        Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y * mapWidth + x]);
                }
            }

            return noiseMap;
        }

        private static float[] GenerateRoomGradientMap(int mapWidth, int mapHeight)
        {
            var map = new float[mapWidth * mapHeight];
            for (var x = 0; x < mapWidth; x++)
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    var i = x / (float)mapWidth * 2 - 1;
                    var j = y / (float)mapHeight * 2 - 1;

                    var value = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));

                    var a = 3f;
                    var b = 2.2f;
                    var gradientValue = Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

                    map[y * mapWidth + x] = gradientValue;
                }
            }

            return map;
        }

        [Serializable]
        class NoiseValues
        {
            [Range(0f, 1f)] public float Height;

            public GroundTileType GroundTile;
        }
    }
}