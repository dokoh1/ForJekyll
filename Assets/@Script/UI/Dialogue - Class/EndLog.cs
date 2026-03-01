using TMPro;
using UnityEngine;

    public class EndLog : MonoBehaviour
    {
        [Header("Element")] 
        [SerializeField] private TextMeshProUGUI _endText;

        public void UpdateString(string str) => _endText.text = str;
    }