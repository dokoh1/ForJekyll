using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IllustView : MonoBehaviour
{
    private List<(string, Illust)> onIllustList = new(2);
    [SerializeField] private SerializableDictionary<string, Illust> illustList;
    [SerializeField] private RectTransform onIllust;
    [SerializeField] private RectTransform offIllust;

    [Header("DoScale 설정")] 
    [SerializeField] private float scaleSize;
    [SerializeField] private float scaleTime;
    [SerializeField] private Color offIllustColor;

    private void Start()
    {
        foreach (var item in illustList)
        {
            item.Value.IllustImage.color = offIllustColor;
            item.Value.IllustImage.rectTransform.localScale = Vector3.one * scaleSize;
            //item.Value.IllustImage.rectTransform.DOScale(scaleSize, 0);
        }
    }

    public void SetImage(string npcName, string faceKey)
    {
        if (DataManager.Instance.GetNpcName(npcName, out string nameKey))
            npcName = nameKey;

        var current = onIllustList.FirstOrDefault(x => x.Item1 == npcName);
        var other = onIllustList.FirstOrDefault(x => x.Item1 != npcName);
        
        if (current.Item2 != null)
        {
            if (current.Item2.Face != faceKey && DataManager.Instance.GetNpcFace(npcName, faceKey, out Sprite sprite))
            {
                current.Item2.IllustImage.sprite = sprite;
            }

            if (current.Item2.Faded)
                FadeImage(current.Item2, false);
            
            if (other.Item2 != null && !other.Item2.Faded)
                FadeImage(other.Item2, true);
            
            return;
        }
        
        if (illustList.TryGetValue(npcName, out Illust illust) && 
            DataManager.Instance.GetNpcFace(npcName, faceKey, out Sprite face))
        {
            int siblingIndex = 0;
            if (onIllustList.Count == 2)
            {
                CloseImage(out siblingIndex);
            }
            else
                siblingIndex = 1;

            var on = onIllustList.FirstOrDefault(x => !x.Item2.Faded);
            if (on.Item2 != null)
            {
                FadeImage(on.Item2, true);
            }
            
            illust.Face = faceKey;
            illust.IllustImage.sprite = face;
            illust.IllustImage.rectTransform.SetParent(onIllust);
            illust.IllustImage.rectTransform.SetSiblingIndex(siblingIndex);
            onIllustList.Add((npcName, illust));
            FadeImage(illust, false);
        }
        else
        {
            foreach (var item in onIllustList)
            {
                if (!item.Item2.Faded)
                    FadeImage(item.Item2, true);
            }
        }
    }

    private void FadeImage(Illust illust, bool fadeIn)
    {
        illust.Faded = fadeIn;

        illust.IllustImage.gameObject.SetActive(true);
        illust.IllustImage.DOKill();
        //illust.IllustImage.DOFade(fadeIn? 0.75f : 1f, 1f);
        illust.IllustImage.DOColor(fadeIn ? offIllustColor : Color.white, scaleTime);
        illust.IllustImage.rectTransform.DOKill();
        illust.IllustImage.rectTransform.DOScale(fadeIn ? scaleSize : 1f, scaleTime);
    }

    public void FadeAllImage()
    {
        foreach (var item in onIllustList)
        {
            if (!item.Item2.Faded)
                FadeImage(item.Item2, true);
        }
    }

    private void CloseImage(out int _siblingIndex)
    {
        var fadedIllust = onIllustList.FirstOrDefault(x => x.Item2.Faded);
        _siblingIndex = fadedIllust.Item2.IllustImage.transform.GetSiblingIndex();
        onIllustList.Remove(fadedIllust);

        fadedIllust.Item2.IllustImage.rectTransform.localScale = Vector3.one * scaleSize;
        fadedIllust.Item2.IllustImage.color = offIllustColor;
        fadedIllust.Item2.IllustImage.rectTransform.SetParent(offIllust);
        
        if (!onIllustList.First().Item2.Faded)
            FadeImage(onIllustList.First().Item2, true);
    }

    public void CloseAllImage()
    {
        foreach (var item in onIllustList)
        {
            item.Item2.IllustImage.rectTransform.DOScale(scaleSize, 0f);
            item.Item2.IllustImage.rectTransform.SetParent(offIllust);
        }
        onIllustList.Clear();
    }

    [Serializable]
    private class Illust
    {
        [field: SerializeField] public FadeEffect Fade { get; set; }
        [field: SerializeField] public Image IllustImage { get; set; }
        [field: SerializeField] public string Face { get; set; }
        [field: SerializeField] public bool Faded { get; set; }
    }
}
