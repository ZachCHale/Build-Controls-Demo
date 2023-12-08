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
        private readonly TileableBuildingManager _buildingManager;
        public GameObject GameObjectInstance => _gameObjectInstance;
        public TileObjectData ResourceData => _resourceData;
        public TileableBuildingManager BuildingManager => _buildingManager;
        public TileableBuilding(TileableBuildingManager buildingManager, TileObjectData tileableData) 
        {
            _resourceData = tileableData;
            _buildingManager = buildingManager;
            TileablePlane plane = buildingManager.TileablePlane;
            _gameObjectInstance = Object.Instantiate(original: _resourceData.PrefabReference);
        }

        protected override void OnRegisteredToIndices(List<Vector2Int> ownedIndices)
        {

        }

        protected override void OnCleared(List<Vector2Int> clearedIndices)
        {
            Debug.Log("onCleared");
            if(_gameObjectInstance != null)
                Object.Destroy(_gameObjectInstance);
        }
    }
}
