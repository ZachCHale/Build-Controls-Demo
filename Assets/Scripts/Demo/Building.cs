using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.TileableSystems;

namespace Demo
{
    public class Building
    {
        private readonly GameObject _gameObjectInstance;
        private readonly BuildingData _resourceData;
        private readonly TileableBuildingManager _buildingManager;
        public GameObject GameObjectInstance => _gameObjectInstance;
        public BuildingData ResourceData => _resourceData;
        public TileableBuildingManager BuildingManager => _buildingManager;
        public Building(TileableBuildingManager buildingManager, BuildingData tileableData, GameObject gameObjectInstance) 
        {
            _resourceData = tileableData;
            _buildingManager = buildingManager;
            TileablePlane plane = buildingManager.TileablePlane;
            _gameObjectInstance = gameObjectInstance;
        }
        
    }
}
