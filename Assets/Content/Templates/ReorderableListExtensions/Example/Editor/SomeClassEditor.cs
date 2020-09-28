using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// Created using ReorderableDrawerTemplate.cs

[CustomEditor(typeof(SomeClass))]
public class SomeClassEditor : Editor
{
    private readonly string someDataListPropertyName = "someDataList";
    private readonly string[] someDataPropertiesNames = { "someInt", "someGameObject", "someType" };

    private readonly string gameObjectsArrayPropertyName = "gameObjectsArray";

    private ReorderableDrawer someDataListDrawer;
    private ReorderableDrawer gameObjectsArrayDrawer;

    private void OnEnable()
    {
        someDataListDrawer = new ReorderableDrawer(ReorderableType.WithOneLineProperties, someDataPropertiesNames);
        someDataListDrawer.SetUp(serializedObject, someDataListPropertyName);
        
        gameObjectsArrayDrawer = new ReorderableDrawer(ReorderableType.WithRemoveButtons, someDataPropertiesNames);
        gameObjectsArrayDrawer.SetUp(serializedObject, gameObjectsArrayPropertyName);
    }
    public override void OnInspectorGUI()
    {
        string[] currentlyDrawnPropertiesNames = { someDataListPropertyName, gameObjectsArrayPropertyName };
        DrawPropertiesExcluding(serializedObject, currentlyDrawnPropertiesNames);
        
        someDataListDrawer.Draw(serializedObject, target);
        gameObjectsArrayDrawer.Draw(serializedObject, target);
    }
}
