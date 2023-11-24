using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZHDev.CardinalDirections;

namespace ZHDev.TileMaps
{
    public partial class TileMap : MonoBehaviour
    {
        private readonly int _tileCountX = 10;
        private readonly int _tileCountY = 20;
        private readonly float _tileSize = 1f;
        private Camera _mainCamera;
    
        private readonly Dictionary<Vector2Int, Tile>_tiles = new ();
        private Vector3 _mapOriginWorld => transform.position;
        private Vector3 _mapEndWorld => transform.position + _tileSize * ((_mapXDir * _tileCountX) + (_mapYDir * _tileCountY));
        private Vector3 _mapOriginLocal => Vector3.zero;
        private Vector3 _mapEndLocal => new (_tileSize * _tileCountX, 0, _tileSize * _tileCountY);

        private Vector3 _mapNormal => transform.up;
        private Vector3 _mapXDir => transform.right;
        private Vector3 _mapYDir => transform.forward;
    
        private void Awake()
        {
            _mainCamera = Camera.main;
            foreach (int x in Enumerable.Range(0, _tileCountX))
            foreach (int y in Enumerable.Range(0, _tileCountY))
                _tiles.Add(new Vector2Int(x,y), new Tile(this, new(x,y)));
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
        
            return new Vector3(localX, 0, localY);
        }
        /// <summary>
        /// <para>For a given position in world space, finds the index of the tile containing that point. </para>
        /// </summary>
        /// <param name="worldPos">The position to find the tile index of.</param>
        /// <param name="mapIndex">When this method returns, contains the index for the tile containing the point if the point
        /// is within the bounds of the map; otherwise, contains the index of the closest edge tile.</param>
        /// <returns><c>true</c> if the position is within the bounds of the map. <c>false</c> if the position is out of bounds.</returns>
        public bool WorldSpaceToMapTileIndex(Vector3 worldPos, out Vector2Int mapIndex)
        {
            Vector3 localPos = WorldSpaceToLocalMapSpace(worldPos);
            bool withinBounds = IsLocalPositionInBounds(localPos);
            int xIndex = (int)Math.Clamp(localPos.x / _tileSize, _mapOriginLocal.x, _mapEndLocal.x);
            int yIndex = (int)Math.Clamp(localPos.z / _tileSize, _mapOriginLocal.z, _mapEndLocal.z);
        
            mapIndex = new(xIndex, yIndex);
            return withinBounds;
        }
        /// <summary>
        /// For a given <c>TileObjectData</c>, adds it to the tile under the mouse.
        /// </summary>
        /// <param name="objData"><c>TileObjectData</c> to add to a tile.</param>
        /// <returns><c>true</c> if the new object was placed successfully. <c>false</c> the tile was already occupied by an object,
        /// or if the mouse is pointing out of the bounds, in the opposite direction of the map, or parallel to the map. </returns>
        public bool PlaceObjectAtMousePosition(TileObjectData objData, CardinalDirection objectFacingDirection)
        {
            bool validPosition = _mainCamera.GetScreenPointIntersectionWithPlane(Input.mousePosition, _mapNormal, _mapOriginWorld, out Vector3 worldPos);
            bool inBounds = WorldSpaceToMapTileIndex(worldPos, out Vector2Int mapIndex);
        
            if (!inBounds || !validPosition) return false;
        
            bool objectWasPlaced = !_tiles[mapIndex].PlaceNewObject(objData, objectFacingDirection);

            return objectWasPlaced;
        }
    
        /// <summary>
        /// Attempts to remove an object from the tile under the mouse pointer.
        /// </summary>
        /// <param name="removedObjData">When this method returns, contains the <c>TileObjectData</c> of the object removed if there was an
        /// object removed; otherwise, defaults to null.</param>
        /// <returns><c>true</c> if an object was removed successfully. <c>false</c> if no object was removed.</returns>
        public bool RemoveObjectAtMousePosition(out TileObjectData removedObjData)
        {
            bool validPosition = _mainCamera.GetScreenPointIntersectionWithPlane(Input.mousePosition, _mapNormal, _mapOriginWorld, out Vector3 worldPos);
            bool inBounds = WorldSpaceToMapTileIndex(worldPos, out Vector2Int mapIndex);
            if (!inBounds || !validPosition)
            {
                removedObjData = null;
                return false;
            }
        
            bool objectWasRemoved = !_tiles[mapIndex].RemoveObject(out removedObjData);
            return objectWasRemoved;
        }

        private bool IsIndexInBounds(Vector2Int tileIndex) => tileIndex.x >= 0 && tileIndex.x < _tileCountX && tileIndex.y >= 0 && tileIndex.y < _tileCountY;
        private bool IsLocalPositionInBounds(Vector3 localPosition) => localPosition.x >= _mapOriginLocal.x && 
                                                                       localPosition.x <= _mapEndLocal.x && 
                                                                       localPosition.z >= _mapOriginLocal.z &&
                                                                       localPosition.z <=_mapEndLocal.z;
    }
}

