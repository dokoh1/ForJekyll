using System.Collections.Generic;
using System;
using System.Linq;

public class ScenarioManager : SingletonManager<ScenarioManager>
{
    public List<bool> scenarioArchieves;
    public List<bool> playerArchieves;

    public event Action<ScenarioAchieve, bool> OnSwitchChanged;
    public event Action<PlayerAchieve, bool> OnPlayerChanged;
    public bool readOnly = false;
    public bool escActive { get; set; } = true;

    protected override void Awake()
    {
        base.Awake();
        InitializeAchieves();
    }
    public void ResetScenarioFlags()
    {
        for (var i = 0; i < scenarioArchieves.Count; i++)
        {
            scenarioArchieves[i] = false;
        }
    }
    private void InitializeAchieves()
    {
        int switchCount = Enum.GetValues(typeof(ScenarioAchieve)).Length;
        if (scenarioArchieves == null || scenarioArchieves.Count != switchCount)
        {
            scenarioArchieves = new List<bool>(new bool[switchCount]);
        }

        int otherCount = Enum.GetValues(typeof(PlayerAchieve)).Length;
        if (playerArchieves == null || playerArchieves.Count != otherCount)
        {
            playerArchieves = new List<bool>(new bool[otherCount]);
        }
    }

    public void SetAchieve(ScenarioAchieve scenarioAchieveType, bool flag) // BOOL �� ����
    {
        // ���� switchStates ����Ʈ���� �ش� ����ġ�� ���¸� ������Ʈ
        int index = (int)scenarioAchieveType;
        if (index >= 0 && index < scenarioArchieves.Count)
        {
            scenarioArchieves[index] = flag; // ���� ����ġ ���� ������Ʈ

            OnSwitchChanged?.Invoke(scenarioAchieveType, flag);
        }
        else
        {
            return;
        }
    }

    public void SetAchieve(PlayerAchieve playerAchieve, bool flag) // BOOL �� ����
    {
        // ���� switchStates ����Ʈ���� �ش� ����ġ�� ���¸� ������Ʈ
        int index = (int)playerAchieve;
        if (index >= 0 && index < playerArchieves.Count)
        {
            playerArchieves[index] = flag; // ���� ����ġ ���� ������Ʈ

            OnPlayerChanged?.Invoke(playerAchieve, flag);
        }
        else
        {
            return;
        }
    }
    public bool GetAchieve(ScenarioAchieve scenarioAchieveType) // BOOL �� üũ
    {
        int index = (int)scenarioAchieveType;
        if (index >= 0 && index < scenarioArchieves.Count)
        {
            return scenarioArchieves[index];
        }
        return false;
    }

    public bool GetAchieve(PlayerAchieve playerAchieve) // BOOL �� üũ
    {
        int index = (int)playerAchieve;
        if (index >= 0 && index < playerArchieves.Count)
        {
            return playerArchieves[index];
        }
        return false;
    }

    #region Npc ��ȣ�ۿ�
    public void ResetNpcInteract()
    {
        foreach (var key in npcInteracts.Keys.ToList())
        {
            npcInteracts[key] = false;
        }
    }
    public void AddNpcInteract(NpcName npcName)
    {
        npcInteracts[npcName] = false;
    }
    public void DeleteNpcInteract()
    {
        npcInteracts.Clear();
    }
    public bool CheckNpcInteract()
    {
        foreach (var key in npcInteracts.Keys.ToList())
        {
            if (npcInteracts[key] == false)
            {
                return false;
            }
        }

        return true;
    }
    public void ResetNpcFavorInteract()
    {
        foreach (var key in npcFavorInteract.Keys.ToList())
        {
            npcFavorInteract[key] = false;
        }
    }

    public Dictionary<NpcName, bool> npcFavorInteract = new Dictionary<NpcName, bool>()
    {
        { NpcName.PSY, false},
        { NpcName.KJM, false},
        { NpcName.YHJ, false}
    };
    public Dictionary<NpcName, bool> npcInteracts = new Dictionary<NpcName, bool>()
    {
        {NpcName.PJH , false},
        {NpcName.HMS , false},
        {NpcName.PSY , false},
        {NpcName.JYS , false},
        {NpcName.KJM , false},
        {NpcName.YHJ , false},
    };
}
public enum NpcName
{
    PJH,
    KJM,
    PSY,
    HMS,
    JYS,
    YHJ
}

#endregion

public enum PlayerAchieve
{
    #region PlayerSetting
    PlayerMove,
    PlayerStop,
    Dialogue,
    Saving,
    LookAndInteract,
    OnlyLook,
    OnlyInteractLocked,
    CutScenePlaying,
    ObjectInteract,
    EscMenu,
    FlashLockOnlyLook
    #endregion
}
public enum ScenarioAchieve
{
    #region Scenario
    PlayerFlashLight,
    GasMask,
    SecurityOfficeKey,
    GeneratorRoomKey,
    #endregion

    #region NPC
    JYS_Dead,
    HMS_Dead,
    PSU_Dead,
    PJH_Dead,
    YHJ_Dead,

    PSU_Crazy,
    #endregion

    #region Karma
    KarmaUI_Phase1,
    KarmaUI_Phase2,
    KarmaUI_Phase3,
    #endregion

    #region Scene
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

    #endregion

    #region Chapter1 Script
    Chapter1_1_TalkAllEvent,
    Chapter1_2_WithYHJ,
    Chapter1_3_CafeEvent,
    #endregion

    #region Script
    BloodCapsule,
    Knife,
    #endregion

    #region Chapter3 Script
    haveCTR_RoomCard,
    #endregion

    #region Chapter4 Script
    HMS_MiddleBossEvent,
    #endregion

    #region SideStory
    JYS_SideStory,
    HMS_SideStory,
    PSU_SideStory,
    PJH_SideStory,
    YHJ_SideStory,
    KJM_SideStory,
    #endregion
}
