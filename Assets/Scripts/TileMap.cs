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
            float tileOffset = _tileMap._tileSize / 2;
            Vector3 mapCornerOffsetPosition = _tileMap._mapOriginWorld + new Vector3(tileOffset, 0, tileOffset);
            WorldPosition = mapCornerOffsetPosition + new Vector3(tileMap._tileSize * MapIndex.x, 0, tileMap._tileSize * mapIndex.y);
        }

        public bool PlaceNewObject(PlaceableObjData objData, out PlaceableObjData removedObjData)
        {
            bool tileWasEmpty = _placedObject == null;
            removedObjData  = _placedObject?.Data;
            _placedObject?.Dispose();
            _placedObject = new PlaceableObjInstance(objData, WorldPosition);
            return tileWasEmpty;
        }
    }
    
    private readonly int _tileCountX = 10;
    private readonly int _tileCountY = 20;
    private readonly float _tileSize = 1f;
    private Camera _mainCamera;
    
    private readonly Dictionary<Vector2, Tile>_tiles = new ();
    private Vector3 _mapOriginWorld => transform.position;
    private Vector3 _mapEndWorld => transform.position + _tileSize * ((_mapXDir * _tileCountX) + (_mapYDir * _tileCountY));
    private Vector3 _mapOriginLocal => Vector3.zero;
    private Vector3 _mapEndLocal => new Vector3(_tileSize * _tileCountX, 0, _tileSize * _tileCountY);

    private Vector3 _mapNormal => transform.up;
    private Vector3 _mapXDir => transform.right;
    private Vector3 _mapYDir => transform.forward;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        foreach (int x in Enumerable.Range(0, _tileCountX))
            foreach (int y in Enumerable.Range(0, _tileCountY))
                _tiles.Add(new(x,y), new Tile(this, new(x,y)));
    }

    public Vector3 WorldSpaceToLocalMapSpace(Vector3 worldPos)
    {
        Vector3 xProjection = Vector3.Project(worldPos - _mapOriginWorld, _mapXDir);
        Vector3 yProjection = Vector3.Project(worldPos - _mapOriginWorld,_mapYDir);

        float localX = xProjection.magnitude * Math.Sign(Vector3.Dot(xProjection, _mapXDir));
        float localY = yProjection.magnitude * Math.Sign(Vector3.Dot(yProjection, _mapYDir));
        
        return new(localX, 0, localY);
    }
    
    public bool WorldSpaceToMapTileIndex(Vector3 worldPos, out Vector2 mapIndex)
    {
        Vector3 localPos = WorldSpaceToLocalMapSpace(worldPos);
        bool withinBounds = localPos.x >= _mapOriginLocal.x && 
                             localPos.x <= _mapEndLocal.x && 
                             localPos.z >= _mapOriginLocal.z &&
                             localPos.z <=_mapEndLocal.z;
        int xIndex = (int)Math.Clamp(localPos.x / _tileSize, _mapOriginLocal.x, _mapEndLocal.x);
        int yIndex = (int)Math.Clamp(localPos.z / _tileSize, _mapOriginLocal.z, _mapEndLocal.z);
        
        mapIndex = new(xIndex, yIndex);
        return withinBounds;
    }

    public bool PlaceObjectAtMousePosition(PlaceableObjData objData)
    {
        bool validPosition = _mainCamera.GetScreenPointIntersectionWithPlane(Input.mousePosition, _mapNormal, _mapOriginWorld, out Vector3 worldPos);
        bool inBounds = WorldSpaceToMapTileIndex(worldPos, out Vector2 mapIndex);
        
        if (!inBounds || !validPosition) return false;
        
        bool objectWasRemoved = !_tiles[mapIndex].PlaceNewObject(objData, out PlaceableObjData removedObjData);
        if(objectWasRemoved)Debug.Log($"Removed:{removedObjData.Label}");
        return true;
    }
}

