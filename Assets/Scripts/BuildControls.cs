using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class BuildControls : MonoBehaviour
{
    [SerializeField] private StyleSheet _styleSheet;
    [SerializeField] private Texture2D _deleteButtonBackgroundImage;

    private VisualElement _blockingUI;

    private UIDocument _document;

    private TileMap _map;

    private TileObjectData _selectedObjData;
    private bool isInDeleteMode = false;

    private void Awake()
    {
        StartCoroutine(RenderUI());
        _map = FindObjectOfType<TileMap>();
    }

    private void PreventSceneClick(MouseDownEvent evt) => evt.StopPropagation();
    private void HandleSceneClick(MouseDownEvent evt)
    {
        if (evt.button != 0) return;
        if (isInDeleteMode)
            _map.RemoveObjectAtMousePosition(out TileObjectData removedObjData);
        else if(_selectedObjData != null)
            _map.PlaceObjectAtMousePosition(_selectedObjData);
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
    
    private T CreateElement<T>(VisualElement parent = null, string[] classNames = null ) where T : VisualElement, new()
    {
        T element = new();
        if(parent is null)
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
        root.RegisterCallback<MouseDownEvent>(HandleSceneClick);

        VisualElement container = CreateElement(classNames: new[]{"container"});
        container.RegisterCallback<MouseDownEvent>(PreventSceneClick);

        VisualElement mainPanel = CreateElement(parent: container, classNames: new[]{"main-panel"});

        ScrollView scrollView = CreateElement<ScrollView>(parent: mainPanel, classNames: new[] { "scroll-view" });
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        
        VisualElement scrollViewContent =
            CreateElement(parent: scrollView, classNames: new[] { "scroll-view-content" });
        
        TileObjectData[] allPlaceableObjectData = Resources.LoadAll("PlaceableObjData", typeof(TileObjectData)).Cast<TileObjectData>()
            .ToArray();

        VisualElement deleteButton =
            CreateElement(parent: scrollViewContent, classNames: new[] { "delete-button" });
        deleteButton.style.backgroundImage = _deleteButtonBackgroundImage;
        deleteButton.RegisterCallback<MouseDownEvent>(HandleDeleteButtonClick);
        
        foreach (TileObjectData objData in allPlaceableObjectData)
        {
            VisualElement button = CreateElement(parent: scrollViewContent, classNames: new[]{"building-data-button"});
            button.style.backgroundImage = objData.MenuPreviewImage;
            button.RegisterCallback<MouseDownEvent, TileObjectData>(HandleObjDataButtonClick, objData);
        }
        
    }
}
