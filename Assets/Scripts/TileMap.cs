using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    private class Tile
    {
        private TileMap _tileMap;
        public readonly Vector3 WorldPosition;
        public readonly Vector2 MapIndex;
        private PlaceableObjInstance _placedObject;
        /// <summary>
        /// <para>Creates a tile and initializes its world position based on the given <c>TileMap</c> and tile index.</para>
        /// </summary>
        /// <param name="tileMap">The <c>TileMap</c> containing this tile.</param>
        /// <param name="mapIndex">The index/key for this tile in the <c>TileMap</c></param>
        public Tile(TileMap tileMap, Vector2 mapIndex)
        {
            _tileMap = tileMap;
            MapIndex = mapIndex;
            float tileCenterOffset = _tileMap._tileSize / 2;
            Vector3 mapCornerCenterOffsetPosition = _tileMap._mapOriginWorld + new Vector3(tileCenterOffset, 0, tileCenterOffset);
            WorldPosition = mapCornerCenterOffsetPosition + new Vector3(tileMap._tileSize * MapIndex.x, 0, tileMap._tileSize * mapIndex.y);
        }
        /// <summary>
        /// <para>For a given <c>PlaceableObjData</c>, dispose of any object instance in the tile and create a new
        /// <c>PlaceableObjectInstance</c> at the center of the tile.</para>
        /// </summary>
        /// <param name="objData"><c>PlaceableObjData</c> to create a <c>PlaceableObjectInstance</c> from.</param>
        /// <param name="removedObjData">When this method returns, contains the <c>PlaceableObjData</c> of the <c>PlaceableObjectInstance</c>
        /// that was removed from the tile. If no tile was removed, remains null.</param>
        /// <returns><c>true</c> if the tile was empty. <c>false</c> if an object was removed to add this one.</returns>
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
    /// <summary>
    /// <para>For a given position in world space, returns the position relative to the map's position and orientation.</para>
    /// </summary>
    /// <param name="worldPos">Position to convert to a local map position.</param>
    /// <returns>New position relative to the map's position and orientation.</returns>
    public Vector3 WorldSpaceToLocalMapSpace(Vector3 worldPos)
    {
        Vector3 xProjection = Vector3.Project(worldPos - _mapOriginWorld, _mapXDir);
        Vector3 yProjection = Vector3.Project(worldPos - _mapOriginWorld,_mapYDir);

        float localX = xProjection.magnitude * Math.Sign(Vector3.Dot(xProjection, _mapXDir));
        float localY = yProjection.magnitude * Math.Sign(Vector3.Dot(yProjection, _mapYDir));
        
        return new(localX, 0, localY);
    }
    /// <summary>
    /// <para>For a given position in world space, finds the index of the tile containing that point. </para>
    /// </summary>
    /// <param name="worldPos">The position to find the tile index of.</param>
    /// <param name="mapIndex">When this method returns, contains the index for the tile containing the point if the point
    /// is within the bounds of the map; otherwise, contains the index of the closest edge tile.</param>
    /// <returns><c>true</c> if the position is within the bounds of the map. <c>false</c> if the position is out of bounds.</returns>
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
    /// <summary>
    /// For a given <c>PlaceableObjData</c>, Creates a <c>PlaceableObjInstance</c> and adds it to the tile under the mouse.
    /// </summary>
    /// <param name="objData"><c>PlaceableObjData</c> to create a <c>PlaceableObjInstance</c> from.</param>
    /// <returns><c>true</c> if the object was placed successfully. <c>false</c> if the mouse is pointing out of the bounds,
    /// in the opposite direction of the map, or parallel to the map. </returns>
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

