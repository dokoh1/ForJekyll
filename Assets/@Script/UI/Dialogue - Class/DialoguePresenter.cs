using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class DialoguePresenter : MonoBehaviour
{
    private TimeLiner timeLiner => TimeLiner.Instance;
    private DialogueView view => UIManager.Instance.view;

    private Coroutine auto;

    public void RequestData(Queue<Action> commands)
    {
        commands.Clear();
        timeLiner.CurrentDialogue().Get(view, commands);
    }

    public void JumpData(int _index)
    {
        timeLiner.LoadIndex(_index);
    }

    public void RequestSkip()
    {
        timeLiner.SkipData();
    }

    public void ChangeTimeline(Dialogue type, string nameKey) => timeLiner.ChangeTimeline(type, nameKey);
}
