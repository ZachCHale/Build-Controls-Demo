using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class DynamicTileable : Tileable
    {
        protected internal abstract void OnRegisteredIndex(Vector2Int newIndex);
        protected internal abstract void OnFreeIndex(Vector2Int freedIndex);
    }
}
