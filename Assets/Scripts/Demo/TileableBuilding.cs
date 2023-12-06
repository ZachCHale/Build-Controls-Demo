using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.TileMaps;

namespace Demo
{
    public class TileableBuilding : StaticTileable
    {
        private readonly GameObject _gameObjectInstance;
        private readonly TileObjectData _resourceData;
        
        public TileableBuilding(TileableManager tileableManager, Vector2Int pivotIndex, TileObjectData tileableData, CardinalDirection facingDirection) 
            : base(tileableManager, tileableData.GetTransformIndices(pivotIndex, facingDirection))
        {
            _resourceData = tileableData;
            _gameObjectInstance = Object.Instantiate(original: tileableData.PrefabReference, 
                parent: _manager.transform,
                position: _manager.IndexToWorld(pivotIndex),
                rotation: facingDirection.ToRotationFromNorth());
        }

        protected override void OnRegisteredToAllIndices()
        { }

        protected override void OnDeleted()
        {
            Object.Destroy(_gameObjectInstance);
        }

    }
}
