using UnityEngine;
using Object = UnityEngine.Object;

public class PlaceableObjInstance
{
    public PlaceableObjData Data { get; }
    public GameObject Instance { get; }

    public PlaceableObjInstance(PlaceableObjData data, Vector3 position, Quaternion rotation)
    {
        Data = data;
        Instance = Object.Instantiate(data.PrefabReference, position, rotation);
    }

    public PlaceableObjInstance(PlaceableObjData data, Vector3 position) : this(data, position, Quaternion.identity) {}

    public PlaceableObjInstance(PlaceableObjData data) : this(data, Vector3.zero, Quaternion.identity) {}

    public void Dispose()
    {
        Object.Destroy(Instance);
    }
}
