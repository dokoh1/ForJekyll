using System.Collections;
using Michsky.UI.Dark;
using UnityEngine;

public class UI_SplashScreenPopup : UI_Popup
{
    private enum GameObjects
    {
        SplashScreens,
        TransitionHelper
    }

    private enum Images
    {
        Image_Background,
    }

    private const float startDelay = 0.15f;

    private SplashScreenTitle[] splashScreenTitles;
    private UIDissolveEffect transitionHelper;

    protected override void Awake()
    {
        base.Awake();
        BindObjects(typeof(GameObjects));
        BindImages(typeof(Images));

        splashScreenTitles = GetObject((int)GameObjects.SplashScreens).FindChilds<SplashScreenTitle>();
        transitionHelper = GetObject((int)GameObjects.TransitionHelper).GetComponent<UIDissolveEffect>();
    }

    public void EnableSplashScreen()
    {
        PManagers.Instance.input.GameUI.ESC.Disable();

        GetObject((int)GameObjects.SplashScreens).SetActive(true);
        GetObject((int)GameObjects.TransitionHelper).SetActive(false);
        GetImage((int)Images.Image_Background).enabled = true;

        foreach (Transform child in GetObject((int)GameObjects.SplashScreens).transform)
            child.gameObject.SetActive(false);

        StartCoroutine(splashScreenTitles.Length != 0 ? nameof(CorEnableSplashScreen) : nameof(DisableSplashScreen));
    }

    private IEnumerator CorEnableSplashScreen()
    {
        int currentTitleIndex = 0;
        while (currentTitleIndex < splashScreenTitles.Length)
        {
            GameObject currentTitle = splashScreenTitles[currentTitleIndex].gameObject;
            yield return new WaitForSeconds(startDelay);
            currentTitle.SetActive(true);
            float duration = splashScreenTitles[currentTitleIndex].screenTime;
            yield return new WaitForSeconds(duration);
            currentTitle.SetActive(false);
            currentTitleIndex++;
        }
        yield return DisableSplashScreen();
    }

    private IEnumerator DisableSplashScreen()
    {
        GetObject((int)GameObjects.SplashScreens).SetActive(false);
        GetImage((int)Images.Image_Background).enabled = false;
        transitionHelper.gameObject.SetActive(true);
        transitionHelper.location = 0;
        transitionHelper.DissolveOut();
        yield return new WaitForSeconds(transitionHelper.animationSpeed);
        PManagers.DarkUI.ClosePopupUI(this);
    }

    public override IEnumerator CorClosePopupUI()
    {
        PManagers.Instance.input.GameUI.ESC.Enable();
        gameObject.SetActive(false);
        yield return null;
    }
}
