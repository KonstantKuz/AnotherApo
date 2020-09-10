using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public static class ReorderableListTools
{
    public static bool RemoveButton(Rect rect)
    {
        if (GUI.Button(rect, "Remove")) 
        { return true; }
        else { return false; }
    }

    public static bool IndexWasOutOfBounds(ReorderableList list, int elementIndex)
    {
        if (elementIndex > list.serializedProperty.arraySize - 1) 
        { return true; }
        else { return false; }
    }

    private static readonly Dictionary<ReorderableList, bool> ShowStatuses = new Dictionary<ReorderableList, bool>();

    public static void HandleShowStatusByButton(ReorderableList list)
    {
        if (ShowStatuses.ContainsKey(list))
        {
            if (ShowStatuses[list])
            {
                if (GUILayout.Button($"Hide {list.serializedProperty.name}"))
                    ShowStatuses[list] = false;
            }
            else
            {
                if (GUILayout.Button($"Show {list.serializedProperty.name}"))
                    ShowStatuses[list] = true;
            }
        }
        else
        {
            ShowStatuses.Add(list, new bool());
        }

        if (ShowStatuses[list])
            list.DoLayoutList();
    }

    public static void AddElementsByDragAndDropWithType(SerializedProperty listProperty, Rect dragnDropArea)
    {
        Event currentEvent = Event.current;
        switch (currentEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dragnDropArea.Contains(currentEvent.mousePosition))
                    return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    TryAddElementsFromDragAndDrop(listProperty);
                }

                break;
        }
    }

    private static void
        TryAddElementsFromDragAndDrop(SerializedProperty listProperty)
    {
        foreach (Object draggedObject in DragAndDrop.objectReferences)
        {
            if (ElementContainsInArray(listProperty, draggedObject))
            {
                Debug.LogWarning($"Element {draggedObject.name} is already exists in {listProperty.name} list!");
            }
            else
            {
                listProperty.arraySize++;
                listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).objectReferenceValue = null;
                listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).objectReferenceValue = draggedObject;
            }
        }
    }

    private static bool ElementContainsInArray(SerializedProperty listProperty, Object objectToCheck)
    {
        bool contains = false;

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            if (listProperty.GetArrayElementAtIndex(i).objectReferenceValue == objectToCheck)
            {
                contains = true;
                break;
            }
        }

        return contains;
    }
}