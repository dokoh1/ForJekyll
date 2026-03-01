using System.Collections.Generic;
using UnityEngine;

public class TimeLiner : SingletonManager<TimeLiner>
{
    private TimeLine currentTimeLine;
    public int CurrentIndex => currentTimeLine.CurrentIndex;

    public IDialogue CurrentDialogue()
    {
        return  currentTimeLine.ReturnData();
    }
    
    public Dialogue CurrentDialogueType { get; set; } = Dialogue.Main;

    [Header("스토리 데이터")]
    [SerializeField] private ScriptData scriptData;
    
    public void ChangeTimeline(Dialogue type, string nameKey)
    {
        CurrentDialogueType = type;
        
        switch (type)
        {
            case Dialogue.Main : currentTimeLine = scriptData.MainTimeline(DataManager.Instance.Chapter);
                break;
            case Dialogue.Interaction : currentTimeLine = scriptData.InteractionTimeline;
                break;
            case Dialogue.Favor :
                if (nameKey != null)
                    currentTimeLine = scriptData.FavorTimeline(nameKey);
                break;
        }
    }

    public void LoadIndex(int _index) => currentTimeLine.MoveToIndex(_index);

    public void SkipData() => currentTimeLine.SetSkipIndex();

    public void LoadPrevLogs()
    {
        UIManager.Instance.view.ClearLog();
        scriptData.CreatePrevLog();
    }
}

public enum Dialogue
{
    Main,
    Favor,
    Interaction
}