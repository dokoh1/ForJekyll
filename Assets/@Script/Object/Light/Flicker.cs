using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

public class Flicker : MonoBehaviour
{
    [SerializeField] private Light lightSource;
    public float minTime = 0.8f;
    public float maxTime = 1.0f;

    private Coroutine flickerCoroutine;
    
    public void StartFlicker()
    {
        if (flickerCoroutine != null)
            return;

        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            int flickCount = UnityEngine.Random.Range(1, 3);

            for (int i = 0; i < flickCount; i++)
            {
                lightSource.enabled = false;
                yield return new WaitForSeconds(0.1f);

                lightSource.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime));
        }
    }

    /*
    private IEnumerator StartFlicker()
    {
        while (true)
        {
            int flickCount = UnityEngine.Random.Range(1, 3);

            for (int i = 0; i < flickCount; i++)
            {
                lightSource.enabled = false;
                yield return new WaitForSeconds(0.1f);

                lightSource.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime));
        }
    }
    */
}