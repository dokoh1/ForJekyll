using DG.Tweening;
using Marker;
using UnityEngine;

public class Chapter2_3_Manager
{
    private readonly Chapter2_3_SceneInitializer _ch23;
    private readonly SerializableDictionary<ParkingIndicatorState, QuestTarget> _questTargets;
    private readonly ColliderHandler _b1_7Collider;
    private readonly ColliderHandler _b2_1Collider;
    private bool _b1GeneratorObjectiveSet;
    private bool _b2GeneratorObjectiveSet;
    private bool _b1DistributionObjectiveSet;
    private bool _b2DistributionObjectiveSet;
    private bool _b1DiaryObjectiveSet;
    public Chapter2_3_Manager(
        Chapter2_3_SceneInitializer ch23,
        SerializableDictionary<ParkingIndicatorState, QuestTarget> questTargets,
        ColliderHandler b1_7Collider,
        ColliderHandler b2_1Collider)
    {
        _ch23 = ch23;
        _questTargets = questTargets;
        _b1_7Collider = b1_7Collider;
        _b2_1Collider = b2_1Collider;
    }

    public void EventInitialize(SerializableDictionary<ParkingEventType, ColliderHandler> eventHandlers)
    {
        foreach (var e in eventHandlers)
        {
            switch (e.Key)
            {
                case ParkingEventType.CarNoise:
                    e.Value.OnTriggerEntered += PlayCarNoiseEvent;
                    break;
                case ParkingEventType.EarTypeShow:
                    break;
                case ParkingEventType.WayPoint2:
                    e.Value.OnTriggerEntered += _ch23.LightFlicker;
                    break;
            }
        }
    }

    private void PlayCarNoiseEvent()
    {
        // GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.LookAndInteract, true);
        // UIManager.Instance.dialogueEnd += LookAndInteractFalse;
        var seq = DOTween.Sequence();

        _ch23.carNoise.PlayNoise(2f);
        seq.AppendInterval(2f);
        seq.AppendCallback(() => UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 104));
        seq.Play();
    }

    private void LookAndInteractFalse()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.LookAndInteract, false);
    }

    #region 오브젝티브 헬퍼

    private void SetObjective(
        string key,
        ParkingIndicatorState indicator,
        ObjectiveAction action,
        float radius = 1f,
        bool clearAll = true)
    {
        if (clearAll)
            UIManager.Instance.objective.ClearAll();

        UIManager.Instance.objective.SetMarkerByKey(key);
        UIManager.Instance.objective.Follow(
            _questTargets[indicator],
            action,
            radius);
    }

    #endregion

    #region 이벤트 처리 (콜라이더, 분전함 등)

    // B1 분전함 ON
    public void OnB1DistributionOn()
    {
        if (_b1DistributionObjectiveSet)
            return;
        _b1DistributionObjectiveSet = true;
        
        SetObjective("FindDiary", ParkingIndicatorState.B1_Diary, ObjectiveAction.PickUp);
        _ch23.clearWall.gameObject.SetActive(false);
        Debug.Log("B1 발전기 작동");
    }

    public void OnB1DiaryOn()
    {
        if (_b1DiaryObjectiveSet)
            return;
        _b1DiaryObjectiveSet = true;
        SetObjective("FindPowerRoom", ParkingIndicatorState.B1_7, ObjectiveAction.Follow);
        _b1_7Collider.OnTriggerEntered += OnB1Point7Trigger;
        _b2_1Collider.OnTriggerEntered += OnB2Point1Trigger;
    }

    // B2 분전함 ON
    public void OnB2DistributionOn()
    {
        if (_b2DistributionObjectiveSet)
            return;
        _b2DistributionObjectiveSet = true;
        SetObjective("Activate4Stabilizers", ParkingIndicatorState.B2_PuzzlePoint, ObjectiveAction.Push);
        _ch23.emerganceyDoor.isOpen = true;
        Debug.Log("B2 발전기 작동");
    }

    // B1_7 포인트 진입
    private void OnB1Point7Trigger()
    {
        SetObjective("FindPowerRoom", ParkingIndicatorState.B2_1, ObjectiveAction.Follow);
        _b1_7Collider.gameObject.SetActive(false);
    }

    // B2_1 포인트 진입
    private void OnB2Point1Trigger()
    {
        SetObjective("FindPowerRoom", ParkingIndicatorState.B2_PowerRoomPoint, ObjectiveAction.Push);
        _b2_1Collider.gameObject.SetActive(false);
    }

    // B1 발전실 문 열릴 때
    public void OnEnterB1GeneratorRoom()
    {
        if (_b1GeneratorObjectiveSet)
            return;
        _b1GeneratorObjectiveSet = true;
        SetObjective("ExcuteGeneration", ParkingIndicatorState.B1_GenerationPoint, ObjectiveAction.Push);
    }

    // B2 발전실 문 열림 (EnterB2GeneratorRoom의 오브젝티브 부분만)
    public void OnEnterB2GeneratorRoomObjective()
    {
        if (_b2GeneratorObjectiveSet)
            return;
        _b2GeneratorObjectiveSet = true;
        SetObjective("ExcuteGeneration", ParkingIndicatorState.B2_GenerationPoint, ObjectiveAction.PickUp);
    }

    // 키 도어 못 열었을 때
    public void OnFindKey()
    {
        SetObjective("FindKey", ParkingIndicatorState.FindKey, ObjectiveAction.Follow);
    }

    // 퍼즐 클리어 시 오브젝티브
    public void OnPuzzleClearObjective()
    {
        SetObjective("RunFireEscape", ParkingIndicatorState.B2_9, ObjectiveAction.Push);
    }

    #endregion
}