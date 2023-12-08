using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.TileMaps;

namespace Demo
{
    public class TileableBuildingManager : MonoBehaviour
    {
        [SerializeField] private TileablePlane _tileablePlane;
        public TileablePlane TileablePlane => _tileablePlane;
        
        private StaticTileableManager<TileableBuilding> _tileableManager = new();
        public TileableManager<TileableBuilding> TileableManager => _tileableManager;

        public TileableBuilding PlaceNewBuilding(TileObjectData buildingData, Vector3 positionToAdd, CardinalDirection facingDirection)
        {
            TileableBuilding newBuilding = new TileableBuilding(this, buildingData);
            Transform buildingTransform = newBuilding.GameObjectInstance.transform;
            Vector2Int closestIndex = _tileablePlane.WorldToProjectedIndex(positionToAdd);
            buildingTransform.parent = _tileablePlane.transform;
            buildingTransform.position = _tileablePlane.IndexToWorld(closestIndex);
            buildingTransform.rotation = facingDirection.ToRotationFromNorth();
            _tileableManager.RegisterAsOwner(buildingData.GetTransformIndices(closestIndex,facingDirection), newBuilding);
            return newBuilding;
        }

        public bool TryRemoveBuildingAt(Vector3 positionToRemove)
        {
            Vector2Int closestIndex = _tileablePlane.WorldToProjectedIndex(positionToRemove);
            return _tileableManager.FreeIndex(closestIndex);
        }
        

    }
}
