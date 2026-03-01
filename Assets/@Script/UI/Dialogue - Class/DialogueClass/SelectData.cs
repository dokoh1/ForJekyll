
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class SelectData
    {
        [field: SerializeField] public int Chapter { get; set; }
        [field: SerializeField] public int Index { get; set; }
        [field: SerializeField] public ESelectType Type { get; set; }
        [field: SerializeField] public string Condition { get; set; }
        [field: SerializeField] public string Target { get; set; }
        [field: SerializeField] public int Value { get; set; }
        [field: SerializeField] public int Next { get; set; }

        [DictionaryDrawerSettings(KeyLabel = "Locale", ValueLabel = "Context")]
        [SerializeField, SerializeReference] public Dictionary<string, string> Context;

        public SelectData(int chapter, int index, ESelectType type,string condition, string target, int value, int next)
        {
            Chapter = chapter;
            Index = index;
            Type = type;
            Condition = condition;
            Target = target;
            Value = value;
            Next = next;
            Context = new();
        }

        public void AddContext(string locale, string context) => Context.TryAdd(locale, context);
    }