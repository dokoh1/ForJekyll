using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
    public struct End : IDialogue, ILog
    {
        [SerializeField] private int background;

        public End(int _background)
        {
            background = _background;
        }
        
        public void Get(DialogueView view, Queue<Action> commands)
        {
            var end = this;
            commands.Enqueue(() => view.ChangeDialogueType(EDialogueType.End));
            commands.Enqueue(() => end.CreateLog(view));
            commands.Enqueue(view.CloseDialogue);
            view.SetBackgroundAndPlay(background);
        }

        public void CreateLog(DialogueView view)
        {
            view.CreateEndLog();
        }

        public bool CreateLogJump(DialogueView view, out int index)
        {
            view.CreateEndLog();
            index = 0;
            return false;
        }
    }