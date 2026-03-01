using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class GeneratorPuzzle : MonoBehaviour
{
    [SerializeField] private Light[] redLights;
    [SerializeField] private Light[] greenLights;
    [SerializeField] private GeneratorSwitch_Obj[] generatorSwitchObjs;
    [field: SerializeField] public bool CanInteract { get; set; } = false;

    [Title("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip machineSound;

    public event Action onClear;
    public void SwitchToggle(int num)
    {
        switch (num)
        {
            case 1:
                redLights[1].enabled = !redLights[1].enabled;
                redLights[2].enabled = !redLights[2].enabled;
                greenLights[1].enabled = !greenLights[1].enabled;
                greenLights[2].enabled = !greenLights[2].enabled;
                break;
            case 2:
                greenLights[2].enabled = !greenLights[2].enabled;
                greenLights[3].enabled = !greenLights[3].enabled;
                redLights[2].enabled = !redLights[2].enabled;
                redLights[3].enabled = !redLights[3].enabled;
                break;
            case 3:
                redLights[0].enabled = !redLights[0].enabled;
                redLights[2].enabled = !redLights[2].enabled;
                redLights[3].enabled = !redLights[3].enabled;
                greenLights[0].enabled = !greenLights[0].enabled;
                greenLights[2].enabled = !greenLights[2].enabled;
                greenLights[3].enabled = !greenLights[3].enabled;
                break;
            case 4:
                redLights[0].enabled = !redLights[0].enabled;
                redLights[1].enabled = !redLights[1].enabled;
                greenLights[0].enabled = !greenLights[0].enabled;
                greenLights[1].enabled = !greenLights[1].enabled;
                break;
        }

        if (greenLights.Any(l => !l.enabled))
        {
            return;
        }

        PuzzleClear();
    }

    public void PuzzleStart()
    {
        Debug.Log("PuzzleStart");
        foreach (var l in redLights) { l.enabled = true; }
        foreach (var s in generatorSwitchObjs) { s.Init(this); }
    }

    private void PuzzleClear()
    {
        Debug.Log("PuzzleClear");
        UIManager.Instance.dialogueEnd += onClear;
        foreach (var s in generatorSwitchObjs) { s.IsInteractable = false; }

        audioSource.clip = machineSound;
        audioSource.Play();
        audioSource.DOFade(1, 1f);

        var seq = DOTween.Sequence();
        seq.AppendInterval(2f);
        seq.AppendCallback(() => SoundManager.Instance.PlaySE(Obj_Sound.FireFlag, 0.01f));
        // check
        // seq.AppendCallback(() => UIManager.Instance.DialogueOpen(Dialogue.Main, false, 254));
        seq.AppendCallback(() => UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 143));
        seq.Play();
    }
}