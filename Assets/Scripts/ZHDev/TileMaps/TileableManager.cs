using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class TileableManager : MonoBehaviour
    {
        private readonly Dictionary<Vector2Int, Tile>_tiles = new ();
        private readonly Dictionary<Guid, List<Tile>> _tileableToTilesDictionary = new();
        
        private readonly float _gapSize = 1f;
        public float GapSize => _gapSize;
        
        public Vector3 OriginPosition => transform.position;
        public Vector3 LocalOriginPosition => Vector3.zero;
        public Vector3 Up => transform.up;
        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;

        
        //might be able to use strategy pattern later, mostly for custom, strategies
        public enum OverwriteStrategy
        {
            DeleteConflictingObject,
            TransferConflictingIndicesToThisObject,
            //Cancel
            //ExcludeConflictingIndices
            //Custom, might be dependent on state of tileables in indices or something
        }
        
        private Dictionary<Vector2Int, TileableObject> _indexToObjDict;
        private Dictionary<Guid, List<Vector2Int>> _objGuidToIndices;
        
        public T CreateTileable<T>(Vector2Int targetIndex) where T : TileableObject, new()
        {
            T newTileable = new T();
            _objGuidToIndices.Add(newTileable.Guid, new() { targetIndex });
            AssignOwnerToIndex(targetIndex, newTileable);
            return newTileable;
        }
        public T CreateTileable<T>(List<Vector2Int> targetIndices) where T : TileableObject, new()
        {
            T newTileable = new T();
            _objGuidToIndices.Add(newTileable.Guid, new(targetIndices));
            foreach (var i in targetIndices)
                AssignOwnerToIndex(i, newTileable);
            return newTileable;
        }

        public void RemoveTileable(TileableObject obj)
        {
            if(!_objGuidToIndices.ContainsKey(obj.Guid))
                Debug.LogError($"Object with Guid \"{obj.Guid}\" does not exist in TileableManager");
            List<Vector2Int> indices = _objGuidToIndices[obj.Guid];
            foreach (var i in indices)
                _indexToObjDict.Remove(i);
            _objGuidToIndices.Remove(obj.Guid);
        }

        public bool TryGetTileableAt(Vector2Int targetIndex, out TileableObject outObj)
        {
            outObj = null;
            if (!_indexToObjDict.ContainsKey(targetIndex)) return false;
            outObj = _indexToObjDict[targetIndex];
            return true;
        }

        private void AssignOwnerToIndex(Vector2Int index, TileableObject ownerObject)
        {
            if (!_indexToObjDict.TryAdd(index, ownerObject))
                _indexToObjDict[index] = ownerObject;
        }
        
        
        public Vector2Int WorldToProjectedIndex(Vector3 worldPos)
        {
            Vector3 localPos = WorldToLocal(worldPos);

            int xIndex = (int)(localPos.x / _gapSize);
            int yIndex = (int)(localPos.z / _gapSize);
            xIndex = localPos.x < 0 ? xIndex - 1 : xIndex;
            yIndex = localPos.y < 0 ? yIndex - 1 : yIndex;
            
            return new Vector2Int(xIndex, yIndex);
        }

        public Vector3 WorldToLocal(Vector3 pos) => transform.InverseTransformPoint(pos);
        public Vector3 IndexToWorld(Vector2Int index) => transform.TransformPoint(IndexToLocal(index));
        public Vector3 IndexToLocal(Vector2Int index) => Vector3.zero + (Vector3.right * GapSize * index.x) + (Vector3.forward * GapSize * index.y);
        public Vector3 LocalToWorld(Vector3 localPos) => transform.TransformPoint(localPos);

        

    }
}
