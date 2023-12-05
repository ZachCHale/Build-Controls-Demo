using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class TileableObject
    {
        private Guid _guid;
        internal Guid Guid => _guid;
        
        TileableObject()
        {
            _guid = System.Guid.NewGuid();
        }
        protected internal abstract void OnAddedToTile(Tile targetTile);
        protected internal abstract void OnRemovedFromTile();
        
        protected internal abstract bool IsValidToAddToTile(Tile targetTile);

    }
    
}

