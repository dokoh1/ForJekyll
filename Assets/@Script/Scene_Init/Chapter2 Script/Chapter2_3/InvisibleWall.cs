using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invisibleWall : MonoBehaviour
{
    private bool oneTime = false;
    [SerializeField] private int dialogueIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (oneTime || GameManager.Instance.IsTimeStop) return;
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, dialogueIndex);
            oneTime = true;
        }
    }
}