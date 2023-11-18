using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TileMap : MonoBehaviour
{
    private class Tile
    {
        private TileMap _tileMap;
        public readonly Vector3 WorldPosition;
        public readonly Vector2 MapIndex;
        private PlaceableObjInstance _placedObject;
        
        public Tile(TileMap tileMap, Vector2 mapIndex)
        {
            _tileMap = tileMap;
            MapIndex = mapIndex;
            WorldPosition = tileMap._mapOrigin + new Vector3(_tileMap._gapDistance / 2, 0, _tileMap._gapDistance / 2) +
                            new Vector3(tileMap._gapDistance * MapIndex.x, 0, tileMap._gapDistance * mapIndex.y);
        }

        public PlaceableObjData PlaceNewObject(PlaceableObjData data)
        {
            PlaceableObjData prevObjectData = _placedObject?.Data;
            _placedObject?.Dispose();
            _placedObject = new PlaceableObjInstance(data, WorldPosition);
            return prevObjectData;
        }
        
        
    }
    
    private readonly int _lengthX = 10;
    private readonly int _lengthY = 20;
    private readonly float _gapDistance = 1f;
    private Camera _mainCamera;


    private readonly Dictionary<Vector2, Tile>_tiles = new ();
    private Vector3 _mapOrigin => transform.position;
    private Vector3 _mapNormal => transform.up;
    private Vector3 _mapXDir => transform.right;
    private Vector3 _mapYDir => transform.forward;

    
    private void Awake()
    {
        _mainCamera = Camera.main;
        foreach (int x in Enumerable.Range(0, _lengthX-1))
            foreach (int y in Enumerable.Range(0, _lengthY-1))
                _tiles.Add(new(x,y), new Tile(this, new(x,y)));
    }

    public Vector3 WorldSpaceToMapSpace(Vector3 worldPos)
    {
        Vector3 xProjection = Vector3.Project(worldPos - _mapOrigin, transform.right);
        Vector3 yProjection = Vector3.Project(worldPos - _mapOrigin,transform.forward);

        float localX = xProjection.magnitude * Math.Sign(Vector3.Dot(xProjection, _mapXDir));
        float localY = yProjection.magnitude * Math.Sign(Vector3.Dot(yProjection, _mapYDir));
        
        return new(localX, 0, localY);
    }

    public Vector2 WorldSpaceToMapIndex(Vector3 worldPos)
    {
        Vector3 localPos = WorldSpaceToMapSpace(worldPos);
        if (localPos.x < 0 || localPos.x > _lengthX * _gapDistance || localPos.z < 0 ||
            localPos.z > _lengthY * _gapDistance)
            return new (-1, -1);
        return new((int)(localPos.x / _gapDistance), (int)(localPos.z / _gapDistance));
    }

    public bool PlaceObjectAtMousePosition(PlaceableObjData objData)
    {
        Vector3 worldPos = _mainCamera.GetScreenPointIntersectionWithPlane(Input.mousePosition, _mapNormal, _mapOrigin);
        Vector2 mapIndex = WorldSpaceToMapIndex(worldPos);
        
        if (mapIndex.Equals(new(-1, -1))) return false;

        Assert.IsTrue(_tiles.ContainsKey(mapIndex));
        
        PlaceableObjData oldObjData = _tiles[mapIndex].PlaceNewObject(objData);
        if(oldObjData != null)Debug.Log($"Removed:{oldObjData.Label}");
        return true;
    }
}

