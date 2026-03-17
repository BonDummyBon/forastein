using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(VendingRecipe))]
public class VendingRecipeEditor : Editor {
    private SerializedProperty recipeName;
    private SerializedProperty waitTime;
    private SerializedProperty inputs;
    private SerializedProperty outputs;

    private void OnEnable() {
        recipeName = serializedObject.FindProperty("RecipeName");
        waitTime = serializedObject.FindProperty("WaitTime");
        inputs = serializedObject.FindProperty("Inputs");
        outputs = serializedObject.FindProperty("Outputs");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.LabelField("Static Attributes", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(recipeName);
        EditorGUILayout.PropertyField(waitTime);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(inputs);
        EditorGUILayout.PropertyField(outputs);

        serializedObject.ApplyModifiedProperties();
    }
}
