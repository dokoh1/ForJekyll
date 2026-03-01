using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

    public class ConversationLog : SerializedMonoBehaviour
    {
        [Header("Element")] 
        [SerializeField] private TextMeshProUGUI contextText;
        
        [Header("Data")]
        [DictionaryDrawerSettings(KeyLabel = "Locale", ValueLabel = "Value"), SerializeField]
        private Dictionary<string, string> speaker;
        [DictionaryDrawerSettings(KeyLabel = "Locale", ValueLabel = "Value"), SerializeField]
        private Dictionary<string, string> context;
        
        private string NameColor => ColorUtility.ToHtmlStringRGBA(DataManager.Instance.GetNPCColor(speaker["ko"]));
        
        private void OnEnable()
        {
            if (speaker != null && speaker.Count != 0 && context != null && context.Count != 0)
            {
                CheckCustomTag(context[DataManager.Instance.LocaleSetting], out string result);
                contextText.text = $"<color=#{NameColor}>{speaker[DataManager.Instance.LocaleSetting]}</color> : {result}";
            }
        }

        public void SetData(Dictionary<string, string> _speaker, Dictionary<string, string> _context)
        {
            speaker = _speaker;
            context = _context;

            CheckCustomTag(context[DataManager.Instance.LocaleSetting], out string result);
            contextText.text = $"<color=#{NameColor}>{speaker[DataManager.Instance.LocaleSetting]}</color> : {result}";
        }
        
        private void CheckCustomTag(string _context, out string _result)
        {
            _result = _context;
        
            if (_context.Contains("<shake>"))
            {
                StringBuilder removed = new();
                removed.Append(_context);
                removed.Replace("<shake>", "");
                _result = removed.ToString();
            }
        }
    }