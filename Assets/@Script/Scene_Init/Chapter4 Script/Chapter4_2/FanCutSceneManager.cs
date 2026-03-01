using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanCutSceneManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] cams;

    private const int ACTIVE = 10;
    private const int INACTIVE = 0;

    [SerializeField] private FanLever fanLever;
    [SerializeField] private Fan fan;

    public void UseCam(int index)
    {
        for (int i = 0; i < cams.Length; i++)
            cams[i].Priority = (i == index) ? ACTIVE : INACTIVE;
    }

    public void UseCam1() => UseCam(0);
    public void UseCam2() => UseCam(1);

    public void ResetCam()
    {
        for(int i = 0; i < cams.Length; i++)
            cams[i].Priority = INACTIVE;
    }

    public void LeverActionPlay()
    {
        fanLever.LeverAction();

        fanLever.CutScenePlayed = false;
    }
    
    public void ActivateFan()
    {
        fan.Play();
    }

    public void FadeOut()
    {
        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeOut);
    }

    public void FadeIn()
    {
        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn);
    }
}