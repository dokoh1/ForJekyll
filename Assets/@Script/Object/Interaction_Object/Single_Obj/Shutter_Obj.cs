using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class Shutter_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; }
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private Material highLight;
    [SerializeField] private Material changeMaterial;
    [SerializeField] private PlayableDirector shutterCutScene;

    [SerializeField] private DOTweenAnimation downAnimation;

    [field: Header("Sound")]
    [SerializeField] private AudioClip[] shakeSounds;
    [SerializeField] private AudioClip downSound;
    private AudioSource _audioSource;
    [SerializeField] private AudioSource _monsterAS;


    private void Awake()
    {
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        IsInteractable = false;

        UIManager.Instance.DialogueClose();
        UIManager.Instance.objective.ClearAll();
        GameManager.Instance.fadeManager.fadeComplete += TimeLineStart;
        StartCoroutine(GameManager.Instance.fadeManager.FadeStart(FadeState.FadeOut));
        GameManager.Instance.HighLightMaterialDelete(GetComponent<Renderer>(), changeMaterial);
        TimeLineStart();
    }

    private void FadeIn()
    {
 
    }
    private void TimeLineStart()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
        StartCoroutine(GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn, 0.5f));
        _monsterAS.Stop();
        shutterCutScene.gameObject.SetActive(true);
        //OnEvent(InteractEventType.Off);
    }
    public void MaterialChange()
    {
        var renderer = GetComponent<Renderer>();
        var materials = renderer.materials;
        materials[1] = highLight;
        renderer.materials = materials;
    }
    public void DownAnimation()
    {
        downAnimation.DOKill();
        downAnimation.CreateTween(true);
        PManagers.Sound.Play(ESound.SFX, downSound);
    }

    public void MoveScene()
    {
        UIManager.Instance.questUI.QuestClear(true);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
        DataManager.Instance.Chapter = 1;
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Tutorial, true);
        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Chapter1_1);
    }

    public void PlayShakeSound()
    {
        if (_audioSource == null || shakeSounds == null || shakeSounds.Length == 0)
            return;
        if (_audioSource.isPlaying) _audioSource.Stop();

        int index = Random.Range(0, shakeSounds.Length);

        _audioSource.clip = shakeSounds[index];
        _audioSource.Play();
    }

    void OnDisable()
    {
        if (_audioSource.isPlaying) _audioSource.Stop();
    }
}