using Sirenix.OdinInspector;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[Serializable]
public enum SceneEnum
{
    MainMenu,
    Tutorial,
    Chapter1_1,
    Chapter1_2,
    Chapter1_3,
    Chapter2_1,
    Chapter2_2,
    Chapter2_3,
    Chapter3_1,
    Chapter3_2,
    Chapter3_3,
    Chapter4_1,
    Chapter4_2,
    Chapter4_3,
    Chapter5_1,
    Chapter5_2,
    Chapter5_3,
    LoadingScene,
    Demo
}
public class FadeManager : MonoBehaviour
{
    [TitleGroup("FadeManager", "MonoBehaviour", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: false)]
    [SerializeField] FadeEffect fadeEffect;
    [SerializeField] private GameObject blackSpace;

    [TabGroup("Tab", "Scene", SdfIconType.Film, TextColor = "white")]
    [TabGroup("Tab", "Scene")][SerializeField] GameObject[] sceneLoadings;
    [TabGroup("Tab", "Scene")][SerializeField] GameObject loadingBar;

    private Sequence fadeSequence;
    public event Action fadeComplete;
    public event Action fadeInComplete;
    public event Action SceneLoading;
    public SceneEnum currentScene;
    public async Awaitable FadeStart(FadeState fadeState)
    {
        await Fade(fadeState);
    }

    public async Awaitable FadeStart(FadeState fadeState, float fadeTime)
    {
        fadeEffect.slowFade = true;
        fadeEffect.slowFadeTime = fadeTime;
        await Fade(fadeState);
    }

    public void MoveScene(SceneEnum sceneEnum)
    {
        MoveSceneFade(sceneEnum);
    }

    private async Awaitable Fade(FadeState fadeState)
    {
        switch (fadeState)
        {
            case FadeState.FadeOut:
                await fadeEffect.UseFadeEffect(FadeState.FadeOut, () => FadeComplete(fadeState));
                break;
            case FadeState.FadeIn:
                await fadeEffect.UseFadeEffect(FadeState.FadeIn, () => FadeComplete(fadeState));
                break;
        }
    }

    private void FadeComplete(FadeState fadeState)
    {
        fadeComplete?.Invoke();
        fadeComplete = null;
        fadeEffect.slowFade = false;
        if (fadeState == FadeState.FadeIn)
        {
            fadeInComplete?.Invoke();
            fadeInComplete = null;
            fadeEffect.image.enabled = false;
        }
    }
    private async Awaitable MoveSceneFade(SceneEnum sceneEnum)
    {
        currentScene = sceneEnum;
        UI_PauseMenuPopup.Instance.SwitchActiveESC(false);

        SoundManager.Instance.StopAllBGM();
        fadeComplete += OnSceneLoading;
        fadeComplete += UIManager.Instance.DialogueCloseImmediate;

        await Fade(FadeState.FadeOut);
        await Loading();

        int rand = UnityEngine.Random.Range(0, sceneLoadings.Length);
        sceneLoadings[rand].SetActive(true);
        loadingBar.SetActive(true);
        GC.Collect();
        await Resources.UnloadUnusedAssets();
        await Awaitable.WaitForSecondsAsync(3);

        var num = sceneEnum switch
        {
            SceneEnum.MainMenu => 0,
            SceneEnum.Tutorial => 2,
            SceneEnum.Demo => 4,
            SceneEnum.Chapter1_1 or SceneEnum.Chapter1_2 or SceneEnum.Chapter1_3 or SceneEnum.Chapter2_1
                or SceneEnum.Chapter2_2 or SceneEnum.Chapter3_1 or SceneEnum.Chapter3_2 => 2,
            SceneEnum.Chapter2_3 or SceneEnum.Chapter4_3 => 2,
            SceneEnum.Chapter3_3 => 5,
            SceneEnum.Chapter4_1 => 5,
            SceneEnum.Chapter4_2 => 6,
        };
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(num);

        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            await Awaitable.NextFrameAsync();
        }
        sceneLoadings[rand].SetActive(false);
        loadingBar.SetActive(false);
        UIManager.Instance.questUI.OffUI();
        UIManager.Instance.objective.ClearAll();
        GameManager.Instance.fadeManager.fadeComplete += OnEsc;

        await Fade(FadeState.FadeIn);
    }

    private void OnSceneLoading()
    {
        SceneLoading?.Invoke();
    }

    private async Awaitable Loading()
    {
        SceneManager.LoadScene("LoadingScene");
        await Awaitable.NextFrameAsync();
    }

    void OnEsc()
    {
        UI_PauseMenuPopup.Instance.SwitchActiveESC(true);
    }

    public void AddBlackSpace(PlayableDirector playable)
    {
        playable.stopped += (x) => blackSpace.SetActive(true);
        UIManager.Instance.DialogueStart += () => blackSpace.SetActive(false);
        UIManager.Instance.dialogueEnd += () => blackSpace.SetActive(false);
    }
}
