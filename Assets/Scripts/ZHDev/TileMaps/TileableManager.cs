using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class TileableManager : MonoBehaviour
    {
        private readonly float _gapSize = 1f;
        public float GapSize => _gapSize;
        
        public Vector3 OriginPosition => transform.position;
        public Vector3 LocalOriginPosition => Vector3.zero;
        public Vector3 Up => transform.up;
        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;
        
        private Dictionary<Vector2Int, Tileable> _indexToObjDict;
        private Dictionary<Guid, List<Vector2Int>> _objGuidToIndices;
        
        public T CreateStaticTileable<T>(Vector2Int targetIndex) where T : StaticTileable, new()
        {
            T newTileable = new T(){_manager = this};
            
            _objGuidToIndices.Add(newTileable.Guid, new List<Vector2Int>() { targetIndex });
            AssignOwnerToIndex(newTileable, targetIndex);
            newTileable.OnCreated();
            return newTileable;
        }
        
        public T CreateStaticTileable<T>(List<Vector2Int> targetIndices) where T : StaticTileable, new()
        {
            T newTileable = new T(){_manager = this};
            _objGuidToIndices.Add(newTileable.Guid, new(targetIndices));
            foreach (var i in targetIndices)
                AssignOwnerToIndex(newTileable, i);
            newTileable.OnCreated();
            return newTileable;
        }

        internal void RemoveStaticTileable(StaticTileable obj)
        {
            if(!_objGuidToIndices.ContainsKey(obj.Guid))
                Debug.LogError($"Object with Guid \"{obj.Guid}\" does not exist in TileableManager");
            obj.OnDeleted();
            List<Vector2Int> indices = _objGuidToIndices[obj.Guid];
            foreach (var i in indices)
                _indexToObjDict.Remove(i);
            _objGuidToIndices.Remove(obj.Guid);
        }

        public T CreateDynamicTileable<T>() where T : DynamicTileable, new()
        {
            return new T() { _manager = this };
        }

        internal void AddToDynamicTileable(DynamicTileable dTileable, Vector2Int newIndex)
        {
            AssignOwnerToIndex(dTileable, newIndex);
        }
        
        internal bool RemoveFromDynamicTileable(DynamicTileable dTileable, Vector2Int targetIndex)
        {
            if (!TryGetOwnerOfIndex(targetIndex, out Tileable actualOwner) ||
                actualOwner.Guid != dTileable.Guid) return false;
            _objGuidToIndices.Remove(dTileable.Guid);
            _indexToObjDict.Remove(targetIndex);
            return true;
        }

        public bool TryGetOwnerOfIndex(Vector2Int targetIndex, out Tileable outObj)
        {
            outObj = null;
            if (!_indexToObjDict.ContainsKey(targetIndex)) return false;
            outObj = _indexToObjDict[targetIndex];
            return true;
        }
        
        public List<Vector2Int> GetIndicesOwnedBy(Tileable targetObject)
        {
            List<Vector2Int> retrievedIndices = new();
            Guid objGuid = targetObject.Guid;
            if (_objGuidToIndices.ContainsKey(objGuid))
                retrievedIndices = new(_objGuidToIndices[objGuid]);
            return retrievedIndices;
        }

        private void AssignOwnerToIndex(Tileable owner, Vector2Int targetIndex)
        {
            if (!_indexToObjDict.TryAdd(targetIndex, owner))
                _indexToObjDict[targetIndex] = owner;
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
