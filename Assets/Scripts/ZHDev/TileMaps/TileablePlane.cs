using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZHDev.TileMaps
{
    public class TileablePlane : MonoBehaviour
    {
        [SerializeField] private float _gapSize;
        public float GapSize => _gapSize;
        public Vector3 OriginPosition =>transform.position;
        public Vector3 LocalOriginPosition => Vector3.zero;
        public Vector3 Up => transform.up;
        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;
        
        
        public Vector2Int WorldToProjectedIndex(Vector3 worldPos)
        {
            Vector3 localPos = WorldToLocal(worldPos);

            int xIndex = (int)(localPos.x / _gapSize);
            int yIndex = (int)(localPos.z / _gapSize);
            xIndex = localPos.x < 0 ? xIndex - 1 : xIndex;
            yIndex = localPos.y < 0 ? yIndex - 1 : yIndex;
            
            return new Vector2Int(xIndex, yIndex);
        }

        public Vector3 WorldToLocal(Vector3 pos) => transform.InverseTransformPoint(pos);
        public Vector3 IndexToWorld(Vector2Int index) => transform.TransformPoint(IndexToLocal(index));
        public Vector3 IndexToLocal(Vector2Int index) => Vector3.zero + GapSize * ((Vector3.right * index.x) + (Vector3.forward * index.y) + (new Vector3(0.5f, 0, 0.5f)));
        public Vector3 LocalToWorld(Vector3 localPos) => transform.TransformPoint(localPos);
        
        
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
        
        /*void OnDrawGizmos()
        {
            Mesh gizmoQuad = new Mesh
            {
                vertices = _gizmoQuadVertices,
                normals = _gizmoQuadNormals,
                triangles = _gizmoQuadTriangles
            };
            float offset = _gapSize / 2;
            Vector3 offsetPosition = new(offset, 0, offset);
            
            Color quadColorWhite = new Color(1, 1, 1, .3f);
            Color quadColorBlack = new Color(0, 0, 0, .3f);
            int renderDistance = 10;
            
            foreach (int x in Enumerable.Range(-renderDistance, renderDistance*2))
                foreach (int y in Enumerable.Range(-renderDistance, renderDistance*2))
                {
                    Gizmos.color = ((y + renderDistance * x) + (x%2)*(1-(renderDistance % 2))) %2 == 0 ? quadColorWhite : quadColorBlack;
                    Vector3 localTileCenter = offsetPosition + _gapSize * (Vector3.right * x + Vector3.forward * y);
                    Gizmos.DrawMesh(gizmoQuad, transform.TransformPoint(localTileCenter), transform.rotation, _gapSize * transform.lossyScale);
                }

            Vector3 pointA = transform.TransformPoint((Vector3.right*(-renderDistance)*_gapSize + Vector3.forward*(-renderDistance)*_gapSize));
            Vector3 pointB = transform.TransformPoint(Vector3.right*(renderDistance)*_gapSize  + Vector3.forward*(-renderDistance)*_gapSize);
            Vector3 pointC = transform.TransformPoint(Vector3.right*(renderDistance)*_gapSize + Vector3.forward*(renderDistance)*_gapSize);
            Vector3 pointD = transform.TransformPoint((Vector3.right*(-renderDistance)*_gapSize + Vector3.forward*(renderDistance)*_gapSize));
            
            Gizmos.color = Color.red;
            
            Gizmos.DrawLineList(new Vector3[]{pointA, pointB, pointB, pointC, pointC, pointD, pointD, pointA});

        }*/
    }
}
