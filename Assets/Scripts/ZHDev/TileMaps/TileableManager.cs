using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class TileableManager<T> where T : Tileable
    {
        protected readonly Dictionary<Vector2Int, T> _indexToObjDict = new();
        protected readonly Dictionary<Guid, List<Vector2Int>> _objGuidToIndices = new();

        public abstract void RegisterAsOwner(Vector2Int targetIndex, T owner);

        public abstract void RegisterAsOwner(List<Vector2Int> targetIndices, T owner);
        
        private Guid _guid;
        public Guid Guid => _guid;

        internal TileableManager()
        {
            _guid = Guid.NewGuid();
        }

        public void FreeIndicesOf(T targetTileable)
        {
            if(!_objGuidToIndices.ContainsKey(targetTileable.Guid))return;
            foreach (var i in _objGuidToIndices[targetTileable.Guid]) 
                _indexToObjDict.Remove(i);
            _objGuidToIndices.Remove(targetTileable.Guid);
        }

        public abstract bool FreeIndex(Vector2Int targetIndex);

        public void FreeIndices(List<Vector2Int> targetIndices)
        {
            foreach (var i in targetIndices)
                FreeIndex(i);
        }
        
        public bool TryGetAt(Vector2Int targetIndex, out T outObj)
        {
            outObj = null;
            if (!_indexToObjDict.ContainsKey(targetIndex)) return false;
            outObj = _indexToObjDict[targetIndex];
            return true;
        }
        
        public List<Vector2Int> GetRegisteredIndices(T targetObject)
        {
            List<Vector2Int> retrievedIndices = new();
            Guid objGuid = targetObject.Guid;
            if (_objGuidToIndices.ContainsKey(objGuid))
                retrievedIndices = new(_objGuidToIndices[objGuid]);
            return retrievedIndices;
        }

        protected void AssignOwnerToIndex(T owner, Vector2Int targetIndex)
        {
            if (!_indexToObjDict.TryAdd(targetIndex, owner))
                _indexToObjDict[targetIndex] = owner;
        }

        public bool IsOwnerOf(Vector2Int targetIndex, T suspectedOwner) => (_indexToObjDict.TryGetValue(targetIndex, out T owner) && owner.Guid == suspectedOwner.Guid);
        
        protected bool TestDictionariesInSync()
        {
            foreach (var o in _objGuidToIndices.Keys)
            {
                foreach (var i in _objGuidToIndices[o])
                {
                    if (o != _indexToObjDict[i].Guid)
                    {
                        Debug.LogError(
                            $"TileableManager Out Of Sync!" +
                            $"\n{o}    ->    {i}    ->    {_indexToObjDict[i].Guid}");
                        return false;
                    }
                }
            }

            return true;
        }

        protected void Log()
        {
            foreach (var o in _objGuidToIndices.Keys)
            { 
                Debug.Log(o);
                foreach (var i in _objGuidToIndices[o])
                {
                    Debug.Log($"   {i}");
                }
            }
        }

    }
}
