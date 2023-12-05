using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class StaticTileable : Tileable
    {
        protected internal abstract void OnCreated();
        protected internal abstract void OnDeleted();

        public void Delete()
        {
            _manager.RemoveStaticTileable(this);
        }
    }
}
