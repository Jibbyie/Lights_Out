#if false
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugController))]
public class DebugControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (target == null) return;

        DebugController debug = target as DebugController;
        if (debug == null || debug.Equals(null)) return; // Handles destroyed refs

        base.OnInspectorGUI();

        if (debug.roundMethodOptions != null &&
            debug.roundMethodOptions.Length > 0 &&
            debug.selectedRoundIndex >= 0 &&
            debug.selectedRoundIndex < debug.roundMethodOptions.Length)
        {
            debug.selectedRoundIndex = EditorGUILayout.Popup("Select Round", debug.selectedRoundIndex, debug.roundMethodOptions);

            if (GUILayout.Button("Trigger Selected Round"))
            {
                debug.TriggerSelectedRound();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No round methods found. Is the Rounds reference assigned and initialized?", MessageType.Info);
        }
    }
}
#endif
