using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.TileMaps;

public class DemoTileableObject : TileableObject
{
    private GameObject _gameObjectInstance;
    private TileObjectData _resourceData;
    private CardinalDirection _forwardFacingDirection;
    public DemoTileableObject(TileObjectData resourceData, CardinalDirection forwardFacingDirection)
    {
        _resourceData = resourceData;
        _forwardFacingDirection = forwardFacingDirection;
    }
    protected override void OnAddedToTile(TileMap.Tile targetTile)
    {
        if (_gameObjectInstance == null)
        {
            _gameObjectInstance = Object.Instantiate(_resourceData.PrefabReference);
            _gameObjectInstance.SetActive(false);
        }
        targetTile.TileMap.ParentToMapTransform(_gameObjectInstance.transform);
        _gameObjectInstance.transform.localPosition = targetTile.LocalPosition;
        _gameObjectInstance.transform.localRotation = _forwardFacingDirection.ToRotationFromNorth();
        _gameObjectInstance.SetActive(true);
    }

    protected override void OnRemovedFromTile()
    {
        Object.Destroy(_gameObjectInstance);
    }
}
