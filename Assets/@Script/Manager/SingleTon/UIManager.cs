using System;
using Marker;
using DG.Tweening;
using UnityEngine;

public class UIManager : SingletonManager<UIManager>
{
    [SerializeField] private DialogueView dialogueView;
    [SerializeField] private InputController input;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Canvas dialogueCanvas;
    
    [SerializeField] public ObjectiveIndicator objective;
    [SerializeField] public QuestUI questUI;

    public DialogueView view => dialogueView;
    
    private Camera mainCamera;

    private Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            return mainCamera;
        }
    }

    private bool init = false;

    public bool LogPanelOn => view.CheckLogPanel();

    public bool isInDialogue { get; private set; }
    public event Action dialogueEnd;
    public event Action DialogueStart;
    [SerializeField] private bool is2D;

    private void Awake()
    {
        base.Awake();
        input.Init();
        objective.SetCamera(MainCamera);
    }

    private void OnEnable()
    {
        if (!init)
        {
            AudioSource[] sources;
            sources = dialogueView.GetComponents<AudioSource>();
            SoundManager.Instance.Voice_Source = sources[0];
            SoundManager.Instance.DialogueSE_Source = sources[1];
            init = true;
        }
        
        input.UI.Click.started += dialogueView.OnClick;
        input.UI.Skip.started += dialogueView.OnSkip;
        DataManager.Instance.OnSetting += dialogueView.HandleSetting;
    }

    private void OnDisable()
    {
        input.UI.Click.started -= dialogueView.OnClick;
        input.UI.Skip.started -= dialogueView.OnSkip;
        DataManager.Instance.OnSetting -= dialogueView.HandleSetting;
    }

    private void Start() => DataManager.Instance.LoadSetting();
    
    public void DialogueOpen(Dialogue type, bool _2D, int _index, string nameKey = null)
    {
        isInDialogue = true;
        is2D = _2D;
        input.UIInputSwitch(true);
        
        
        if (is2D)
        {
            DialogueStart += () => dialogueView.OpenDialogue(type, _2D, !_2D, _index, nameKey);
            Fade(true);
        }
        else
        {
            dialogueView.OpenDialogue(type, _2D, !_2D, _index, nameKey);
        }
        
        dialogueCanvas.renderMode = is2D ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
    }

    public void DialogueClose()
    {
        input.UIInputSwitch(false);
        dialogueEnd?.Invoke();
        dialogueEnd = null;
        
        if (is2D)
        {
            Fade(false);
        }
        else
        {
            dialogueView.gameObject.SetActive(false);
            isInDialogue = false;
        }
        
        if (TimeLiner.Instance.CurrentDialogueType == Dialogue.Main) 
            DataManager.Instance.SaveMainIndex();
    }

    public void DialogueCloseImmediate()
    {
        input.UIInputSwitch(false);
        MainCamera.gameObject.SetActive(true);
        uiCamera.gameObject.SetActive(false);
        dialogueEnd?.Invoke();
        dialogueEnd = null;
        
        dialogueView.gameObject.SetActive(false);
        SoundManager.Instance.StopVoice();
        if (is2D) { GameManager.Instance.playerSettingManager?.SetState(PlayerAchieve.Dialogue, false); }
        
        if (!is2D) GameManager.Instance.playerSettingManager?.SetState(PlayerAchieve.Dialogue, false);
        input.GameUISwitch(true);
        isInDialogue = false;
    }

    private async Awaitable Fade(bool isOn)
    {
        await GameManager.Instance.fadeManager.FadeStart(FadeState.FadeOut, 1f);
        dialogueView.gameObject.SetActive(isOn);
        MainCamera.gameObject.SetActive(!isOn);
        uiCamera.gameObject.SetActive(isOn);
        DialogueStart?.Invoke();
        DialogueStart = null;
        if (is2D) { GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.Dialogue, isOn); }
        GameManager.Instance.fadeManager.fadeInComplete += () => fadeEnd(isOn);
        await GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn, 1f);
    }

    void fadeEnd(bool isOn)
    {
        if (isOn)
        {
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.Dialogue, true);
            GameManager.Instance.CursorVisible();
        }
        else
        {
            if (!is2D) GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.Dialogue, false);
            input.GameUISwitch(true);
            isInDialogue = false;
        }
        
    }
}