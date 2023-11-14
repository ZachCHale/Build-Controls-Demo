using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Demo/Placeable Object Data")]
public class PlaceableObjData : ScriptableObject
{
    [SerializeField] private GameObject prefabReference;
    public GameObject PrefabReference => prefabReference;
    [SerializeField] private string label;
    public string Label => label;
    [SerializeField] private string description;
    public string Description => description;
}
