#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugController))]
public class DebugControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugController debug = (DebugController)target;

        if (debug.roundMethodOptions != null && debug.roundMethodOptions.Length > 0)
        {
            debug.selectedRoundIndex = EditorGUILayout.Popup("Select Round", debug.selectedRoundIndex, debug.roundMethodOptions);

            if (GUILayout.Button("Trigger Selected Round"))
            {
                debug.TriggerSelectedRound();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No round methods found. Is Rounds.cs assigned?", MessageType.Info);
        }
    }
}
#endif
