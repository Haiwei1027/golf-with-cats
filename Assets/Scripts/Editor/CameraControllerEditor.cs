using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraController controller = (CameraController)target;
        if (GUILayout.Button("Update"))
        {
            controller.UpdateCamera();
        }
    }
}
