using System;
using System.Collections.Generic;
using System.Linq;
using Code;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code.MapGeneration.Strategies
{
    [CreateAssetMenu(fileName = nameof(SpawnMapGenerationStrategy), menuName = "MapGeneration/Strategies/Spawn")]
    public class SpawnMapGenerationStrategy : MapGenerationStrategy
    {
        public GameObject Spawn;

        [Range(0, 1)]
        public float AmountOfSpawnsPerPlainTile;

        public float MinimumDistanceBetweenSpawns;

        private GameObject[] _gameObjects;
        
        public override void Apply(TilemapController tilemap)
        {
            // foreach (var tile in tilemap.Tiles)
            // {
            //     if (tile.GroundTileType == GroundTileType.Water)
            //     {
            //         tilemap.SetTile(tile.X, tile.Y, new MapTile(
            //             GroundTileType.Wall,
            //             tile.SmallestDistanceToNextGroundTileType,
            //             tile.HighestDistanceToNextGroundTileType));
            //     }
            // }
            
            var plainTiles = tilemap.Tiles
                .Where(x => x.GroundTileType == GroundTileType.Plain)
                .ToArray();
            // var amountOfSpawns = (int)(plainTiles.Length * AmountOfSpawnsPerPlainTile);
            var amountOfSpawns = 1;
            
            var spawnPlaces = plainTiles.OrderByDescending(x => x.SmallestDistanceToNextGroundTileType);

            var spawnedGameObjects = new LinkedList<GameObject>();
            var spawnedSpawnPlaces = new HashSet<MapTile>();
            foreach (var spawnPlace in spawnPlaces)
            {
                // if(spawnedSpawnPlaces.Any(x => Vector2.Distance(new Vector2(x.X, x.Y), new Vector2(spawnPlace.X, spawnPlace.Y)) < MinimumDistanceBetweenSpawns))
                //     continue;

                // var spawnedObject = Instantiate(
                //     Spawn,
                //     new Vector3(
                //         spawnPlace.X,
                //         spawnPlace.Y,
                //         -3),
                //     Quaternion.LookRotation(
                //         new Vector3(0, 1, 0)));
                // spawnedGameObjects.AddLast(spawnedObject);
                
                tilemap.SetTile(spawnPlace.X-1, spawnPlace.Y-1, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));
                
                tilemap.SetTile(spawnPlace.X-1, spawnPlace.Y+1, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));
                
                tilemap.SetTile(spawnPlace.X-1, spawnPlace.Y, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));
                
                tilemap.SetTile(spawnPlace.X, spawnPlace.Y-1, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));
                
                // tilemap.SetTile(spawnPlace.X, spawnPlace.Y, new MapTile(
                //     GroundTileType.Wall,
                //     spawnPlace.SmallestDistanceToNextGroundTileType,
                //     spawnPlace.HighestDistanceToNextGroundTileType));
                
                tilemap.SetTile(spawnPlace.X+1, spawnPlace.Y+1, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));
                
                tilemap.SetTile(spawnPlace.X+1, spawnPlace.Y-1, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));
                
                tilemap.SetTile(spawnPlace.X, spawnPlace.Y+1, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));
                
                tilemap.SetTile(spawnPlace.X+1, spawnPlace.Y, new MapTile(
                    GroundTileType.Water,
                    spawnPlace.SmallestDistanceToNextGroundTileType,
                    spawnPlace.HighestDistanceToNextGroundTileType));

                spawnedSpawnPlaces.Add(spawnPlace);
                if (spawnedSpawnPlaces.Count >= amountOfSpawns)
                    break;
            }

            _gameObjects = spawnedGameObjects.ToArray();
        }

        public override void OnDestroy()
        {
            if (_gameObjects != null)
            {
                foreach (var gameObject in _gameObjects)
                {
                    GameObjectCleanupUtilities.SafeDestroy(gameObject);
                }
            }

            _gameObjects = null;
        }
    }
}