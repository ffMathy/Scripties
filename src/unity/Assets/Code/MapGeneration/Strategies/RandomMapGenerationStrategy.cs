using System;
using UnityEngine;

namespace Assets.Code.MapGeneration.Strategies
{
    [CreateAssetMenu(fileName = nameof(RandomMapGenerationStrategy), menuName = "MapGeneration/Strategies/Random")]
    public class RandomMapGenerationStrategy : MapGenerationStrategy
    {
        public override void Apply(TilemapController tilemap)
        {
            var validEnumValues = (GroundTileType[])Enum.GetValues(typeof(GroundTileType));
            for (var x=0; x < tilemap.Width; x++)
            {
                for (var y = 0; y < tilemap.Height; y++)
                {
                    var randomValue = validEnumValues[UnityEngine.Random.Range(0, validEnumValues.Length)];
                    tilemap.SetTile(x, y, new MapTile(
                        randomValue,
                        float.MinValue,
                        float.MaxValue)); 
                }
            }
        }

        public override void OnDestroy()
        {
            
        }
    }
}