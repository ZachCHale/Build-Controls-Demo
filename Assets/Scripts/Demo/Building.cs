using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.TileMaps;

namespace Demo
{
    public class Building
    {
        private readonly GameObject _gameObjectInstance;
        private readonly TileObjectData _resourceData;
        private readonly TileableBuildingManager _buildingManager;
        public GameObject GameObjectInstance => _gameObjectInstance;
        public TileObjectData ResourceData => _resourceData;
        public TileableBuildingManager BuildingManager => _buildingManager;
        public Building(TileableBuildingManager buildingManager, TileObjectData tileableData, GameObject gameObjectInstance) 
        {
            _resourceData = tileableData;
            _buildingManager = buildingManager;
            TileablePlane plane = buildingManager.TileablePlane;
            _gameObjectInstance = gameObjectInstance;
        }
        
    }
}
