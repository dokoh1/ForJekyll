using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightSystem))]
public class FindLightObject : Editor
{
    public override void OnInspectorGUI()
    {
        LightSystem lightSystem = (LightSystem)target;
        
        DrawDefaultInspector();

        if (lightSystem.noneLightMat is null)
        {
            EditorGUILayout.HelpBox("No none light mat", MessageType.Info);
            return;
        }

        if (GUILayout.Button("Find All Objects with Material"))
        {
            lightSystem.useLightRenderer.Clear();
            Renderer[] allRenderers = FindObjectsOfType<Renderer>(true);

            foreach (var r in allRenderers)
            {
                if (r.sharedMaterial == lightSystem.noneLightMat ||
                    System.Array.Exists(r.sharedMaterials, mat => mat == lightSystem.noneLightMat))
                {
                    lightSystem.useLightRenderer.Add(r);
                }
            }
            EditorUtility.SetDirty(lightSystem);
        }
    }
}