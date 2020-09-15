using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Object = System.Object;

[CustomEditor(typeof(ObjectPooler))]
public class ObjectPoolerEditor : Editor
{
    private ReorderableDrawer poolsDrawer;
    private ReorderableDrawer groupsDrawer;
    
    private const string poolsPropertyName = "pools";
    private const string poolGroupsPropertyName = "poolGroups";

    private void OnEnable()
    {
        poolsDrawer = new ReorderableDrawer(ReorderableType.WithRemoveButtons, false);
        groupsDrawer = new ReorderableDrawer(ReorderableType.WithRemoveButtons, false);
        
        poolsDrawer.SetUp(serializedObject, poolsPropertyName);
        groupsDrawer.SetUp(serializedObject, poolGroupsPropertyName);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawPropertiesExcluding(serializedObject, new string[] {poolsPropertyName, poolGroupsPropertyName});
        
        poolsDrawer.Draw(serializedObject, target );
        groupsDrawer.Draw(serializedObject, target);
        
        TryResolveUnassignedPools();

        serializedObject.ApplyModifiedProperties();
    }

    private void TryResolveUnassignedPools()
    {
        ObjectPooler objectPooler = (ObjectPooler) target;
        objectPooler.TryResolveUnassignedPools();

        if (GUI.changed)
        {
            Undo.RecordObject(objectPooler, $"ObjectPooler Modify");
            EditorUtility.SetDirty(objectPooler);
        }
    }
}
