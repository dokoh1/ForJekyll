using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
    public struct Select : IDialogue
    {
        [SerializeField] private int background;
        [field: SerializeField, SerializeReference] public List<Dictionary<string, string>> Context { get; set; }
        [field: SerializeField] public List<ESelectType> Type { get; set; }
        [field: SerializeField] public List<string> Condition { get; set; }
        [field: SerializeField] public List<string> Target { get; set; }
        [field: SerializeField] public List<int> Value { get; set; }
        [field: SerializeField] public List<int> Next { get; set; }

        public Select(int _background, List<SelectData> data)
        {
            background = _background;
            Context = new();
            Type = new();
            Condition = new();
            Target = new();
            Value = new();
            Next = new();

            for (int i = 0; i < data.Count; i++)
            {
                Context.Add(data[i].Context);
                Type.Add(data[i].Type);
                Condition.Add(data[i].Condition);
                Target.Add(data[i].Target);
                Value.Add(data[i].Value);
                Next.Add(data[i].Next);
            }
        }
        
        public void Get(DialogueView view, Queue<Action> commands)
        {
            var sel = this;
            commands.Enqueue(view.DestroySelect);
            commands.Enqueue(() => sel.AddSelect(view));
            commands.Enqueue(() => view.ChangeDialogueType(EDialogueType.Select));
            view.SetBackgroundAndPlay(sel.background);
        }

        private void AddSelect(DialogueView view)
        {
            for (int i = 0; i < Context.Count; i++)
            {
                if (!DataManager.Instance.CheckCondition(Condition[i]))
                    continue;
                
                string target = Target[i];
                int next = Next[i];
                int value = Value[i];

                var select = this;
                var index = i;
                Action onClick = () => DataManager.Instance.AddSelectData(index);
                onClick += () => select.CreateLog(view, select.Context[index]);
                
                onClick += Type[i] switch
                {
                    ESelectType.Save => () => select.Save(view, next),
                    ESelectType.Kill => () => select.Kill(view, target, next),
                    ESelectType.Karma => () => select.Karma(view, value, next),
                    ESelectType.Favor => () => select.Favor(view, value, target, next),
                    ESelectType.Move => () => select.Move(view, value),
                    _ => () => select.Normal(view, next),
                };
                
                view.AddSelect(Context[i][DataManager.Instance.LocaleSetting], onClick);
            }
        }

        private void Save(DialogueView view, int next)
        {
            GameManager.Instance.fadeManager.fadeComplete += DataManager.Instance.UISavePanel.OpenPanel;
            view.RequestJump(next);
            view.RequestNext();
        }

        private void Kill(DialogueView view, string target, int next)
        {
            DataManager.Instance.NPC_Alive[target] = false;
            view.RequestJump(next);
            view.RequestNext();
        }

        private void Karma(DialogueView view, int value, int next)
        {
            DataManager.Instance.Karma += value;
            SoundManager.Instance.PlaySE(value >= 0 ? SE_Sound.KarmaUp : SE_Sound.KarmaDown);
            view.RequestJump(next);
            view.RequestNext();
        }

        private void Favor(DialogueView view, int value, string target, int next)
        {
            DataManager.Instance.NPC_Favor[target] += value;
            view.RequestJump(next);
            view.RequestNext();
        }
        
        private void Normal(DialogueView view, int next)
        {
            view.RequestJump(next);
            view.RequestNext();
        }

        private void Move(DialogueView view, int chapter)
        {
            switch (chapter)
            {
                case 5: ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter1_3, true); break;
                case 7: ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter2_2, true); break;
            }
            GameManager.Instance.fadeManager.fadeComplete += view.CloseDialogue;
            GameManager.Instance.fadeManager.MoveScene((SceneEnum)chapter);
            ScenarioManager.Instance.ResetNpcFavorInteract();
        }

        public void CreateLog(DialogueView view, Dictionary<string, string> _context)
        {
            view.CreateMonoLog(_context);
        }

        public int CreateLog(DialogueView view, int index)
        {
            view.CreateMonoLog(Context[index]);
            return Next[index];
        }
    }