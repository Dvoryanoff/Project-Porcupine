using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(AutomaticVerticalSize))]
public class AutomaticVerticalSizeEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        bool button = GUILayout.Button("Recalc size");

        if (button) {
            AutomaticVerticalSize automaticVerticalSize = (AutomaticVerticalSize)target;
            if (button) {
                automaticVerticalSize.AdjustSise();
                //typeof(AutomaticVerticalSize).
                //GetMethod("AdjustSize", BindingFlags.NonPublic | BindingFlags.Instance).
                //Invoke(new AutomaticVerticalSize(), null);
            }
        }
    }
}
