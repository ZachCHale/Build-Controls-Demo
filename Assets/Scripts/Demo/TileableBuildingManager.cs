using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;
using ZHDev.TileableSystems;

namespace Demo
{
    public class TileableBuildingManager : MonoBehaviour
    {
        [SerializeField] private TileablePlane _tileablePlane;
        public TileablePlane TileablePlane => _tileablePlane;
        
        private TileableRegistry<Building> _tileableRegistry = new();
        
        public TileableRegistry<Building> TileableRegistry => _tileableRegistry;

        public TileableRegistry<BuildingConnection> _connectionRegistry = new();

        public bool TryPlaceNewBuilding(BuildingData buildingData, Vector3 positionToAdd, CardinalDirection facingDirection, out Building resultBuilding)
        {
            Vector2Int closestIndex = _tileablePlane.WorldToProjectedIndex(positionToAdd);
            List<Vector2Int> targetIndices = buildingData.GetTransformIndices(closestIndex, facingDirection);
            resultBuilding = null;
            
            if (!_tileableRegistry.IsFree(targetIndices)) return false;

            GameObject buildingGameObject = Instantiate(original: buildingData.PrefabReference, parent: _tileablePlane.transform);
            buildingGameObject.transform.localPosition = _tileablePlane.IndexToLocal(closestIndex);
            buildingGameObject.transform.rotation = facingDirection.ToRotationFromNorth();
            
            resultBuilding = new Building(this, buildingData, buildingGameObject);

            _tileableRegistry.RegisterAsOwner(targetIndices, resultBuilding);


            if (buildingData.ConnectionData != null)
            {
                List<Vector2Int> connectionIndices = buildingData.GetTransformConnectionIndices(closestIndex, facingDirection);

                HashSet<Vector2Int> indicesToRebuildConnections = new();
                foreach (var index in connectionIndices)
                {
                    indicesToRebuildConnections.Add(index);
                    GameObject newConnectionInstance = new GameObject("Building Connection");
                    newConnectionInstance.transform.parent = _tileablePlane.transform;
                    newConnectionInstance.transform.localPosition = _tileablePlane.IndexToLocal(index);
                    newConnectionInstance.transform.rotation = Quaternion.identity;
                    newConnectionInstance.transform.parent = resultBuilding.GameObjectInstance.transform;
                    _connectionRegistry.RegisterAsOwner(index, new BuildingConnection(this, buildingData.ConnectionData, newConnectionInstance));
                    List<Vector2Int> adjacentIndices = new()
                    {
                        index + Vector2Int.left, index + Vector2Int.right, index + Vector2Int.up,
                        index + Vector2Int.down
                    };
                    foreach (var adjI in adjacentIndices)
                    {
                        if (_connectionRegistry.IsFree(adjI)) continue;
                        indicesToRebuildConnections.Add(adjI);
                    }
                }

                foreach (var i in indicesToRebuildConnections)
                {
                    RebuildConnection(i);
                }
            }
            return true;
        }

        public bool TryRemoveBuildingAt(Vector3 positionToRemove)
        {
            Vector2Int closestIndex = _tileablePlane.WorldToProjectedIndex(positionToRemove);
            if (!_tileableRegistry.TryGetAt(closestIndex, out Building foundBuilding)) return false;
            HashSet<Vector2Int> removedIndices = new(_tileableRegistry.GetRegisteredIndices(foundBuilding));
            _tileableRegistry.FreeIndicesOf(foundBuilding);
            Destroy(foundBuilding.GameObjectInstance);


            if (foundBuilding.ResourceData.ConnectionData != null)
            {
                HashSet<Vector2Int> indicesToRebuildConnections = new(_tileableRegistry.GetRegisteredIndices(foundBuilding));

                foreach (var index in removedIndices)
                {
                    bool isConnection = _connectionRegistry.TryGetAt(index, out BuildingConnection removedConnection);
                    if(!isConnection)continue;
                    _connectionRegistry.FreeIndex(index);
                    //Destroy(removedConnection.GameObjectInstance);
                    List<Vector2Int> adjacentIndices = new()
                    {
                        index + Vector2Int.left, index + Vector2Int.right, index + Vector2Int.up,
                        index + Vector2Int.down
                    };
                    foreach (var adjI in adjacentIndices)
                    {
                        if (removedIndices.Contains(adjI) || _connectionRegistry.IsFree(adjI)) continue;
                        indicesToRebuildConnections.Add(adjI);
                    }
                }
                foreach (var i in indicesToRebuildConnections)
                {
                    RebuildConnection(i);
                }
            }
            
            return true;
        }

        public List<Vector2Int> GetObstructedIndices(List<Vector2Int> targetIndices)
        {
            List<Vector2Int> obstructed = new();
            foreach (var i in targetIndices)
            {
                if (_tileableRegistry.IsFree(i)) continue;
                obstructed.Add(i);
            }

            return obstructed;
        }
        public List<Vector2Int> GetUnobstructedIndices(List<Vector2Int> targetIndices)
        {
            List<Vector2Int> unobstructed = new();
            foreach (var i in targetIndices)
            {
                if (!_tileableRegistry.IsFree(i)) continue;
                unobstructed.Add(i);
            }

            return unobstructed;
        }

        public bool TryGetBuildingAt(Vector2Int targetIndex, out Building foundBuilding) =>
            _tileableRegistry.TryGetAt(targetIndex, out foundBuilding);

        public List<Vector2Int> GetIndicesOfBuilding(Building targetBuilding) =>
            _tileableRegistry.GetRegisteredIndices(targetBuilding);

        private void RebuildConnection(Vector2Int targetIndex)
        {
            BuildingConnection buildingConnection;
            if (!_connectionRegistry.TryGetAt(targetIndex, out buildingConnection)) return;
            
            
            GameObject rootObject = buildingConnection.GameObjectInstance;
            BuildingConnectionData connectionData = buildingConnection.BuildingConnectionData;
            
            
            Vector2Int northIndex = targetIndex + CardinalDirection.North.ToVector2Int();
            Vector2Int eastIndex = targetIndex + CardinalDirection.East.ToVector2Int();
            Vector2Int southIndex = targetIndex + CardinalDirection.South.ToVector2Int();
            Vector2Int westIndex = targetIndex + CardinalDirection.West.ToVector2Int();

            bool isNorthConnected = !_connectionRegistry.IsFree(northIndex);
            bool isEastConnected = !_connectionRegistry.IsFree(eastIndex);
            bool isSouthConnected = !_connectionRegistry.IsFree(southIndex);
            bool isWestConnected = !_connectionRegistry.IsFree(westIndex);

            bool isJointed = (!(isNorthConnected && isSouthConnected && !(isEastConnected || isWestConnected)) && 
                                !(isEastConnected && isWestConnected && !(isSouthConnected || isNorthConnected)));
            
            
            if(!isNorthConnected) RemoveConnectionDirection(CardinalDirection.North);
            else CreateConnectionDirection(CardinalDirection.North);
            if(!isEastConnected) RemoveConnectionDirection(CardinalDirection.East);
            else CreateConnectionDirection(CardinalDirection.East);
            if(!isSouthConnected) RemoveConnectionDirection(CardinalDirection.South);
            else CreateConnectionDirection(CardinalDirection.South);
            if(!isWestConnected) RemoveConnectionDirection(CardinalDirection.West);
            else CreateConnectionDirection(CardinalDirection.West);

            if (!isJointed)
            {
                GameObject toDelete = buildingConnection.GetJointInstance();
                if (toDelete != null)
                    Object.Destroy(toDelete);
            }
            else if (buildingConnection.GetJointInstance() == null) 
            {
                GameObject newJoint = Object.Instantiate(original: connectionData.JointReference, parent: rootObject.transform);
                newJoint.transform.localPosition = Vector3.zero;
                newJoint.transform.rotation = Quaternion.identity;
                buildingConnection.SetJointInstance(newJoint);
            }

            void RemoveConnectionDirection(CardinalDirection dir)
            {
                GameObject toDelete = buildingConnection.GetConnectionInstanceByDirection(dir);
                if (toDelete != null)
                    Object.Destroy(toDelete);
            }

            void CreateConnectionDirection(CardinalDirection dir)
            {
                if (buildingConnection.GetConnectionInstanceByDirection(dir) != null) return;
                GameObject newConnection = Object.Instantiate(original: connectionData.GetConnectionByDirection(dir), parent: rootObject.transform);
                newConnection.transform.localPosition = Vector3.zero;
                newConnection.transform.rotation = Quaternion.identity;
                buildingConnection.SetConnectionInstanceByDirection(dir, newConnection);
            }
        }

    }
}
