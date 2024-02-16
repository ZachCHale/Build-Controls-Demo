using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.Extensions;

namespace Demo
{
    [CreateAssetMenu(menuName = "Demo/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [SerializeField] private GameObject prefabReference;
        public GameObject PrefabReference => prefabReference;
        [SerializeField] private Texture2D menuPreviewImage;
        public Texture2D MenuPreviewImage => menuPreviewImage;

        [SerializeField] private List<Vector2Int> occupiedSpaces = new List<Vector2Int>(){Vector2Int.zero};
        public List<Vector2Int> OccupiedSpaces => occupiedSpaces;
        
        [SerializeField] private List<Vector2Int> connectionSpaces = new List<Vector2Int>();
        public List<Vector2Int> ConnectionSpaces => connectionSpaces;
        
        [SerializeField] private BuildingConnectionData connectionData;
        public BuildingConnectionData ConnectionData => connectionData;

        public List<Vector2Int> GetTransformIndices(Vector2Int pivotIndex, CardinalDirection facingDirection)
        {
            List<Vector2Int> transformedVectors = new();
            foreach (Vector2Int vec in OccupiedSpaces)
                transformedVectors.Add(vec.RotateDegrees(facingDirection.ToDegreesPastNorth()) + pivotIndex);
            return transformedVectors;
        }
        
        public List<Vector2Int> GetTransformConnectionIndices(Vector2Int pivotIndex, CardinalDirection facingDirection)
        {
            List<Vector2Int> transformedVectors = new();
            foreach (Vector2Int vec in ConnectionSpaces)
                transformedVectors.Add(vec.RotateDegrees(facingDirection.ToDegreesPastNorth()) + pivotIndex);
            return transformedVectors;
        }
        
        

    }
}
