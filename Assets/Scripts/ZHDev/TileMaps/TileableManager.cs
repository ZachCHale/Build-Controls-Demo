using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class TileableManager
    {
        private readonly Dictionary<Vector2Int, Tileable> _indexToObjDict = new();
        private readonly Dictionary<Guid, List<Vector2Int>> _objGuidToIndices = new();
        
        internal void RegisterStaticTileable(StaticTileable sTileable, Vector2Int targetIndex)
        {
            if(_indexToObjDict.ContainsKey(targetIndex)) ((StaticTileable)_indexToObjDict[targetIndex]).Delete();
            _objGuidToIndices.Add(sTileable.Guid, new List<Vector2Int>() { targetIndex });
            AssignOwnerToIndex(sTileable, targetIndex);
            sTileable.OnRegisteredToAllIndices();
        }   
        
        internal void RegisterStaticTileable(StaticTileable sTileable, List<Vector2Int> targetIndices)
        {
            foreach (var i in targetIndices)
            {
                if(_indexToObjDict.ContainsKey(i)) ((StaticTileable)_indexToObjDict[i]).Delete();
            }
            _objGuidToIndices.Add(sTileable.Guid, new(targetIndices));
            foreach (var i in targetIndices)
                AssignOwnerToIndex(sTileable, i);
            sTileable.OnRegisteredToAllIndices();
        }

        internal void RemoveStaticTileable(StaticTileable sTileable)
        {
            if(!_objGuidToIndices.ContainsKey(sTileable.Guid))
                Debug.LogError($"Object with Guid \"{sTileable.Guid}\" does not exist in TileableManager");
            sTileable.OnDeleted();
            List<Vector2Int> indices = _objGuidToIndices[sTileable.Guid];
            foreach (var i in indices)
                _indexToObjDict.Remove(i);
            _objGuidToIndices.Remove(sTileable.Guid);
        }

        internal void RegisterDynamicTileable(DynamicTileable dTileable)
        {
            _objGuidToIndices.Add(dTileable.Guid, new());
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
        
        private bool TestDictionariesInSync()
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

    }
}
