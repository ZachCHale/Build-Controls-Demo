using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.Extensions;
using ZHDev.TileMaps;

public class DemoTileableObject : StaticTileable
{
    private GameObject _gameObjectInstance;
    private TileObjectData _resourceData;
    private CardinalDirection _forwardFacingDirection;
    private List<Tile> _coveredTiles;
    private Tile _pivotTile;
    private List<Vector2Int> _coveredTileIndices;
    private Vector2Int _pivotTileIndex;
    
    public DemoTileableObject(TileObjectData resourceData, CardinalDirection forwardFacingDirection)
    {
        _resourceData = resourceData;
        _forwardFacingDirection = forwardFacingDirection;
        _coveredTiles = new();
    }
    
    
    /*
    
    protected override void OnAddedToTile(Tile targetTile)
    {
        _pivotTile = targetTile;
        CreateAndPlaceGameObjectOnPivotTile();
        AssignObjectToCoveredTiles();
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

    private void CreateAndPlaceGameObjectOnPivotTile()
    {
        if (_gameObjectInstance == null)
        {
            _gameObjectInstance = Object.Instantiate(_resourceData.PrefabReference);
        }
        _pivotTile.TileMap.ParentToMapTransform(_gameObjectInstance.transform);
        _gameObjectInstance.transform.localPosition = _pivotTile.LocalPosition;
        _gameObjectInstance.transform.localRotation = _forwardFacingDirection.ToRotationFromNorth();
    }

    private void AssignObjectToCoveredTiles()
    {
        List<Vector2Int> allCoveredIndices = GetAllCoveredTilIndices(_pivotTile.TileIndex);
        _coveredTiles = _pivotTile.TileMap.GetTiles(allCoveredIndices);
        foreach (var t in _coveredTiles)
        {
            t.AddToTileUnhandled(this);
        }
    }*/
    protected override void OnCreated()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDeleted()
    {
        throw new System.NotImplementedException();
    }
}
