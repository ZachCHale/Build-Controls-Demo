using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class TileableRegisterPipelineContext<T> where T : Tileable
    {
        private readonly TileableManager<T> _manager;
        public TileableManager<T> Manager => _manager;
        private readonly List<Vector2Int> _indices;
        public List<Vector2Int> Indices => _indices;
        private readonly T _subjectTileable;
        public T SubjectTileable => _subjectTileable;
        public bool exitPipeline= false;
        
        TileableRegisterPipelineContext(TileableManager<T> manager, List<Vector2Int> indices, T subjectTileable )
        {
            _manager = manager;
            _indices = indices;
            _subjectTileable = subjectTileable;
        }
    }
}
