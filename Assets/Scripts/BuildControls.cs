using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildControls : MonoBehaviour
{
    [SerializeField] private PlaceableObjData selectedData;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        GameObject.Instantiate(selectedData.PrefabReference);
    }
}
