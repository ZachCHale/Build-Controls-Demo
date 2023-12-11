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
using ZHDev.TileMaps;


namespace Demo
{
    [RequireComponent(typeof(UIDocument))]
    public class BuildControls : MonoBehaviour
    {
        [SerializeField] private StyleSheet _styleSheet;
        [SerializeField] private Texture2D _deleteButtonBackgroundImage;
        [SerializeField] private TileableBuildingManager _buildingManager;
        private Highlighter _highlighter;
        private TileablePlane BuildingManagerPlane => _buildingManager.TileablePlane;
        private VisualElement _blockingUI;

        private UIDocument _document;
        
        private TileObjectData _selectedObjData;

        private CardinalDirection _currentFacingDirection = CardinalDirection.North;
        private bool isInDeleteMode = false;

        private void Awake()
        {
            StartCoroutine(RenderUI());
            Assert.IsNotNull(_buildingManager, "BuildingManager was not set on build controls in inspector");
            _highlighter = new(BuildingManagerPlane);
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
                    _highlighter.SetHighlights(_buildingManager.GetIndicesOfBuilding(foundBuilding), Color.red);
                    _highlighter.ClearColor(Color.green);
                }
                else
                {
                    _highlighter.ClearAll();
                }
              
            }
            else if(_selectedObjData != null)
            {
                List<Vector2Int> previewIndices =
                    _selectedObjData.GetTransformIndices(targetIndex, _currentFacingDirection);
                List<Vector2Int> validIndices = _buildingManager.GetUnobstructedIndices(previewIndices);
                List<Vector2Int> invalidIndices = _buildingManager.GetObstructedIndices(previewIndices);

                _highlighter.SetHighlights(validIndices, Color.green);
                _highlighter.SetHighlights(invalidIndices, Color.red);
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

        private void HandleObjDataButtonClick(MouseDownEvent evt, TileObjectData objData)
        {
            isInDeleteMode = false;
            _selectedObjData = objData;
        }

        private void HandleDeleteButtonClick(MouseDownEvent evt)
        {
            isInDeleteMode = true;
            _selectedObjData = null;
        }

        private T CreateElement<T>(VisualElement parent = null, string[] classNames = null)
            where T : VisualElement, new()
        {
            T element = new();
            if (parent is null)
                _document.rootVisualElement.Add(element);
            else
                parent.Add(element);
            if (classNames is null) return element;
            foreach (string className in classNames)
                element.AddToClassList(className);

            return element;
        }

        private VisualElement CreateElement(VisualElement parent = null, string[] classNames = null) =>
            CreateElement<VisualElement>(parent, classNames);

        private void OnValidate() => StartCoroutine(RenderUI());

        private IEnumerator RenderUI()
        {
            yield return null;
            _document = GetComponent<UIDocument>();
            Assert.IsNotNull(_document.panelSettings, "Missing Panel Settings for BuildControls.");
            Assert.IsNotNull(_styleSheet, "Missing style sheet for BuildControls.");
            Assert.IsNotNull(_deleteButtonBackgroundImage, "Missing delete button background image for BuildControls.");


            VisualElement root = _document.rootVisualElement;
            root.Clear();
            root.styleSheets.Add(_styleSheet);
            root.pickingMode = PickingMode.Position;
            root.focusable = true;
            root.Focus();
            root.RegisterCallback<MouseDownEvent>(HandleSceneClick);
            root.RegisterCallback<KeyDownEvent>(HandleSceneKeyPress);

            VisualElement container = CreateElement(classNames: new[] { "container" });
            container.RegisterCallback<MouseDownEvent>(PreventSceneClick);

            VisualElement mainPanel = CreateElement(parent: container, classNames: new[] { "main-panel" });

            ScrollView scrollView = CreateElement<ScrollView>(parent: mainPanel, classNames: new[] { "scroll-view" });
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            VisualElement scrollViewContent =
                CreateElement(parent: scrollView, classNames: new[] { "scroll-view-content" });

            TileObjectData[] allPlaceableObjectData = Resources.LoadAll("PlaceableObjData", typeof(TileObjectData))
                .Cast<TileObjectData>()
                .ToArray();

            VisualElement deleteButton =
                CreateElement(parent: scrollViewContent, classNames: new[] { "delete-button" });
            deleteButton.style.backgroundImage = _deleteButtonBackgroundImage;
            deleteButton.RegisterCallback<MouseDownEvent>(HandleDeleteButtonClick);

            foreach (TileObjectData objData in allPlaceableObjectData)
            {
                VisualElement button = CreateElement(parent: scrollViewContent,
                    classNames: new[] { "building-data-button" });
                button.style.backgroundImage = objData.MenuPreviewImage;
                button.RegisterCallback<MouseDownEvent, TileObjectData>(HandleObjDataButtonClick, objData);
            }

        }
    }
}
