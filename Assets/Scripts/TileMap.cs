using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CardinalDirections;

public class TileMap : MonoBehaviour
{
    private class Tile
    {
        private TileMap _tileMap;
        public Vector3 WorldPosition => _tileMap.transform.TransformPoint(LocalPosition);
        public readonly Vector3 LocalPosition;
        public readonly Vector2Int TileIndex;
        private TileObjectData _objectData;
        private GameObject _objectInstance;
        private bool _tileIsOccupied;
        private List<Tile> _tilesContainingSameObject;
        /// <summary>
        /// <para>Creates a tile and calculates its local position based on the given <c>TileMap</c> and tile index.</para>
        /// </summary>
        /// <param name="tileMap">The <c>TileMap</c> containing this tile.</param>
        /// <param name="tileIndex">The index/key for this tile in the <c>TileMap</c></param>
        public Tile(TileMap tileMap, Vector2Int tileIndex)
        {
            _tileMap = tileMap;
            TileIndex = tileIndex;
            float tileCenterOffset = _tileMap._tileSize / 2;
            LocalPosition = new Vector3(tileCenterOffset, 0, tileCenterOffset) +
                            new Vector3(tileMap._tileSize * TileIndex.x, 0, tileMap._tileSize * tileIndex.y);
            _tileIsOccupied = false;
            _tilesContainingSameObject = new List<Tile>() { this };
        }

        /// <summary>
        /// <para>For a given <c>TileObjectData</c>, create a new GameObject at the center of the tile, facing the given
        /// Cardinal Direction. Will also handle finding other tiles covered by the GameObject and assign them the same object.</para>
        /// </summary>
        /// <param name="objData"><c>TileObjectData</c> to create a GameObject from.</param>
        /// <param name="objectFacingDirection">Object will be place with its forward direction facing this Cardinal Direction</param>
        /// <returns><c>true</c> if a new GameObject was added to the tile. <c>false</c> if a GameObject is already in the tile,
        /// there is a GameObject in one of the other tiles covered by the object, or the object extends out of bounds.</returns>
        public bool PlaceNewObject(TileObjectData objData, CardinalDirection objectFacingDirection)
        {
            bool objectPlacementIsInBounds = true;
            bool allCoveredTilesAreEmpty = true;
            List<Tile> tilesCoveredByObject = new();
            List<Vector2Int> spacesToOccupy = FaceVectorsAtCardinalDirection(objData.OccupiedSpaces, objectFacingDirection);
            
            foreach (Vector2Int tileToOccupy in spacesToOccupy)
            {
                Vector2Int relativeTileIndex = tileToOccupy + TileIndex;
                if (_tileMap.IsIndexInBounds(relativeTileIndex))
                    tilesCoveredByObject.Add(_tileMap._tiles[relativeTileIndex]);
                
                else
                    objectPlacementIsInBounds = false;
            }

            foreach (Tile coveredTile in tilesCoveredByObject)
                if (coveredTile._tileIsOccupied) allCoveredTilesAreEmpty = false;

            if (!(objectPlacementIsInBounds && allCoveredTilesAreEmpty)) return false;
            
            GameObject newObjectInstance = Instantiate(objData.PrefabReference, _tileMap.transform);
            newObjectInstance.transform.localRotation = objectFacingDirection.ToRotationFromNorth();
            newObjectInstance.transform.localPosition = LocalPosition;

            foreach (Tile coveredTile in tilesCoveredByObject)
            {
                coveredTile._objectData = objData;
                coveredTile._objectInstance = newObjectInstance;
                coveredTile._tileIsOccupied = true;
                coveredTile._tilesContainingSameObject = tilesCoveredByObject;
            }
            return true;
        }
        /// <summary>
        /// Dispose of any GameObject contained within the tile. Also handles removing 
        /// </summary>
        /// <param name="removedObjData">When this method returns, contains the <c>TileObjectData</c> of the GameObject
        /// that was removed from the tile. If no tile was removed, remains null.</param>
        /// <returns><c>true</c> if a GameObject was removed from the tile and other tiles covered by the object.
        /// <c>false</c> if the tile is already empty.</returns>
        public bool RemoveObject(out TileObjectData removedObjData)
        {
            removedObjData  = _tileIsOccupied ? _objectData : null;
            if (!_tileIsOccupied) return false;
            Destroy(_objectInstance);
            List<Tile> tilesToRemoveFrom = _tilesContainingSameObject;
            foreach (Tile coveredTile in tilesToRemoveFrom)
            {
                coveredTile._objectData = null;
                coveredTile._objectInstance = null;
                coveredTile._tileIsOccupied = false;
                coveredTile._tilesContainingSameObject = new() { coveredTile };
            }
            return true;
        }
        private List<Vector2Int> FaceVectorsAtCardinalDirection(List<Vector2Int> vectorsToTransform, CardinalDirection facingDirection)
        {
            List<Vector2Int> transformedVectors = new();
            foreach (Vector2Int vec in vectorsToTransform)
                transformedVectors.Add(vec.RotateDegrees(facingDirection.ToDegreesPastNorth()));
            return transformedVectors;
        }
    }
    
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

