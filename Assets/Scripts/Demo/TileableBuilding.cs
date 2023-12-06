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
        
        public TileableBuilding(TileablePlane tileablePlane, Vector2Int pivotIndex, TileObjectData tileableData, CardinalDirection facingDirection) 
            : base(tileablePlane.GetTileableManager<TileableBuilding>(), tileableData.GetTransformIndices(pivotIndex, facingDirection))
        {
            _resourceData = tileableData;
            _gameObjectInstance = Object.Instantiate(original: tileableData.PrefabReference, 
                parent: tileablePlane.transform,
                position: tileablePlane.IndexToWorld(pivotIndex),
                rotation: tileablePlane.transform.rotation * facingDirection.ToRotationFromNorth());
        }

        protected override void OnRegisteredToAllIndices()
        { }

        protected override void OnDeleted()
        {
            Object.Destroy(_gameObjectInstance);
        }

    }
}
