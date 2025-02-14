using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConnectTest))]
public class ConnectTestButton : Editor
{
    private string input_PIN;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ConnectTest connectTest = (ConnectTest)target;
        input_PIN = connectTest.input_PIN;

        if (GUILayout.Button("ConnectTest"))
        {
            connectTest.ConnectTester(input_PIN);
        }
    }

}
