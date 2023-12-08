using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class StaticTileable : Tileable
    {
        protected internal abstract void OnRegisteredToIndices(List<Vector2Int> ownedIndices);

        protected internal abstract void OnCleared(List<Vector2Int> clearedIndices);
    }
}
