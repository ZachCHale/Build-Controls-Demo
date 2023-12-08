using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class Tileable
    {
        //protected readonly TileableManager _manager;
        //public TileableManager Manager => _manager;
        //private List<Vector2Int> OwnedIndices => _manager.GetIndicesOwnedBy(this);
        private Guid _guid;
        public Guid Guid => _guid;

        internal Tileable()
        {
            _guid = Guid.NewGuid();
        }
        
        
        
    }
}
