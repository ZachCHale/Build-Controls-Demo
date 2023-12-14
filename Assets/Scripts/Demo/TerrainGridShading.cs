using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using ZHDev.Extensions;
using ZHDev.TileableSystems;

public class TerrainGridShading : MonoBehaviour
{
    private Material _terrainMat;

    [SerializeField] private TileablePlane _tileablePlane;
    // Start is called before the first frame update
    void Awake()
    {
       _terrainMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.GetScreenPointIntersectionWithPlane(Input.mousePosition, _tileablePlane.Up,
            _tileablePlane.OriginPosition, out Vector3 pos);
        _terrainMat.SetVector("_Grid_radius_position", pos);
    }
}
