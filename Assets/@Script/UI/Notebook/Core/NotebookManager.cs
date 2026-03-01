using UnityEngine;
using UnityEngine.UI;

public enum NotebookState
{
    Closed,
    Opening,
    Active,
    Switching,
    Closing
}

// 사용법
// NotebookManager.Instance.NotebookMap.UpdateMap("map_sample");
// NotebookManager.Instance.NotebookMission.UpdateMission("MoveOpposite2F");

public class NotebookManager : SingletonManager<NotebookManager>
{
    public NotebookState CurrentState { get; private set; } = NotebookState.Closed;

    [Header("UI Components")]
    [SerializeField] private NotebookUI notebookUI;
    public NotebookUI NotebookUI => notebookUI;
    [SerializeField] private NotebookAnimation notebookAnimation;
    public NotebookAnimation NotebookAnimation => notebookAnimation;

    [Header("Tab Container")]
    [SerializeField] private NotebookMission notebookMission;
    [SerializeField] private NotebookMap notebookMap;
    public NotebookMission NotebookMission => notebookMission;
    public NotebookMap NotebookMap => notebookMap;

    [Header("Game State")]
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("Karma Phase")]
    [SerializeField] private Image karmaImage;
    [SerializeField] private Sprite[] karmaBloodSprites;
    private int currentPhase;
    private int newPhase;

    private bool initialized = false;

    private void OnEnable()
    {
        DataManager.Instance.OnKarmaChanged += NotebookKarmaChanged;
    }

    private void OnDisable()
    {
        DataManager.Instance.OnKarmaChanged -= NotebookKarmaChanged;
    }

    public void RequestOpen(string tab)
    {
        if (CurrentState != NotebookState.Closed) return;
        ChangeState(NotebookState.Opening);

        PlayOpenAnimation(tab);
    }

    public void RequestClose()
    {
        if (CurrentState != NotebookState.Active) return;
        ChangeState(NotebookState.Closing);

        PlayCloseAnimation();
    }

    public void RequestTabSwitch(string tab)
    {
        if (CurrentState != NotebookState.Active) return;

        if (notebookUI.CurrentTabName == tab) return;
        ChangeState(NotebookState.Switching);

        PlayTabSwitchAnimation(tab);
    }

    private void ChangeState(NotebookState newState)
    {
        CurrentState = newState;

        if (CurrentState == NotebookState.Closed && isPaused)
        {
            ResumeGame();
            isPaused = false;
        }
        else if (CurrentState != NotebookState.Closed && !isPaused)
        {
            PauseGame();
            isPaused = true;
        }
    }

    private void PauseGame()
    {
        GameManager.Instance.IsPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ResumeGame()
    {
        GameManager.Instance.IsPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void PlayOpenAnimation(string tab)
    {
        notebookAnimation.PlayOpen(() =>
        {
            notebookUI.OpenNotebook(tab);

            PlayKarmaIfNeeded(() =>
            {
                ChangeState(NotebookState.Active);
            });
        });
    }

    private void PlayCloseAnimation()
    {
        notebookAnimation.PlayClose(() =>
        {
            notebookUI.CloseNotebook();
            ChangeState(NotebookState.Closed);
        });
    }

    private void PlayTabSwitchAnimation(string tab)
    {
        notebookAnimation.PlaySwitch(tab, () =>
        {
            notebookUI.OpenTab(tab);
            ChangeState(NotebookState.Active);
        });
    }

    private void PlayKarmaIfNeeded(System.Action onComplete = null)
    {
        if (newPhase > currentPhase)
        {
            int phaseToPlay = newPhase;
            notebookAnimation.PlayKarma(phaseToPlay, () =>
            {
                ChangeKarmaImage(phaseToPlay);
                currentPhase = phaseToPlay;
                onComplete?.Invoke();
            });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void CheckCurrentPhase()
    {
        bool phase1 = ScenarioManager.Instance.GetAchieve(ScenarioAchieve.KarmaUI_Phase1);
        bool phase2 = ScenarioManager.Instance.GetAchieve(ScenarioAchieve.KarmaUI_Phase2);
        bool phase3 = ScenarioManager.Instance.GetAchieve(ScenarioAchieve.KarmaUI_Phase3);

        if (phase3)
            currentPhase = 3;
        else if (phase2)
            currentPhase = 2;
        else if (phase1)
            currentPhase = 1;
        else
            currentPhase = 0;
    }

    private void NotebookKarmaChanged(int Karma)
    {
        if (Karma > 90)
            newPhase = 3;
        else if (Karma > 75)
            newPhase = 2;
        else if (Karma > 64)
            newPhase = 1;
        else
            newPhase = 0;


        if (newPhase > currentPhase)
        {
            switch (newPhase)
            {
                case 1: ScenarioManager.Instance.SetAchieve(ScenarioAchieve.KarmaUI_Phase1, true); break;
                case 2: ScenarioManager.Instance.SetAchieve(ScenarioAchieve.KarmaUI_Phase2, true); break;
                case 3: ScenarioManager.Instance.SetAchieve(ScenarioAchieve.KarmaUI_Phase3, true); break;
            }
        }

        if (!initialized)
        {
            CheckCurrentPhase();
            if (currentPhase > 0)
                ChangeKarmaImage(currentPhase);

            initialized = true;
        }
    }

    public void ChangeKarmaImage(int phase)
    {
        if (phase <= 0 || phase > karmaBloodSprites.Length) return;
        karmaImage.sprite = karmaBloodSprites[phase - 1];

        Color c = karmaImage.color;
        c.a = Mathf.Clamp01(0.8f);
        karmaImage.color = c;
    }
}
