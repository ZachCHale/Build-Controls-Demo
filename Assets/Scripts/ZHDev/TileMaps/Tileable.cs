using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class Tileable : EqualityComparer<Tileable>
    {
        private Guid _guid;
        public Guid Guid => _guid;

        protected internal Tileable()
        {
            _guid = Guid.NewGuid();
        }
        
        
    }
}
