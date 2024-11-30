using UnityEditor;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Update serialized object
        serializedObject.Update();

        // Display properties
        EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerOnceOnly"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("doFreezeTime"));

        // Display list of dialogue data
        SerializedProperty listProperty = serializedObject.FindProperty("dialogueData");
        EditorGUILayout.PropertyField(listProperty, true);

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}