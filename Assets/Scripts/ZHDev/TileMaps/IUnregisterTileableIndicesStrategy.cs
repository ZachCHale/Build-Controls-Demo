using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public interface IUnregisterTileableIndicesStrategy<T> where T : Tileable
    {
        public void HandleUnregister(TileableManager<T> manager, Vector2Int targetIndex, T owner);
    }
}
