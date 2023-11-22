using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Demo/Tile Object Data")]
public class TileObjectData : ScriptableObject
{
    [SerializeField] private GameObject prefabReference;
    public GameObject PrefabReference => prefabReference;
    [SerializeField] private string title;
    public string Title => title;
    [SerializeField] private string description;
    public string Description => description;
    [SerializeField] private Texture2D menuPreviewImage;
    public Texture2D MenuPreviewImage => menuPreviewImage;

    [SerializeField] private List<Vector2Int> occupiedSpaces = new List<Vector2Int>(){Vector2Int.zero};
    public List<Vector2Int> OccupiedSpaces => occupiedSpaces;

}
