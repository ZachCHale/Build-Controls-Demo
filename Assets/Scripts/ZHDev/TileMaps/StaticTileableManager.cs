using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace ZHDev.TileMaps
{
    //Overwrite Strategy: ClearConflicting : TransferConflictingIndices
    //    GetOwnerOfIndex, FreeIndicesOf, 
    //Register Strategy: ClearPrevious : AddToCurrentIndices
    //Unregister Strategy: ClearAll : RemoveSpecifiedIndices
    
    public class StaticTileableManager<T> : TileableManager<T> where T : StaticTileable
    {
        public override void RegisterAsOwner(Vector2Int targetIndex, T owner)
        {
            FreeIndex(targetIndex);
            FreeIndicesOf(owner);
            _objGuidToIndices.Add(owner.Guid, new List<Vector2Int>() { targetIndex });
            AssignOwnerToIndex(owner, targetIndex);
            owner.OnRegisteredToIndices(new(){targetIndex});
        }

        public override void RegisterAsOwner(List<Vector2Int> targetIndices, T owner)
        {
            FreeIndices(targetIndices);
            FreeIndicesOf(owner);
            _objGuidToIndices.Add(owner.Guid, new(targetIndices));
            foreach (var i in targetIndices)
                AssignOwnerToIndex(owner, i);
            owner.OnRegisteredToIndices(targetIndices);
        }

        public override bool FreeIndex(Vector2Int targetIndex)
        {
            if (_indexToObjDict.TryGetValue(targetIndex, out var owner))
            {
                List<Vector2Int> freedIndices = new(_objGuidToIndices[owner.Guid]);
                FreeIndicesOf(owner);
                owner.OnCleared(freedIndices);
                return true;
            }

            return false;
        }
    }
}*/
