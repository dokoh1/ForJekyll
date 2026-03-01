using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NotebookTabButton : MonoBehaviour
{
    [SerializeField] Image missionImage;
    [SerializeField] Image MapImage;

    [SerializeField] private TMP_Text missionText;
    [SerializeField] private TMP_Text mapText;

    [SerializeField] private List<Sprite> sprites;

    [Header("Width Settings")]
    [SerializeField] private float minTextWidth = 165f;
    [SerializeField] private float maxTextWidth = 200f;

    public void UpdateImage(string tabName)
    {
        if (missionImage == null || MapImage == null)
        {
            Debug.LogError("NotebookTabButton: missionImage");
            return;
        }

        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogError("NotebookTabButton: sprites 리스트");
            return;
        }

        if (tabName == "Map")
        {
            MapImage.sprite = sprites[0];
            SetWidth(mapText.rectTransform, maxTextWidth);
            missionImage.sprite = sprites[1];
            SetWidth(missionText.rectTransform, minTextWidth);
        }
        else if (tabName == "Mission")
        {
            MapImage.sprite = sprites[2];
            SetWidth(mapText.rectTransform, minTextWidth);
            missionImage.sprite = sprites[3];
            SetWidth(missionText.rectTransform, maxTextWidth);
        }
    }

    private void SetWidth(RectTransform rectTransform, float width)
    {
        var size = rectTransform.sizeDelta;
        size.x = width;
        rectTransform.sizeDelta = size;
    }
}