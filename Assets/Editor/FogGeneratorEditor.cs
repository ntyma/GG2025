using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FogScript))]
public class FogGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FogScript fogGenerator = (FogScript)target;

        if (DrawDefaultInspector())
        {
            if(fogGenerator.autoUpdate)
            {
                fogGenerator.GenerateFog();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            fogGenerator.GenerateFog();
        }
    }
}

