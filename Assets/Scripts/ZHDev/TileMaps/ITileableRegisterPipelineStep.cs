using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZHDev.TileMaps
{

    public interface ITileableRegisterPipelineStep<T> where T : Tileable
    {
        public void Execute(TileableRegisterPipelineContext<T> context);

    }
}
