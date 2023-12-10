using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class TileableIndicesSelection<T> : IEnumerable<Vector2Int> where T : Tileable
    {
        private readonly TileableManager<T> _manager;
        public TileableManager<T> Manager => _manager;
        private readonly List<Vector2Int> _indices;
        public List<Vector2Int> Indices => new(_indices);
        public List<T> Tileables => _manager.GetOwners(_indices);

        public TileableIndicesSelection(TileableManager<T> manager, List<Vector2Int> indices)
        {
            _manager = manager;
            _indices = new(indices);
        }
        
        public IEnumerator<Vector2Int> GetEnumerator()
        {
            return _indices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
