using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class DynamicTileableManager<T> : TileableManager<T> where T : DynamicTileable
    {
        //When adding, any dynamic conflicting tileables will be painted over, leaving unconflicting indices under their control
        //while transfering ownership of the conflicting ones. 
        public override void RegisterAsOwner(Vector2Int targetIndex, T owner)
        {
            if (IsOwnerOf(targetIndex, owner)) return;
            FreeIndex(targetIndex);
            _indexToObjDict.Add(targetIndex, owner);
            if(!_objGuidToIndices.ContainsKey(owner.Guid)) _objGuidToIndices.Add(owner.Guid, new());
            _objGuidToIndices[owner.Guid].Add(targetIndex);
        }

        public override void RegisterAsOwner(List<Vector2Int> targetIndices, T owner)
        {
            if(!_objGuidToIndices.ContainsKey(owner.Guid)) _objGuidToIndices.Add(owner.Guid, new());
            foreach (var i in targetIndices)
            {
                if(IsOwnerOf(i,owner)) continue;
                FreeIndex(i);
                _indexToObjDict.Add(i, owner);
                _objGuidToIndices[owner.Guid].Add(i);
            }
        }

        public override bool FreeIndex(Vector2Int targetIndex)
        {
            if (_indexToObjDict.TryGetValue(targetIndex, out T owner))
            {
                _objGuidToIndices[owner.Guid].Remove(targetIndex);
                _indexToObjDict.Remove(targetIndex);
                return true;
            }
            return false;
        }
    }
}
