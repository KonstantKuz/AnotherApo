using System;
using UnityEditor;
// Change 'Type' to base class name
// in which array is needs to be drawn as reorderable
[CustomEditor(typeof(MusicMixer))]
public class MusicMixerEditor : Editor
{
    // List/Array property name in base class
    private string arrayPropertyName = "parts";
    private ReorderableDrawer arrayDrawer;
    
    private void OnEnable()
    {
        arrayDrawer = new ReorderableDrawer(ReorderableType.WithRemoveButtons, false);
        arrayDrawer.SetUp(serializedObject, arrayPropertyName);
    }
    public override void OnInspectorGUI()
    {
        // If you have some different arrays in one class use
        // DrawPropertiesExcluding(serializedObject,  new string [] { arrayPropertyName1, arrayPropertyName2, ...} );
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject,  arrayPropertyName);
        serializedObject.ApplyModifiedProperties();
        arrayDrawer.Draw(serializedObject, target);
    }
}

