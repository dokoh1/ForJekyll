using System;
using UnityEngine;
public class DistributionBox_Obj : InteractableBase
{
    // 탭이 건전지 삽입, 홀드 상호 작용

    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.TapAndHold;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = false;
    [field: SerializeField] public override float InteractHoldTime { get; set; } = 1f;

    [SerializeField] private Door_Obj doorObj;
    private void Awake() { doorObj.endOpen += InteractRegister; }
    private void InteractRegister() { IsInteractable = true; }
        
    [SerializeField] private bool brokenBox;
    [SerializeField] private GameObject[] particle;
    
    [SerializeField] private int curBattery = 0;
    [SerializeField] private GameObject[] battery;
    
    [SerializeField] private GeneratorPuzzle generatorPuzzle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip electric;
    [SerializeField] private AudioClip pushBattery;

    [Header("Distribution Box Type")]
    [SerializeField] private DistributionBoxType boxType;
    public enum DistributionBoxType { B1, B2 }
    public event Action<DistributionBoxType> CheckBox;


    public override void Interact()
    {
        if (brokenBox)
        {
            foreach (var v in particle)
            {
                v.SetActive(true);
            }
            audioSource.loop = true;
            audioSource.clip = electric;
            audioSource.Play();
            OnEvent(InteractEventType.Off);
            IsInteractable = false;
            enabled = false;

            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 97);
            CheckBox?.Invoke(boxType);

            return;
        }
        
        //if (curBattery == 4) return;
        //
        //if (ParkingSceneManager.Instance.PlayerBatteryCount > 0)
        //{
        //    ParkingSceneManager.Instance.PlayerBatteryCount--;
        //    battery[curBattery].SetActive(true);
        //    curBattery++;
        //}
        //
        //if (curBattery != 4) return;
        generatorPuzzle.PuzzleStart();
        IsInteractable = false;
        CanTap = false;
        CanHold = false;
        OnEvent(InteractEventType.On);
        CheckBox?.Invoke(boxType);
    }

    public override void Hold()
    {
        if (curBattery == 0) return;
        curBattery--;
        battery[curBattery].SetActive(false);
        //GameSceneManager.Instance.PlayerBatteryCount++;
    }
}
