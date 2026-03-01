using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
    public struct Conversation : IDialogue, ILog
    {
        [SerializeField] private string _face;
        [SerializeField] private int _next;
        [SerializeField] private int _background;
        [DictionaryDrawerSettings(KeyLabel = "Locale", ValueLabel = "Speaker")]
        [SerializeField] private Dictionary<string, string> _speaker;
        [DictionaryDrawerSettings(KeyLabel = "Locale", ValueLabel = "Context")]
        [SerializeField] private Dictionary<string, string> _context;
        
        public Conversation(string face, int next, int background)
        {
            _face = face;
            _next = next;
            _background = background;
            _speaker = new();
            _context = new();
        }

        public void AddSpeakerContext(string locale, string speaker, string context)
        {
            _speaker.TryAdd(locale, speaker);
            _context.TryAdd(locale, context);
        }

        public void Get(DialogueView view, Queue<Action> commands)
        {
            var conversation = this;
            commands.Enqueue(SoundManager.Instance.PlayVoice);
            commands.Enqueue(() => view.ChangeDialogueType(EDialogueType.Conversation));
            commands.Enqueue(() => view.SetContext(conversation._speaker[DataManager.Instance.LocaleSetting], conversation._context[DataManager.Instance.LocaleSetting], DataManager.Instance.GetNPCColor(conversation._speaker["ko"])));
            commands.Enqueue((() => view.SetImage(conversation._speaker["ko"], conversation._face)));
            commands.Enqueue(() => view.RequestJump(conversation._next));
            commands.Enqueue(() => conversation.CreateLog(view));
            view.SetBackgroundAndPlay(_background);
        }

        public void CreateLog(DialogueView view)
        {
            view.CreateConLog(_speaker, _context);
        }

        public bool CreateLogJump(DialogueView view, out int index)
        {
            view.CreateConLog(_speaker, _context);
            index = _next;
            return _next != -1;
        }
    }