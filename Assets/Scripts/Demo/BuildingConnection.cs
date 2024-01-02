using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.TileableSystems;
using Object = System.Object;

namespace Demo
{
    public class BuildingConnection
    {
        private readonly GameObject _gameObjectInstance;
        private readonly TileableBuildingManager _buildingManager;
        private readonly BuildingConnectionData _buildingConnectionData;
        public GameObject GameObjectInstance => _gameObjectInstance;
        public TileableBuildingManager BuildingManager => _buildingManager;
        public BuildingConnectionData BuildingConnectionData => _buildingConnectionData;

        private GameObject _jointInstance = null;
        private GameObject _northInstance = null;
        private GameObject _eastInstance = null;
        private GameObject _southInstance = null;
        private GameObject _westInstance = null;

        public GameObject GetConnectionInstanceByDirection(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.North:
                    return _northInstance;
                    break;
                case CardinalDirection.East:
                    return _eastInstance;
                    break;
                case CardinalDirection.South:
                    return _southInstance;
                    break;
                case CardinalDirection.West:
                    return _westInstance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public void SetConnectionInstanceByDirection(CardinalDirection direction, GameObject newInstance)
        {
            switch (direction)
            {
                case CardinalDirection.North:
                    _northInstance = newInstance;
                    break;
                case CardinalDirection.East:
                    _eastInstance = newInstance;
                    break;
                case CardinalDirection.South:
                    _southInstance = newInstance;
                    break;
                case CardinalDirection.West:
                    _westInstance = newInstance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
        
        public GameObject GetJointInstance()
        {
            return _jointInstance;
        }

        public void SetJointInstance(GameObject newInstance)
        {
            _jointInstance = newInstance;
        }
        
        public BuildingConnection(TileableBuildingManager buildingManager, BuildingConnectionData connectionData, GameObject gameObjectInstance) 
        {
            _buildingManager = buildingManager;
            _gameObjectInstance = gameObjectInstance;
            _buildingConnectionData = connectionData;
        }
        
    }
}