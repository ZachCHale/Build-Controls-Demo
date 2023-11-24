using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.Extensions;

namespace ZHDev.TileMaps
{
    public partial class TileMap
    {
        private class Tile
        {
            private readonly TileMap _tileMap;
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
            /// <param name="objectFacingDirection">Object will be placed with its forward direction facing this Cardinal Direction</param>
            /// <returns><c>true</c> if a new GameObject was added to the tile. <c>false</c> if a GameObject is already in the tile,
            /// there is a GameObject in one of the other tiles covered by the object, or the object extends out of bounds.</returns>
            public bool PlaceNewObject(TileObjectData objData, CardinalDirection objectFacingDirection)
            {
                bool objectPlacementIsInBounds = true;
                bool allCoveredTilesAreEmpty = true;
                List<Tile> tilesCoveredByObject = new();
                List<Vector2Int> spacesToOccupy =
                    FaceVectorsAtCardinalDirection(objData.OccupiedSpaces, objectFacingDirection);

                foreach (Vector2Int tileToOccupy in spacesToOccupy)
                {
                    Vector2Int relativeTileIndex = tileToOccupy + TileIndex;
                    if (_tileMap.IsIndexInBounds(relativeTileIndex))
                        tilesCoveredByObject.Add(_tileMap._tiles[relativeTileIndex]);

                    else
                        objectPlacementIsInBounds = false;
                }

                foreach (Tile coveredTile in tilesCoveredByObject)
                    if (coveredTile._tileIsOccupied)
                        allCoveredTilesAreEmpty = false;

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
                removedObjData = _tileIsOccupied ? _objectData : null;
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

            private List<Vector2Int> FaceVectorsAtCardinalDirection(List<Vector2Int> vectorsToTransform,
                CardinalDirection facingDirection)
            {
                List<Vector2Int> transformedVectors = new();
                foreach (Vector2Int vec in vectorsToTransform)
                    transformedVectors.Add(vec.RotateDegrees(facingDirection.ToDegreesPastNorth()));
                return transformedVectors;
            }
        }
    }
}
