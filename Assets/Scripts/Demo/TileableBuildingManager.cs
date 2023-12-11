using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.TileMaps;

namespace Demo
{
    public class TileableBuildingManager : MonoBehaviour
    {
        [SerializeField] private TileablePlane _tileablePlane;
        public TileablePlane TileablePlane => _tileablePlane;
        
        private TileableManager<Building> _tileableManager = new();
        
        public TileableManager<Building> TileableManager => _tileableManager;

        public bool TryPlaceNewBuilding(TileObjectData buildingData, Vector3 positionToAdd, CardinalDirection facingDirection, out Building resultBuilding)
        {
            Vector2Int closestIndex = _tileablePlane.WorldToProjectedIndex(positionToAdd);
            List<Vector2Int> targetIndices = buildingData.GetTransformIndices(closestIndex, facingDirection);
            resultBuilding = null;
            
            if (!_tileableManager.IsFree(targetIndices)) return false;

            GameObject buildingGameObject = Instantiate(original: buildingData.PrefabReference, parent: _tileablePlane.transform);
            buildingGameObject.transform.localPosition = _tileablePlane.IndexToLocal(closestIndex);
            buildingGameObject.transform.rotation = facingDirection.ToRotationFromNorth();
            
            resultBuilding = new Building(this, buildingData, buildingGameObject);

            _tileableManager.RegisterAsOwner(targetIndices, resultBuilding);
            return true;
        }

        public bool TryRemoveBuildingAt(Vector3 positionToRemove)
        {
            Vector2Int closestIndex = _tileablePlane.WorldToProjectedIndex(positionToRemove);
            if (!_tileableManager.TryGetAt(closestIndex, out Building foundBuilding)) return false;
            _tileableManager.FreeIndicesOf(foundBuilding);
            Destroy(foundBuilding.GameObjectInstance);
            return true;
        }

        public List<Vector2Int> GetObstructedIndices(List<Vector2Int> targetIndices)
        {
            List<Vector2Int> obstructed = new();
            foreach (var i in targetIndices)
            {
                if (_tileableManager.IsFree(i)) continue;
                obstructed.Add(i);
            }

            return obstructed;
        }
        public List<Vector2Int> GetUnobstructedIndices(List<Vector2Int> targetIndices)
        {
            List<Vector2Int> unobstructed = new();
            foreach (var i in targetIndices)
            {
                if (!_tileableManager.IsFree(i)) continue;
                unobstructed.Add(i);
            }

            return unobstructed;
        }

        public bool TryGetBuildingAt(Vector2Int targetIndex, out Building foundBuilding) =>
            _tileableManager.TryGetAt(targetIndex, out foundBuilding);

        public List<Vector2Int> GetIndicesOfBuilding(Building targetBuilding) =>
            _tileableManager.GetRegisteredIndices(targetBuilding);

    }
}
