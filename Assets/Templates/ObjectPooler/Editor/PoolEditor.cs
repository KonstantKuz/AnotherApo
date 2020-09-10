using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pool))]
[CanEditMultipleObjects]
public class PoolEditor : Editor
{
    string[] propertiesInBaseClass = new string[] { "autoReturn", "autoReturnDelay", "nameAsTag", "poolTag"};
    
    private bool autoReturnInAll;
    private float autoReturnDelayInAll;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, propertiesInBaseClass);

        if (targets.Length > 1)
        {
            HandleAllSelectedPools();
        }
        else
        {
            HandleConcretePool();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void HandleConcretePool()
    {
        Pool pool = (Pool)target;

        pool.nameAsTag = EditorGUILayout.Toggle("Use name as tag", pool.nameAsTag);
        if (!pool.nameAsTag)
        {
            pool.poolTag = EditorGUILayout.TextField("Pool tag", pool.poolTag);
        }

        pool.autoReturn = EditorGUILayout.Toggle("Use autoreturn", pool.autoReturn);
        if (pool.autoReturn)
        {
            pool.autoReturnDelay = EditorGUILayout.FloatField("Delay", pool.autoReturnDelay);
        }

        if (GUI.changed)
        {
            Undo.RecordObject(pool, $"Pool Modify");
            EditorUtility.SetDirty(pool);
        }
    }
    
    private void HandleAllSelectedPools()
    {
        autoReturnInAll = EditorGUILayout.Toggle("Use auto return", autoReturnInAll);
        if (autoReturnInAll)
        {
            autoReturnDelayInAll = EditorGUILayout.FloatField("Delay", autoReturnDelayInAll);
        }
        
        for (int i = 0; i < targets.Length; i++)
        {
            Pool pool = (Pool) targets[i];
            pool.autoReturn = autoReturnInAll;
            if (autoReturnInAll)
                pool.autoReturnDelay = autoReturnDelayInAll;
            
            if (GUI.changed)
            {
                Undo.RecordObject(pool, $"Pool with prefab {pool.prefab.name} Modify");
                EditorUtility.SetDirty(pool);
            }
        }
    }
}
