using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GSceneEventManager))]
public class GSceneEventManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Find and Register All GEventBase Objects"))
        {
            GSceneEventManager manager = (GSceneEventManager)target;

            if (manager.eventBase == null)
            {
                Debug.LogWarning("eventBase 리스트가 null입니다.");
                return;
            }

            GEventBase[] allEventObjects = FindObjectsOfType<GEventBase>(true);

            foreach (var obj in allEventObjects)
            {
                if (!manager.eventBase.Contains(obj))
                    manager.eventBase.Add(obj);
            }

            EditorUtility.SetDirty(manager);
            Debug.Log($"총 {allEventObjects.Length}개의 GEventBase 오브젝트를 등록했습니다.");
        }
    }
}