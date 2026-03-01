using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    private DialoguePresenter presenter;

    private DialoguePresenter Presenter
    {
        get
        {
            if (presenter == null)
                presenter = TimeLiner.Instance.GetComponent<DialoguePresenter>();
            return presenter;
        }
    }
    
    #region 다이얼로그 요소 필드
    [Header("텍스트 설정")]
    [SerializeField] private SerializableDictionary<bool,TMP_Text> speaker;
    [SerializeField] private SerializableDictionary<bool,TMP_Text> context;
    [Header("일러스트 이미지"), SerializeField] private IllustView illustView;
    [Header("스킵 버튼"), SerializeField] private Button skipButton;
    [SerializeField] private Button logButton;
    [Header("선택지"), SerializeField] private GameObject SelectPanel;
    [SerializeField] private GameObject SelectPrefab;
    [Header("로그"), SerializeField] private GameObject logPanel;
    [SerializeField] private Transform logViewPort;
    [SerializeField] private ScrollRect logScroll;
    [SerializeField] private SerializableDictionary<EDialogueType, GameObject> logPrefab;
    [SerializeField] private List<GameObject> logs;
    #endregion

    #region 2D 대화창 오브젝트 필드
    [Header("대화창 오브젝트")] 
    [SerializeField] private SerializableDictionary<bool, GameObject> dialogue;
    [SerializeField] private Image background;
    [SerializeField] private Image voiceButton;
    [SerializeField] private TMP_Text autoButton;
    [SerializeField] private SerializableDictionary<bool, Sprite> voiceImage;
    #endregion
    
    #region 떨림 효과 프로퍼티
    [Header("떨림 효과")]
    [SerializeField] private RectTransform shakeTransform;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;
    [SerializeField, Range (0, 100)] private float shakeRandomness;
    #endregion
    
    #region PP 또는 Shader
    [Header("Post Processing Profile")]
    [SerializeField] private VolumeProfile volumeProfile;
    
    [Header("Dialogue Transition Material")]
    [SerializeField] private Material material;
    private const string MATERIAL_PROPERTY_NAME = "_FadeAmount";
    #endregion

    #region 기타 변수
    private bool onDialogue = false;
    private bool backgroundAval = false;
    private bool dotextEnd = true;
    private bool auto = false;
    private bool autoPlaying = false;
    private bool mode_2D = false;
    private bool clickable = false;
    private bool skipable = false;
    private bool voice = true;
    private TweenerCore<string, string, StringOptions> tween;
    private string selectLog;
    private float autoInterval;
    private float doTextInterval;
    private int currentDialogueType = 0;
    private int currentBg = -1;
    private List<GameObject> selectList = new();

    private CancellationTokenSource clickCancel;
    private CancellationTokenSource skipCancel;
    private Coroutine autoCoroutine;
    private WaitForSeconds interval = new(1.5f);
    private Sequence innerSequence;
    private Sequence bgSequence;
    private Queue<Action> commandQueue = new();

    private Color grey = new Color(200f / 255f, 200f / 255f, 200f / 255f, 128f / 255f);
    #endregion
    
    /// <summary>
    /// summary 기준 위 : Inspector 필드, 변수
    /// summary 기준 아래 : 메서드
    /// </summary>

    #region 생명주기 함수
    private void OnEnable()
    {
        UIManager.Instance.dialogueEnd += illustView.CloseAllImage;
    }

    private void OnDisable()
    {
        skipCancel?.Cancel();
        onDialogue = false;
    }
    #endregion
    
    #region 다이얼로그 호출 메서드
    public void OpenDialogue(Dialogue type, bool is2D, bool _auto, int _index, string nameKey)
    {
        material.SetFloat(MATERIAL_PROPERTY_NAME, 0f);
        illustView.CloseAllImage();
        DeleteCoroutine();
        onDialogue = true;
        backgroundAval = false;
        mode_2D = is2D;
        auto = _auto;
        autoButton.color = auto ? Color.white : grey;
        Presenter.ChangeTimeline(type, nameKey);
        Presenter.JumpData(_index);
        SkipInterval();
        Presenter.RequestData(commandQueue);
        ClickInterval();
        voiceButton.sprite = voiceImage[voice];
        backgroundAval = true;
        logPanel.SetActive(false);
        logButton.interactable = type == Dialogue.Main;
        gameObject.SetActive(true);
        dialogue[is2D].gameObject.SetActive(true);
        dialogue[!is2D].gameObject.SetActive(false);
    }

    public void CloseDialogue()
    {
        skipCancel?.Cancel();
        if (autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
            autoCoroutine = null;
        }
        onDialogue = false;
        UIManager.Instance.DialogueClose();
    }
    #endregion

    #region 다이얼로그 기능 메서드(다이얼로그 클래스용)
    public void RequestJump(int index)
    {
        if (index == -1)
            return;
        
        Presenter.JumpData(index);
    }
    
    public void RequestNext() => Presenter.RequestData(commandQueue);
    #endregion

    #region UI 이벤트
    public void OnClick(InputAction.CallbackContext value)
    {
        if (ScenarioManager.Instance.GetAchieve(PlayerAchieve.EscMenu)) return; // 임시
        if (logPanel.activeInHierarchy || UIClicked() || !mode_2D || !clickable || bgSequence.IsPlaying())
            return;
        
        if (dotextEnd)
        {
            if (autoCoroutine != null)
            {
                StopCoroutine(autoCoroutine);
                autoCoroutine = null;
                autoPlaying = false;
            }
            
            SoundManager.Instance.StopVoice();
            Presenter.RequestData(commandQueue);
            ClickInterval();
        }
        else
        {
            tween.Complete();
        }
    }
    
    public void OnSkip(InputAction.CallbackContext value) => SkipButtonClick();
    
    public void SkipButtonClick()
    {
        if (logPanel.activeInHierarchy || !skipable || currentDialogueType is (4 or 32))
            return;
        
        skipButton.interactable = false;
        
        if (auto && autoPlaying)
        {
            StopCoroutine(autoCoroutine);
            autoCoroutine = null;
            autoPlaying = false;
        }
        
        SoundManager.Instance.StopVoice();
        Presenter.RequestSkip();
        Presenter.RequestData(commandQueue);
    }

    public void AutoButtonClick()
    {
        auto = !auto;
        autoButton.color = auto ? Color.white : grey;

        if (auto)
        {
            if (dotextEnd && gameObject.activeInHierarchy)
            {
                autoCoroutine = StartCoroutine(autoPlay()); 
            }
            else
            {
                tween?.OnComplete(() =>
                {
                    dotextEnd = true;
                    if (auto && gameObject.activeInHierarchy)
                    {
                        autoCoroutine = StartCoroutine(autoPlay());
                    }
                });
            }
        }
        else
        {
            if (autoCoroutine != null)
            {
                StopCoroutine(autoCoroutine);
                autoCoroutine = null;
            }
        }
    }

    public void VoiceButtonClick()
    {
        voice = !voice;
        voiceButton.sprite = voiceImage[voice];
        if (SoundManager.Instance.Voice_Source == null)
            SoundManager.Instance.Voice_Source = GetComponent<AudioSource>();
        SoundManager.Instance.Voice_Source.volume = voice ? 1f : 0f;
    }

    public void LogButtonClick()
    {
        logPanel.SetActive(!logPanel.activeInHierarchy);
        logScroll.verticalNormalizedPosition = 0f;
    }
    #endregion
    
    #region 다이얼로그 요소 메서드
    public void SetContext(string _speaker, string _context, Color color)
    {
        speaker[mode_2D].text = _speaker;
        speaker[mode_2D].color = color;
        PlayDoText(_context);
    }

    public void SetScript(string _context)
    {
        illustView.FadeAllImage();
        PlayDoText(_context);
    }
    
    public void AddSelect(string context, Action onClick)
    {
        dotextEnd = false;
        
        GameObject go = Instantiate(SelectPrefab, SelectPanel.transform);
        selectList.Add(go);
        
        Text selectText = go.GetComponentInChildren<Text>();
        Button selectButton = go.GetComponentInChildren<Button>();
        
        selectText.text = context;
        selectButton.onClick.AddListener(onClick.Invoke);
        selectButton.onClick.AddListener(() => SkipInterval());
    }
    
    public void DestroySelect()
    {
        foreach (var select in selectList)
            Destroy(select);
        selectList.Clear();
    }

    public void SetBackgroundAndPlay(int key)
    {
        bgSequence?.Kill();
        bgSequence = DOTween.Sequence();
        
        if (key != -1 && key != currentBg && mode_2D)
        {
            currentBg = key;
            if (backgroundAval)
            {
                material.SetFloat(MATERIAL_PROPERTY_NAME, 0);
                bgSequence.Append(DOTween.To(() => material.GetFloat(MATERIAL_PROPERTY_NAME),
                    x => material.SetFloat(MATERIAL_PROPERTY_NAME, x),
                    1f,
                    1f));
                bgSequence.AppendCallback(() =>
                {
                    context[true].gameObject.SetActive(false);
                    speaker[true].gameObject.SetActive(false);
                });
            }
            if (DataManager.Instance.GetBackground(key, out Sprite bg))
            {
                bgSequence.AppendCallback(() => background.sprite = bg);
            }
            if (backgroundAval)
                bgSequence.Append(DOTween.To(() => material.GetFloat(MATERIAL_PROPERTY_NAME),
                    x => material.SetFloat(MATERIAL_PROPERTY_NAME, x),
                    0f,
                    1f));
        }
        
        bgSequence.AppendCallback(() =>
        {
            commandQueue.ForEach(x => x?.Invoke());
        });
        bgSequence.Play();
    }

    public void SetImage(string npcName, string faceKey)
    {
        if (mode_2D && faceKey != null)
        {
            illustView.SetImage(npcName, faceKey);
        }
    }

    public void CreateMonoLog(Dictionary<string,string> _context)
    {
        if (TimeLiner.Instance.CurrentDialogueType != Dialogue.Main)
            return;
        
        GameObject go = Instantiate(logPrefab[EDialogueType.Monologue], logViewPort);
        logs.Add(go);
        go.GetComponent<MonlogueLog>().SetData(_context);
    }

    public void CreateConLog(Dictionary<string,string> _speaker, Dictionary<string, string> _context)
    {
        if (TimeLiner.Instance.CurrentDialogueType != Dialogue.Main)
            return;
        
        GameObject go = Instantiate(logPrefab[EDialogueType.Conversation], logViewPort);
        logs.Add(go);
        go.GetComponent<ConversationLog>().SetData(_speaker, _context);
    }
    
    public void CreateEndLog()
    {
        if (TimeLiner.Instance.CurrentDialogueType != Dialogue.Main)
            return;
        
        GameObject go = Instantiate(logPrefab[EDialogueType.End], logViewPort);
        logs.Add(go);
    }

    public void RefreshLog()
    {
        if (logPanel.activeInHierarchy)
        {
            logPanel.SetActive(false);
            logPanel.SetActive(true);
        }
    }

    public void ClearLog() => logs.ForEach(Destroy);
    #endregion

    #region 다이얼로그 효과
    private void PlayDoText(string _context)
    {
        context[mode_2D].text = null;
        dotextEnd = false;
        
        if (CheckCustomTag(_context, out string result))
        {
            _context = result;
            ShakeEffect();
        }
        
        tween = context[mode_2D].DOText(_context, TextLength(_context), true, ScrambleMode.None).SetEase(Ease.Linear);
        tween.OnComplete(() =>
        {
            dotextEnd = true;
            if (auto && gameObject.activeInHierarchy)
            {
                autoCoroutine = StartCoroutine(autoPlay());
            }
        });
    }

    private bool CheckCustomTag(string _context, out string _result)
    {
        _result = null;
        
        if (_context.Contains("<shake>"))
        {
            StringBuilder removed = new();
            removed.Append(_context);
            removed.Replace("<shake>", "");
            _result = removed.ToString();
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private bool GetColorAdjustment(out ColorAdjustments result) => volumeProfile.TryGet<ColorAdjustments>(out result);
    
    private bool GetVignette(out Vignette result) => volumeProfile.TryGet<Vignette>(out result);
    
    private bool GetChromaticAberration(out ChromaticAberration result) => volumeProfile.TryGet<ChromaticAberration>(out result);
    
    private bool GetFilmGrain(out FilmGrain result) => volumeProfile.TryGet<FilmGrain>(out result);
    
    private void SetInnerEffect()
    {
        if (GetColorAdjustment(out var color) && GetVignette(out var vignette) && GetChromaticAberration(out var chromaticAberration)
            && GetFilmGrain(out var filmGrain))
        {
            color.saturation.value = 0;
            vignette.intensity.value = 0;
            chromaticAberration.intensity.value = 0;

            innerSequence?.Kill();
            innerSequence = DOTween.Sequence();
            innerSequence.Append(
                    DOTween.To(() => color.saturation.value,
                        x => color.saturation.value = x,
                        -100, 1.5f))
                .Join(
                    DOTween.To(() => vignette.intensity.value,
                        x => vignette.intensity.value = x,
                        0.4f, 1.5f))
                .Join(
                    DOTween.To(() => chromaticAberration.intensity.value,
                        x => chromaticAberration.intensity.value = x,
                        0.25f, 1.5f))
                .Join(
                    DOTween.To(() => filmGrain.intensity.value,
                        x => filmGrain.intensity.value = x,
                        1f, 1.5f));
            innerSequence.Play();
        }
    }

    private void RemoveInnerEffect()
    {
        if (GetColorAdjustment(out var color) && GetVignette(out var vignette) &&
            GetChromaticAberration(out var chromaticAberration) && GetFilmGrain(out var filmGrain))
        {
            innerSequence?.Kill();
            innerSequence = DOTween.Sequence();
            innerSequence.Append(
                    DOTween.To(() => color.saturation.value,
                        x => color.saturation.value = x,
                        0, 1.5f))
                .Join(
                    DOTween.To(() => vignette.intensity.value,
                        x => vignette.intensity.value = x,
                        0, 1.5f))
                .Join(
                    DOTween.To(() => chromaticAberration.intensity.value,
                        x => chromaticAberration.intensity.value = x,
                        0, 1.5f))
                .Join(
                    DOTween.To(() => filmGrain.intensity.value,
                        x => filmGrain.intensity.value = x,
                        0, 1.5f));
            innerSequence.Play();
        }
    }

    private void ShakeEffect()
    {
        shakeTransform.DOKill();
        shakeTransform.anchoredPosition = Vector2.zero;
        
        shakeTransform.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness)
            .OnComplete(() => shakeTransform.anchoredPosition = Vector2.zero);
    }

    #endregion
    
    #region Inverval, Coroutines
    private void DeleteCoroutine()
    {
        if (!onDialogue)
            return;
        
        if (autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
            autoCoroutine = null;
        }
        
        skipCancel?.Cancel();
        clickCancel?.Cancel();
        
        onDialogue = false;
        autoPlaying = false;
            
        SoundManager.Instance.StopVoice();
        Presenter.RequestSkip();
    }
    
    private async Awaitable ClickInterval()
    {
        clickCancel?.Dispose();
        clickCancel = new();
        clickable = false;
        await Awaitable.WaitForSecondsAsync(0.2f, clickCancel.Token);
        clickable = true;
    }

    private async Awaitable SkipInterval()
    {
        skipCancel?.Dispose();
        skipCancel = new();
        skipable = false;
        skipButton.interactable = false;
        await Awaitable.WaitForSecondsAsync(1.0f, skipCancel.Token);
        skipable = true;
        skipButton.interactable = true;
    }
    
    private IEnumerator autoPlay()
    {
        autoPlaying = true;

        while (SoundManager.Instance.Voice_Playing)
            yield return null;
        
        yield return interval;

        context[mode_2D].text = null;
        Presenter.RequestData(commandQueue);
        autoPlaying = false;
    }
    #endregion

    #region 기타

    public bool CheckLogPanel()
    {
        if (logPanel.activeInHierarchy)
        {
            logPanel.SetActive(false);
            return true;
        }

        return false;
    }
    
    public void HandleSetting(Settings settings)
    {
        if (mode_2D)
            context[mode_2D].fontSize = (int)settings.FontSize;
        doTextInterval = settings.PlaySpeed;
    }

    private float TextLength(string text)
    {
        float length = 0;
        bool inTag = false;

        foreach (var ch in text)
        {
            switch (ch)
            {
                case '<' : inTag = true;
                    continue;
                case '>' : inTag = false;
                    continue;
                default :
                    if (!inTag) length += (0.05f*doTextInterval);
                    continue;
            }
        }
        
        return length;
    }

    public void ChangeDialogueType(EDialogueType type)
    {
        if ((currentDialogueType & (int)type) != 0)
            return;
        
        currentDialogueType = (int)type;

        switch ((int)type)
        {
            case 1: UiActive(false, true, false);
                RemoveInnerEffect();
                break;
            case 2: UiActive(true, true, false);
                RemoveInnerEffect();
                break;
            case 4: UiActive(false, false, true);
                RemoveInnerEffect();
                break;
            case 16: UiActive(false, true, false);
                SetInnerEffect();
                break;
            case 32: RemoveInnerEffect();
                break;
        }
    }

    private void UiActive(bool _speaker, bool _context, bool select)
    {
        speaker[mode_2D].gameObject.SetActive(_speaker);
        context[mode_2D].gameObject.SetActive(_context);
        SelectPanel.gameObject.SetActive(select);
    }

    private bool UIClicked()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("UIElement"))
                return true;
        }

        return false;
    }
    #endregion
}
