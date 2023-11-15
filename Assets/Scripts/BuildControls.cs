using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

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

        var container = new VisualElement();
        container.AddToClassList("container");
        root.Add(container);
        container.RegisterCallback<MouseDownEvent>(PreventSceneClick);

        var mainPanel = new VisualElement();
        mainPanel.AddToClassList("main-panel");
        container.Add(mainPanel);

        var scrollView = new ScrollView();
        scrollView.AddToClassList("scroll-view");
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        mainPanel.Add(scrollView);
        

        var scrollViewContent = new VisualElement();
        scrollViewContent.AddToClassList("scroll-view-content");
        scrollView.Add(scrollViewContent);

        var allPlaceableObjectData = Resources.LoadAll("PlaceableObjData", typeof(PlaceableObjData)).Cast<PlaceableObjData>()
            .ToArray();

        foreach (var objData in allPlaceableObjectData)
        {
            var button = new VisualElement();
            button.AddToClassList("building-data-button");
            button.style.backgroundImage = objData.ManuPreviewImage;
            scrollViewContent.Add(button);
            button.RegisterCallback<MouseDownEvent, PlaceableObjData>(HandleObjDataButtonClick, objData);
        }
        
    }
}
