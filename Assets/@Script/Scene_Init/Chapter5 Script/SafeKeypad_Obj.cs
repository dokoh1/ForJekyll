using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class SafeKeypad_Obj : MonoBehaviour
{
    [SerializeField] private List<int> kjmBirthDay = new() { 0, 9, 0, 6 }; 
    [SerializeField] private List<int> curKeyPadNum = new();
    [SerializeField] public List<KeypadBtn> keypadBtns;

    private Sequence _sequence;
    
    [SerializeField] private Renderer successRenderer;
    [SerializeField] private Renderer failRenderer;
    [SerializeField] private Material successMat;
    [SerializeField] private Material failMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    
    [SerializeField] private SafeDoor_Obj safeDoorObj;

    public void AddNum(int num)
    {
        curKeyPadNum.Add(num);
        if (curKeyPadNum.Count == 4)
        {
            foreach (var b in keypadBtns)
            {
                b.ButtonInteractToggle(false);
            }
            CheckNum();
        }
    }

    private void CheckNum()
    {
        for (var i = 0; i < curKeyPadNum.Count; i++)
        {
            if (curKeyPadNum[i] != kjmBirthDay[i])
            {
                DoorInteraction(false);
                break;
            }

            DoorInteraction(true);
        }
    }

    private void DoorInteraction(bool value)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        
        _sequence.AppendInterval(1f);
        _sequence.AppendCallback(() => MaterialChange(value, 0));
        _sequence.AppendCallback(() => SoundManager.Instance.PlaySE(value ? Obj_Sound.SafeSuccess : Obj_Sound.SafeFailure));
        _sequence.AppendInterval(1f);
        _sequence.AppendCallback(() => MaterialChange(value, 1)).onComplete += () =>
        {
            ReGame(value);
        };
        _sequence.Play();
    }
    private void ReGame(bool value)
    {
        if (value) { safeDoorObj.Interact(); }
        else
        {
            curKeyPadNum.Clear();
            foreach (var b in keypadBtns)
            {
                b.ButtonInteractToggle(false);
            }
        }
    }
    private void MaterialChange(bool value, int num)
    {
        Material targetMaterial;

        var targetRenderer = value ? successRenderer : failRenderer;
        if (num == 0)
        {
            targetMaterial = value ? successMat : failMat;
        }
        else
        {
            targetMaterial = value ? greenMat : redMat;
        }

        var materials = targetRenderer.materials;
        materials[0] = targetMaterial;
        targetRenderer.materials = materials;
    }
}
