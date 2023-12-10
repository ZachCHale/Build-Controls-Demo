using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class ClearConflictingTileable<T> : IOverwriteTileableIndicesStrategy<T> where T : Tileable
    {
        public void HandleOverwrite(TileableManager<T> manager, Vector2Int conflictingIndex, T oldOwner, T newOwner)
        {
            manager.FreeIndicesOf(oldOwner);
        }
    }
}
