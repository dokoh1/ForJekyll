using System;
using System.Collections.Generic;

public interface IDialogue
{
    void Get(DialogueView view, Queue<Action> commands);
}
