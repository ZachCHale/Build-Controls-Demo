using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public interface IPreregisterTileableIndicesStrategy<T> where T : Tileable
    {
        public void HandlePreregister(TileableManager<T> manager, T newOwner);
    }
}
