using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Client.Gui;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class TileableManager<T> where T : class
    {
        protected readonly Dictionary<Vector2Int, T> _indexToObjDict = new();
        protected readonly Dictionary<T, List<Vector2Int>> _objGuidToIndices = new();
        
        
        public void RegisterAsOwner(Vector2Int targetIndex, T owner)
        {
            _objGuidToIndices.TryAdd(owner, new List<Vector2Int>());

            _objGuidToIndices[owner].Add(targetIndex);
            _indexToObjDict[targetIndex] = owner;

        }

        public void RegisterAsOwner(List<Vector2Int> targetIndices, T owner)
        {
            _objGuidToIndices.TryAdd(owner, new List<Vector2Int>());

            foreach (var i in targetIndices)
            {
                _objGuidToIndices[owner].Add(i);
                _indexToObjDict[i] = owner;
            }
        }

        public bool FreeIndex(Vector2Int targetIndex)
        {
            bool foundSomething = TryGetAt(targetIndex, out T targetTileable);
            if (!foundSomething) return false;
            _indexToObjDict.Remove(targetIndex);
            _objGuidToIndices[targetTileable].Remove(targetIndex);
            if (_objGuidToIndices[targetTileable].Count == 0)
                _objGuidToIndices.Remove(targetTileable);
            return true;
        }
        

        public void FreeIndicesOf(T targetTileable)
        {
            if(!_objGuidToIndices.ContainsKey(targetTileable))return;
            foreach (var i in _objGuidToIndices[targetTileable]) 
                _indexToObjDict.Remove(i);
            _objGuidToIndices.Remove(targetTileable);
            
        }

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
            
            if (_objGuidToIndices.TryGetValue(targetObject, out var index))
                retrievedIndices = new(index);
            return retrievedIndices;
        }

        public bool IsOwnerOf(Vector2Int targetIndex, T suspectedOwner) => (_indexToObjDict.TryGetValue(targetIndex, out T owner) && owner == suspectedOwner);

        public List<T> GetOwners(List<Vector2Int> indices)
        {
            HashSet<T> tileables = new();
            foreach (var i in indices)
                tileables.Add(_indexToObjDict[i]);
            return new List<T>(tileables);
        }

        public bool IsFree(Vector2Int index)
        {
            return !_indexToObjDict.ContainsKey(index);
        }

        public bool IsFree(List<Vector2Int> indices)
        {
            foreach (var i in indices)
                if (!IsFree(i))
                    return false;
            return true;
        }
        
        protected bool TestDictionariesInSync()
        {
            foreach (var o in _objGuidToIndices.Keys)
            {
                foreach (var i in _objGuidToIndices[o])
                {
                    if (o != _indexToObjDict[i])
                    {
                        Debug.LogError(
                            $"TileableManager Out Of Sync!" +
                            $"\n{o}    ->    {i}    ->    {_indexToObjDict[i]}");
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
