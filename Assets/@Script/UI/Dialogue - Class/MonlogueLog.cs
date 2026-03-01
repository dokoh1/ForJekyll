using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

    public class MonlogueLog : SerializedMonoBehaviour
    {
        [Header("Element")] 
        [SerializeField] private TextMeshProUGUI contextText;
        
        [Header("Data")]
        [DictionaryDrawerSettings(KeyLabel = "Locale", ValueLabel = "Value")]
        [SerializeField] private Dictionary<string, string> context;
        
        private void OnEnable()
        {
            if (context != null && context.Count != 0)
            {
                CheckCustomTag(context[DataManager.Instance.LocaleSetting], out string result);
                contextText.text = result;
            }
        }

        public void SetData(Dictionary<string, string> _context)
        {
            context = _context;
            
            CheckCustomTag(context[DataManager.Instance.LocaleSetting], out string result);
            contextText.text = result;
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