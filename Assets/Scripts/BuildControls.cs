using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class BuildControls : MonoBehaviour
{
    [SerializeField] private PlaceableObjData _selectedData;
    [SerializeField] private StyleSheet _styleSheet;

    private VisualElement _blockingUI;

    private UIDocument _document;

    private void Awake()
    {
        StartCoroutine(RenderUI());
    }

    private void HandleSceneClick(MouseDownEvent evt)
    {
        GameObject.Instantiate(_selectedData.PrefabReference);
    }
    private void PreventSceneClick(MouseDownEvent evt)
    {
        evt.StopPropagation();
    }
    private void HandleObjDataButtonClick(MouseDownEvent evt, PlaceableObjData objData)
    {
        _selectedData = objData;
    }

    private void OnValidate() => StartCoroutine(RenderUI());
    
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
    
    private IEnumerator RenderUI()
    {
        yield return null;
        _document = GetComponent<UIDocument>();
        Assert.IsNotNull(_document.panelSettings);
        Assert.IsNotNull(_styleSheet);

        var root = _document.rootVisualElement;
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


        var allPlaceableObjectData = Resources.LoadAll("PlaceableObjData", typeof(PlaceableObjData)).Cast<PlaceableObjData>()
            .ToArray();

        foreach (var objData in allPlaceableObjectData)
        {
            VisualElement button = CreateElement(parent: scrollViewContent, classNames: new[]{"building-data-button"});
            button.style.backgroundImage = objData.ManuPreviewImage;
            button.RegisterCallback<MouseDownEvent, PlaceableObjData>(HandleObjDataButtonClick, objData);
        }
        
    }
}
