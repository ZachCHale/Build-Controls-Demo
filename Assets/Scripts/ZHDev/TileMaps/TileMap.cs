using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public partial class TileMap : MonoBehaviour
    {
        private const int SIZE_MAX_LIMIT = 100;
        private const int SIZE_MIN_LIMIT = 1;
        
        [SerializeField] [Range(SIZE_MIN_LIMIT, SIZE_MAX_LIMIT)] private int _tileCountX = 10;
        [SerializeField] [Range(SIZE_MIN_LIMIT, SIZE_MAX_LIMIT)] private int _tileCountY = 10;
        
        private readonly float _tileSize = 1f;
        public float TileSize => _tileSize;
    
        private readonly Dictionary<Vector2Int, Tile>_tiles = new ();
        public Vector3 MapOriginWorld => transform.position;
        
        private Vector3 _mapEndWorld => transform.position + _tileSize * ((_mapXDir * _tileCountX) + (_mapYDir * _tileCountY));
        private Vector3 _mapOriginLocal => Vector3.zero;
        private Vector3 _mapEndLocal => new (_tileSize * _tileCountX, 0, _tileSize * _tileCountY);

        public Vector3 MapNormal => transform.up;
        private Vector3 _mapXDir => transform.right;
        private Vector3 _mapYDir => transform.forward;

    
        private void Awake()
        {
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
            Vector3 xProjection = Vector3.Project(worldPos - MapOriginWorld, _mapXDir);
            Vector3 yProjection = Vector3.Project(worldPos - MapOriginWorld,_mapYDir);

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
        
        public void AddAt(Vector2Int targetIndex, TileableObject objToAdd)
        {
            if (!IsIndexInBounds(targetIndex)) return;
            _tiles[targetIndex].AddToTile(objToAdd);
        }

        public void RemoveAt(Vector2Int targetIndex)
        {
            if (!IsIndexInBounds(targetIndex)) return;
            _tiles[targetIndex].RemoveFromTile();
        }

        public Tile GetTileAt(Vector2Int targetIndex)
        {
            return !IsIndexInBounds(targetIndex) ? null : _tiles[targetIndex];
        }

        public void ParentToMapTransform(Transform newChildTransform) => newChildTransform.parent = transform;

        private bool IsIndexInBounds(Vector2Int tileIndex) => tileIndex.x >= 0 && tileIndex.x < _tileCountX && tileIndex.y >= 0 && tileIndex.y < _tileCountY;

        private bool IsLocalPositionInBounds(Vector3 localPosition) => localPosition.x >= _mapOriginLocal.x && 
                                                                       localPosition.x <= _mapEndLocal.x && 
                                                                       localPosition.z >= _mapOriginLocal.z &&
                                                                       localPosition.z <=_mapEndLocal.z;   

    }
}

