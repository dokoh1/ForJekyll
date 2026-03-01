using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public class NotebookMapData
{
    public string mapName;
    public Sprite mapSprite;
}

public class NotebookMap : MonoBehaviour
{
    [Header("Map Sprites")]
    [SerializeField] private List<NotebookMapData> maps;

    [Header("UI Image")]
    [SerializeField] private Image image;

    [Header("Localize")]
    [SerializeField] private LocalizeStringEvent localizeStringEvent;

    public void UpdateMap(string name)
    {
        NotebookMapData map = maps.Find(m => m.mapName == name);
        if (map != null)
        {
            image.sprite = map.mapSprite;
            ChangeMapName(name);
        }
        else
        {
            Debug.LogWarning($"Map '{name}' not found!");
        }
    }

    public void ChangeMapName(string name)
    {
        if (localizeStringEvent == null) return;

        localizeStringEvent.StringReference.TableEntryReference = name;

        localizeStringEvent.RefreshString();
    }
}