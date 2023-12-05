using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.Extensions;

namespace ZHDev.TileMaps
{

    public class Tile
    {
        private readonly TileMap _tileMap;
        public TileMap TileMap => _tileMap;

        public Vector3 WorldPosition => _tileMap.transform.TransformPoint(LocalPosition);
        public readonly Vector3 LocalPosition;
        public readonly Vector2Int TileIndex;
        private TileableObject _containedObject;
        internal TileableObject ContainedObject => _containedObject;
            
        /// <summary>
        /// <para>Creates a tile and calculates its local position based on the given <c>TileMap</c> and tile index.</para>
        /// </summary>
        /// <param name="tileMap">The <c>TileMap</c> containing this tile.</param>
        /// <param name="tileIndex">The index/key for this tile in the <c>TileMap</c></param>
        public Tile(TileMap tileMap, Vector2Int tileIndex)
        {
            _tileMap = tileMap;
            TileIndex = tileIndex;
            float tileCenterOffset = _tileMap.TileSize / 2;
            LocalPosition = new Vector3(tileCenterOffset, 0, tileCenterOffset) +
                            new Vector3(tileMap.TileSize * TileIndex.x, 0, tileMap.TileSize * tileIndex.y);
        }
            
        internal void AddToTile(TileableObject objToAdd)
        {
            if(_containedObject!=null && _containedObject != objToAdd)
                RemoveFromTile();
            _containedObject = objToAdd;
            objToAdd.OnAddedToTile(this);
        }

        internal void RemoveFromTile()
        {
            if(_containedObject != null) _containedObject.OnRemovedFromTile();
            _containedObject = null;
        }

        internal void RemoveFromTileUnhandled()
        {
            _containedObject = null;
        }
            
        internal void AddToTileUnhandled(TileableObject objToAdd)
        {
            if(_containedObject != null && _containedObject != objToAdd)
                RemoveFromTile();
            _containedObject = objToAdd;
        }

    }
    
}
