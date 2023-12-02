using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public abstract class TileableObject
    {
        protected internal abstract void OnAddedToTile(Tile targetTile);
        protected internal abstract void OnRemovedFromTile();
    }
}

