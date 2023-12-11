using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.TileMaps;

namespace Demo
{
    public class Highlighter
    {
        private const string PREFAB_PATH = "Demo/Highlighter/HighlightPrefab";
        private const string MAT_PATH = "Demo/Highlighter/HighlightMat";

        
        
        private GameObject _highlightPrefab;
        private Material _baseHighlightMaterial;
        
        private TileablePlane _tileablePlane;
        private TileableManager<HighlightColor> _highlightManager;

        private Dictionary<Vector2Int, GameObject> _currentHighlights = new ();
        private Dictionary<Color, HighlightColor> _colorsInUse = new ();

        private class HighlightColor
        {
            private Color _color;
            public Color Color => _color;
            private Material _material;
            public Material Material => _material;

            public HighlightColor(Color color, Material material)
            {
                _color = color;
                _material = material;
            }
        }

        
        //Keep a seperate dictionary to keep compare and decide which highlights to remove.
        
        public Highlighter(TileablePlane plane)
        {
            _tileablePlane = plane;
            _highlightManager = new();
            _highlightPrefab = Resources.Load<GameObject>(PREFAB_PATH);
            _baseHighlightMaterial = Resources.Load<Material>(MAT_PATH);
        }

        public void SetHighlights(List<Vector2Int> indices, Color highlightColor)
        {
            if (!_colorsInUse.ContainsKey(highlightColor))
            {
                Material newMat = new Material(_baseHighlightMaterial);
                newMat.color = highlightColor;
                HighlightColor newColor = new HighlightColor(highlightColor, newMat);
                _colorsInUse.Add(highlightColor, newColor);
            }
            HighlightColor targetHighlightColor = _colorsInUse[highlightColor]; 
            List<Vector2Int> previousIndices = _highlightManager.GetRegisteredIndices(targetHighlightColor);



            foreach (var i in indices)
            {
                if (_highlightManager.IsFree(i))
                {
                    GameObject newHighlight = Object.Instantiate(_highlightPrefab, _tileablePlane.transform);
                    newHighlight.transform.localPosition = _tileablePlane.IndexToLocal(i);
                    newHighlight.transform.localRotation = Quaternion.identity;
                    newHighlight.GetComponentInChildren<Renderer>().material = _colorsInUse[highlightColor].Material;
                    _currentHighlights.Add(i, newHighlight);
                }
                else if(_highlightManager.TryGetAt(i, out HighlightColor currentHiglightColor))
                {
                    if (currentHiglightColor.Color == highlightColor) continue;
                    _currentHighlights[i].GetComponentInChildren<Renderer>().material = _colorsInUse[highlightColor].Material;
                }
            }
            _highlightManager.FreeIndicesOf(_colorsInUse[highlightColor]);
            _highlightManager.RegisterAsOwner(indices, _colorsInUse[highlightColor]);
            foreach (var i in previousIndices)
            {
                if (!_highlightManager.IsFree(i)) continue;
                Object.Destroy(_currentHighlights[i]);
                _currentHighlights.Remove(i);
            }
        }

        public void ClearColor(Color targetColor)
        {
            if (!_colorsInUse.ContainsKey(targetColor)) return;
            HighlightColor highlightColor = _colorsInUse[targetColor];
            List<Vector2Int> indices = _highlightManager.GetRegisteredIndices(highlightColor);
            _highlightManager.FreeIndicesOf(_colorsInUse[targetColor]);
            foreach (var i in indices)
            {
                Object.Destroy(_currentHighlights[i]);
                _currentHighlights.Remove(i);
            }
        }

        public void ClearAll()
        {
            foreach (var obj in _currentHighlights.Values)
            {
                Object.Destroy(obj);
            }
            _currentHighlights.Clear();
            _highlightManager.Clear();
        }
        
    }
}

// Get current indices
//loop trhough new, if is not free, and dif owner, change color
// if is free, add new
//Clear then reregister
// loop throug current, if is free, remove

