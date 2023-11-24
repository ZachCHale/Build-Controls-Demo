using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public partial class TileMap
    {
        private static float _gizmoClippingOffset = 0.001f;
        private static Vector3[] _gizmoQuadVertices = new Vector3 []
        {
            new(-0.5f,_gizmoClippingOffset,-0.5f), new (0.5f,_gizmoClippingOffset,-0.5f), new (0.5f,_gizmoClippingOffset,0.5f), new (-0.5f,_gizmoClippingOffset,0.5f),
        };
        private static Vector3[] _gizmoQuadNormals = new Vector3[]
        {
            Vector3.up, Vector3.up, Vector3.up, Vector3.up, 
        };
        private static int[] _gizmoQuadTriangles = new int[]
        {
            2,1,0,
            0,3,2
        };
        
        void OnDrawGizmos()
        {
            Mesh gizmoQuad = new Mesh
            {
                vertices = _gizmoQuadVertices,
                normals = _gizmoQuadNormals,
                triangles = _gizmoQuadTriangles
            };
            float offset = _tileSize / 2;
            Vector3 offsetPosition = new(offset, 0, offset);


            Color quadColorWhite = new Color(1, 1, 1, .3f);
            Color quadColorBlack = new Color(0, 0, 0, .3f);


            
            foreach (int x in Enumerable.Range(0, _tileCountX))
                foreach (int y in Enumerable.Range(0, _tileCountY))
                {
                    Gizmos.color = ((y + _tileCountY * x) + (x%2)*(1-(_tileCountY % 2))) %2 == 0 ? quadColorWhite : quadColorBlack;
                    Vector3 localTileCenter = offsetPosition + _tileSize * (Vector3.right * x + Vector3.forward * y);
                    Gizmos.DrawMesh(gizmoQuad, transform.TransformPoint(localTileCenter), transform.rotation, Vector3.one * _tileSize);
                }

            Vector3 pointA = transform.TransformPoint(Vector3.zero);
            Vector3 pointB = transform.TransformPoint(Vector3.right*(_tileCountX)*_tileSize);
            Vector3 pointC = transform.TransformPoint(Vector3.right*(_tileCountX)*_tileSize + Vector3.forward*(_tileCountY)*_tileSize);
            Vector3 pointD = transform.TransformPoint(Vector3.forward*(_tileCountY)*_tileSize);
            
            Gizmos.color = Color.red;
            
            Gizmos.DrawLineList(new Vector3[]{pointA, pointB, pointB, pointC, pointC, pointD, pointD, pointA});

        }
    }
}

