using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class StaticTileable : Tileable
    {
        protected internal abstract void OnRegisteredToAllIndices();
        protected internal abstract void OnDeleted();

        protected StaticTileable(TileableManager tileableManager, List<Vector2Int> staticIndices) : base(
            tileableManager)
        {
            _manager.RegisterStaticTileable(this, staticIndices);
        }
        
        protected StaticTileable(TileableManager tileableManager, Vector2Int staticIndex) : base(
            tileableManager)
        {
            _manager.RegisterStaticTileable(this, staticIndex);
        }

        public void Delete()
        {
            _manager.RemoveStaticTileable(this);
        }
    }
}
