using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    private bool oneTime = false;
    public Chapter3_2_SceneInitializer initializer;

    private void OnTriggerEnter(Collider other)
    {
        if (oneTime || GameManager.Instance.IsTimeStop) return;
        if (other.CompareTag("Player"))
        {
            initializer.CutSceneEvent();
            oneTime = true;
        }
    }
}