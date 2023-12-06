using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class DynamicTileable : Tileable
    {
        protected internal abstract void OnIndexAdded(Vector2Int addedIndex);
        protected internal abstract void OnIndexRemoved(Vector2Int removedIndex);

        protected DynamicTileable(TileableManager tileableManager) : base(tileableManager)
        {
            tileableManager.RegisterDynamicTileable(this);
        }

        public void AddIndex(Vector2Int targetIndex)
        {
            _manager.AddToDynamicTileable(this, targetIndex);
        }

        public void RemoveIndex(Vector2Int targetIndex)
        { 
            _manager.RemoveFromDynamicTileable(this, targetIndex);
        }
    }
}
