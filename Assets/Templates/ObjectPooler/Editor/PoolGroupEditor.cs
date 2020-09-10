using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PoolGroup))]
public class PoolGroupEditor : Editor
{
    private string arrayPropertyName = "poolsInGroup";
    
    private ReorderableDrawer poolsDrawer;
    private void OnEnable()
    {
        poolsDrawer = new ReorderableDrawer(ReorderableType.WithOneLineProperties, 
                                            new string[] { "pool", "weight" });
        poolsDrawer.SetUp(serializedObject, arrayPropertyName);
    }

    public override void OnInspectorGUI()
    {
        HandleGroup();
        
        poolsDrawer.Draw(serializedObject, target);
    }

    private void HandleGroup()
    {
        serializedObject.Update();
        
        PoolGroup poolGroup = (PoolGroup)target;
        poolGroup.groupTag = EditorGUILayout.TextField("Group tag", poolGroup.groupTag);
        
        if (GUI.changed)
        {
            Undo.RecordObject(poolGroup, $"Pool group Modify");
            EditorUtility.SetDirty(poolGroup);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}

// [CustomEditor(typeof(PoolGroup))]
// public class PoolGroupEditor : Editor
// {
//     private SerializedProperty poolsInGroupProperty;
//     private ReorderableList poolsInGroupList;
//     private Rect addPoolsToGroupDragnDropArea;
//
//     private const string poolsInGroupPropertyName = "poolsInGroup";
//     private string[] nonDrawingProperties = new string[] { poolsInGroupPropertyName };
//
//     private void OnEnable()
//     {
//         poolsInGroupProperty = serializedObject.FindProperty(poolsInGroupPropertyName);
//         
//         DrawPoolsInGroupList();
//     }
//
//     private void DrawPoolsInGroupList()
//     {
//         poolsInGroupList = ReorderableListCreator.SimpleListWithRemoveButtonOnEachElement(serializedObject, poolsInGroupProperty, false);
//         addPoolsToGroupDragnDropArea = ReorderableListTools.DragnDropAreaOnHeader(poolsInGroupList);
//     }
//
//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();
//
//         DrawPropertiesExcluding(serializedObject, nonDrawingProperties);
//
//         ReorderableListTools.HandleShowStatusByButton(poolsInGroupList);
//         
//         ReorderableListTools.AddElementsByDragAndDropWithType<Pool>(poolsInGroupProperty, addPoolsToGroupDragnDropArea);
//
//         if (GUI.changed)
//         {
//             PoolGroup poolGroup = (PoolGroup)target;
//             Undo.RecordObject(poolGroup, $"Pool group Modify");
//             EditorUtility.SetDirty(poolGroup);
//         }
//
//         serializedObject.ApplyModifiedProperties();
//     }
// }