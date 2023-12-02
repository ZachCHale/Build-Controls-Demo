using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.Extensions;
using ZHDev.TileMaps;

public class DemoTileableObject : TileableObject
{
    private GameObject _gameObjectInstance;
    private TileObjectData _resourceData;
    private CardinalDirection _forwardFacingDirection;
    private List<TileMap.Tile> _coveredTiles;
    
    public DemoTileableObject(TileObjectData resourceData, CardinalDirection forwardFacingDirection)
    {
        _resourceData = resourceData;
        _forwardFacingDirection = forwardFacingDirection;
        _coveredTiles = new();
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
        
        List<Vector2Int> allCoveredIndexes =
            GetAllCoveredTilIndices(targetTile.TileIndex);

        foreach (var index in allCoveredIndexes)
        {
            Debug.Log(index);
            targetTile.TileMap.GetTileAt(index).AddToTileUnhandled(this);
            _coveredTiles.Add(targetTile.TileMap.GetTileAt(index));
        }
    }

    protected override void OnRemovedFromTile()
    {
        Object.Destroy(_gameObjectInstance);
        foreach (var tile in _coveredTiles)
        {
            tile.RemoveFromTileUnhandled();
        }
    }
    
    private List<Vector2Int> GetAllCoveredTilIndices(Vector2Int originIndex)
    {
        List<Vector2Int> transformedVectors = new();
        foreach (Vector2Int vec in _resourceData.OccupiedSpaces)
            transformedVectors.Add(vec.RotateDegrees(_forwardFacingDirection.ToDegreesPastNorth()) + originIndex);
        return transformedVectors;
    }
}
