using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public enum ReorderableType
{
    Simple,
    WithRemoveButtons,
    WithOneLineProperties,
}

public class ReorderableDrawer
{
    private SerializedProperty property;
    private ReorderableList reorderableList;
    private Rect dragnDropArea;
    private ReorderableType reorderableType;
    private bool showDefaultButtons;
    private string[] elementProperties;
    
    public ReorderableDrawer(ReorderableType reorderableType, bool showDefaultButtons)
    {
        this.reorderableType = reorderableType;
        this.showDefaultButtons = showDefaultButtons;
    }
    /// <summary>
    /// Only if ReorderableType == WithOneLineProperties
    /// </summary>
    public ReorderableDrawer(ReorderableType reorderableType, string[] elementProperties)
    {
        this.reorderableType = reorderableType;
        this.elementProperties = elementProperties;
    }

    public void SetUp(SerializedObject serializedObject, string propertyName)
    {
        property = serializedObject.FindProperty(propertyName);

        switch (reorderableType)
        {
            case ReorderableType.Simple:
                reorderableList =
                    ReorderableListCreator.SimpleList(serializedObject, property, showDefaultButtons);
                break;
            case ReorderableType.WithRemoveButtons:
                reorderableList =
                    ReorderableListCreator.WithRemoveButtons(serializedObject, property, showDefaultButtons);
                break;
            case ReorderableType.WithOneLineProperties:
                reorderableList =
                    ReorderableListCreator.WithOneLineProperties(serializedObject, property, 
                                                                 elementProperties);
                break;
        }

        if (reorderableType == ReorderableType.WithOneLineProperties)
        {
            reorderableList.drawHeaderCallback = delegate(Rect rect)
            {
                EditorGUI.LabelField(rect, propertyName);
            };
        }
        else
        {
            reorderableList.drawHeaderCallback = delegate(Rect rect)
            {
                dragnDropArea = rect;
                EditorGUI.LabelField(rect, "Drag'n'Drop objects here");
            };
        }
    }

    public void Draw(SerializedObject serializedObject, UnityEngine.Object target)
    {
        serializedObject.Update();

        ReorderableListTools.HandleShowStatusByButton(reorderableList);

        ReorderableListTools.AddElementsByDragAndDropWithType(property, dragnDropArea);

        if (GUI.changed)
        {
            Undo.RecordObject(target, $"{reorderableList.serializedProperty.name} Modify");
            EditorUtility.SetDirty(target);
        }

        serializedObject.ApplyModifiedProperties();
    }
}