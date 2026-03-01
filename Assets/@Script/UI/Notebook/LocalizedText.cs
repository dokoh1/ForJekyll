using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    private TMP_Text Text;

    private void Awake()
    {
        Text = GetComponent<TMP_Text>();
    }

    public void UpdateText(string str)
    {
        if (Text != null)
            Text.text = str;
    }
}