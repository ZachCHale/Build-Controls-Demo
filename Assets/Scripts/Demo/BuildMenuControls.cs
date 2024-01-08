using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using ZHDev.CardinalDirections;
using ZHDev.Extensions;
using ZHDev.TileableSystems;


namespace Demo
{
    [RequireComponent(typeof(UIDocument))]
    public class BuildMenuControls : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TileableBuildingManager _buildingManager;

        [Header("Menu UI Assets")]
        [SerializeField] private VisualTreeAsset _buildingButtonAsset;
        [SerializeField] private VisualTreeAsset _deleteButtonAsset;


        [SerializeField] private Texture2D _unselectedButtonTexture;
        [SerializeField] private Texture2D _selectedButtonTexture;
        [SerializeField] private Texture2D _unselectedDeleteButtonTexture;
        [SerializeField] private Texture2D _selectedDeleteButtonTexture;
        
        [Header("Building Preview Settings")]
        [SerializeField] private Color _validPlacmentColor;
        [SerializeField] private Color _invalidPlacementColor;
        
        [Header("Buildings")]
        [SerializeField] private List<BuildingData> _buildingDatas;

        private Highlighter _tileHighlighter;
        private UIDocument _document;
        private BuildingData _selectedObjData;
        private CardinalDirection _currentFacingDirection = CardinalDirection.North;
        private bool isInDeleteMode = false;
        private List<VisualElement> _buildingButtons = new();
        private TileablePlane BuildingManagerPlane => _buildingManager.TileablePlane;
        private VisualElement _deleteButton;

        
        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            var root = _document.rootVisualElement;
            root.pickingMode = PickingMode.Position;
            root.focusable = true;
            root.Focus();
            root.RegisterCallback<MouseDownEvent>(HandleSceneClick);
            root.RegisterCallback<KeyDownEvent>(HandleSceneKeyPress);

            root.Q("Container").Clear();

            foreach (var building in _buildingDatas)
            {
                var button = _buildingButtonAsset.Instantiate();
                button.Q("icon").style.backgroundImage = building.MenuPreviewImage;
                root.Q("Container").Add(button);
                _buildingButtons.Add(button.Q("building-button"));
            }
            
            for (int i = 0; i < 4; i++)
            {
                _buildingButtons[i].RegisterCallback<MouseDownEvent, BuildingData>(HandleObjDataButtonClick, _buildingDatas[i]);
            }
            
            
            var deleteButton = _deleteButtonAsset.Instantiate();
            root.Q("Container").Add(deleteButton);
            _deleteButton = deleteButton.Q("delete-button");
            _deleteButton.RegisterCallback<MouseDownEvent>(HandleDeleteButtonClick);
            
            Assert.IsNotNull(_buildingManager, "BuildingManager was not set on build controls in inspector");
            _tileHighlighter = new(BuildingManagerPlane);
        }

        private void FixedUpdate()
        {
            Vector2 screenPoint = Input.mousePosition;
            Camera.main.GetScreenPointIntersectionWithPlane(screenPoint, BuildingManagerPlane.Up, BuildingManagerPlane.OriginPosition, out Vector3 targetPosition);
            Vector2Int targetIndex = BuildingManagerPlane.WorldToProjectedIndex(targetPosition);
            if (isInDeleteMode)
            {
                if (_buildingManager.TryGetBuildingAt(targetIndex, out Building foundBuilding))
                {
                    _tileHighlighter.SetHighlights(_buildingManager.GetIndicesOfBuilding(foundBuilding), _invalidPlacementColor);
                    _tileHighlighter.ClearColor(_validPlacmentColor);
                }
                else
                {
                    _tileHighlighter.ClearAll();
                }
              
            }
            else if(_selectedObjData != null)
            {
                List<Vector2Int> previewIndices =
                    _selectedObjData.GetTransformIndices(targetIndex, _currentFacingDirection);
                List<Vector2Int> validIndices = _buildingManager.GetUnobstructedIndices(previewIndices);
                List<Vector2Int> invalidIndices = _buildingManager.GetObstructedIndices(previewIndices);

                _tileHighlighter.SetHighlights(validIndices, _validPlacmentColor);
                _tileHighlighter.SetHighlights(invalidIndices, _invalidPlacementColor);
            }
            else
            {
                _tileHighlighter.ClearAll();
            }
            
        }

        private void PreventSceneClick(MouseDownEvent evt) => evt.StopPropagation();

        private void HandleSceneClick(MouseDownEvent evt)
        {
            if (evt.button != 0) return;
            Vector2 screenPoint = evt.mousePosition;
            screenPoint.y = Screen.height - screenPoint.y;
            Camera.main.GetScreenPointIntersectionWithPlane(screenPoint, BuildingManagerPlane.Up, BuildingManagerPlane.OriginPosition,
                out Vector3 targetPosition);
            if (isInDeleteMode)
                _buildingManager.TryRemoveBuildingAt(targetPosition);
            else if (_selectedObjData != null)
                _buildingManager.TryPlaceNewBuilding(_selectedObjData, targetPosition, _currentFacingDirection, out Building resultBuilding);
        }

        private void HandleSceneKeyPress(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.E)
                _currentFacingDirection = _currentFacingDirection.Rotate();
            else if (evt.keyCode == KeyCode.Q)
                _currentFacingDirection = _currentFacingDirection.Rotate(counterClockwise: true);
        }

        private void HandleObjDataButtonClick(MouseDownEvent evt, BuildingData objData)
        {
            UnhighlightAllButtons();
            
            if (_selectedObjData == objData)
            {
                _selectedObjData = null;
                return;
            }
            
            isInDeleteMode = false;
            _selectedObjData = objData;
            
            HighlightButton(((VisualElement)evt.target));

            evt.StopPropagation();
        }

        private void HandleDeleteButtonClick(MouseDownEvent evt)
        {
            UnhighlightAllButtons();

            if (isInDeleteMode)
            {
                isInDeleteMode = false;
                return;
            }
            
            isInDeleteMode = true;
            _selectedObjData = null;
            
            HighlightButtonRed(_deleteButton);
            evt.StopPropagation();
        }

        private void UnhighlightAllButtons()
        {
            foreach (var button in _buildingButtons)
            {
                button.Q("border").ClearClassList();
                button.Q("icon").ClearClassList();
                
                button.Q("border").AddToClassList("border-unselected");
                button.Q("icon").AddToClassList("icon-unselected");
            }
            _deleteButton.Q("border").ClearClassList();
            _deleteButton.Q("icon").ClearClassList();

            _deleteButton.Q("border").AddToClassList("border-unselected");
            _deleteButton.Q("icon").AddToClassList("icon-unselected");
        }

        private void HighlightButton(VisualElement button)
        {
            button.Q("border").ClearClassList();
            button.Q("icon").ClearClassList();
            
            button.Q("border").AddToClassList("border-selected");
            button.Q("icon").AddToClassList("icon-selected");
        }

        private void HighlightButtonRed(VisualElement button)
        {
            button.Q("border").ClearClassList();
            button.Q("icon").ClearClassList();
            
            button.Q("border").AddToClassList("border-selected-red");
            button.Q("icon").AddToClassList("icon-selected-red");
        }
        
    }
}
