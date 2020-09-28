using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public static class ReorderableListCreator
{
    public static ReorderableList SimpleList(SerializedObject serializedObject, SerializedProperty listProperty,
                                             bool showDefaultButtons)
    {
        ReorderableList list = new ReorderableList(serializedObject, listProperty, true, true,
                                                   showDefaultButtons, showDefaultButtons);

        list.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };

        return list;
    }

    public static ReorderableList WithRemoveButtons(SerializedObject serializedObject,
                                                    SerializedProperty listProperty, bool showDefaultButtons)
    {
        ReorderableList list = new ReorderableList(serializedObject, listProperty, true, true,
                                                   showDefaultButtons, showDefaultButtons);

        list.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
        {
            Rect elementRect = new Rect(rect.x, rect.y, 2 * rect.width / 3, EditorGUIUtility.singleLineHeight);
            Rect removeButtonRect = new Rect(rect.x + 2 * rect.width / 3, rect.y, rect.width / 3,
                                             EditorGUIUtility.singleLineHeight);

            if (ReorderableListTools.RemoveButton(removeButtonRect))
            {
                list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = null;
                list.serializedProperty.DeleteArrayElementAtIndex(index);
                list.serializedProperty.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (ReorderableListTools.IndexWasOutOfBounds(list, index))
                    return;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(elementRect, element, GUIContent.none);
            }
        };

        return list;
    }

    public static ReorderableList WithOneLineProperties(SerializedObject serializedObject,
                                                        SerializedProperty listProperty,
                                                        string[] propertiesNamesToDraw)
    {
        ReorderableList list = new ReorderableList(serializedObject, listProperty, true, true,
                                                   true, true);

        list.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
        {
            if (ReorderableListTools.IndexWasOutOfBounds(list, index))
                return;

            for (int i = 0; i < propertiesNamesToDraw.Length; i++)
            {
                Rect elementRect = new Rect(rect.x + rect.width / propertiesNamesToDraw.Length * i, rect.y,
                                            rect.width / propertiesNamesToDraw.Length,
                                            EditorGUIUtility.singleLineHeight);
                var element = list.serializedProperty.GetArrayElementAtIndex(index)
                                  .FindPropertyRelative(propertiesNamesToDraw[i]);
                EditorGUI.PropertyField(elementRect, element, GUIContent.none);
            }
        };

        return list;
    }
}