using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
    public struct Inner : IDialogue, ILog
    {
        [SerializeField] private int _next;
        [SerializeField] private int _background;
        [DictionaryDrawerSettings(KeyLabel = "Locale", ValueLabel = "Context")]
        [SerializeField] private Dictionary<string, string> _context;

        public Inner(int next, int background)
        {
            _next = next;
            _background = background;
            _context = new();
        }
        
        public void AddContext(string locale, string context) => _context.TryAdd(locale, context);
        
        public void Get(DialogueView view, Queue<Action> commands)
        {
            var inner = this;
            commands.Enqueue(SoundManager.Instance.PlayVoice);
            commands.Enqueue(() => view.ChangeDialogueType(EDialogueType.Inner));
            commands.Enqueue(() => view.SetScript(inner._context[DataManager.Instance.LocaleSetting]));
            commands.Enqueue(() => view.RequestJump(inner._next));
            commands.Enqueue(() => inner.CreateLog(view));
            view.SetBackgroundAndPlay(_background);
        }

        public void CreateLog(DialogueView view)
        {
            view.CreateMonoLog(_context);
        }

        public bool CreateLogJump(DialogueView view, out int index)
        {
            view.CreateMonoLog(_context);
            index = _next;
            return _next != -1;
        }
    }