using System;
using UnityEngine;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour
    {
        [Header("Element")] 
        [SerializeField] private Button button;

        [Header("Object")] 
        [SerializeField] private GameObject go;


        private void OnEnable()
        {
            button.onClick.AddListener(() => go.SetActive(false));
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }
    }