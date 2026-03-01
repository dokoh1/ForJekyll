using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SideStoryPanel : MonoBehaviour
{
    [SerializeField] private ScenarioAchieve[] phaseAchieves;
    [SerializeField] private Button[] SideStories;
    [SerializeField] CanvasGroup mainMenu;
    [SerializeField] CanvasGroup fade;

    void OnEnable()
    {
        CheckSideStoryOpen();
    }

    private void CheckSideStoryOpen()
    {
        for (int i = 0; i < SideStories.Length; i++)
        {
            bool isAchieved = ScenarioManager.Instance.GetAchieve(phaseAchieves[i]);

            SideStories[i].interactable = isAchieved;

            var img = SideStories[i].targetGraphic as Image;
            if (img != null)
            {
                img.color = isAchieved ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            }
        }
    }

    public void StartSideStory(int index)
    {
        string nameKey;

        switch (index)
        {
            case 0:
                nameKey = "강주명";
                break;
            case 52:
                nameKey = "표서윤";
                break;
            case 104:
                nameKey = "연효진";
                break;
            default:
                nameKey = null;
                break;
        }

        StartSideStory(index, nameKey);
    }

    private void StartSideStory(int index, string nameKey)
    {
        UIManager.Instance.dialogueEnd += EndSideStory;
        UIManager.Instance.DialogueOpen(Dialogue.Favor, true, index, nameKey);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1.5f);
        sequence.AppendCallback(() =>
        {
            mainMenu.alpha = 0f;
            mainMenu.blocksRaycasts = false;
            mainMenu.interactable = false;
            fade.alpha = 0f;
            fade.blocksRaycasts = false;
            fade.interactable = false;
        });
        sequence.Play();
    }

    private void EndSideStory()
    {
        fade.alpha = 1f;
        fade.blocksRaycasts = true;
        fade.interactable = true;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() =>
        {
            mainMenu.alpha = 1f;
            mainMenu.blocksRaycasts = true;
            mainMenu.interactable = true;
        });
        sequence.Play();
    }
}