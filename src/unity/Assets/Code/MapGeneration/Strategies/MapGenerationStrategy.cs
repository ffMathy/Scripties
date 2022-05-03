using System;
using UnityEngine;

namespace Assets.Code.MapGeneration.Strategies
{
    public abstract class MapGenerationStrategy : ScriptableObject
    {
        public abstract void Apply(TilemapController tilemap);

        public abstract void OnDestroy();
    }
}