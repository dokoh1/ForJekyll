using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectSystem : MonoBehaviour
{
    public event Action<Transform> OnTargetChanged;
    public event Action OnTargetNull;
    public Transform targetTransform;
    public HashSet<Transform> targets = new HashSet<Transform>();

    private Coroutine _checkTargetCoroutine;
    private bool _isDetect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            AddTarget(other);
            StartCheckTargetCoroutine();
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            RemoveTarget(other);
            if (targets.Count == 0)
            {
                StopCheckTargetCoroutine();
                targetTransform = null;
                OnTargetNull?.Invoke();
            }
            else StartCheckTargetCoroutine();
        }
    }

    private void Update()
    {
        if (targetTransform != null)
        {
            if (!targetTransform.gameObject.activeInHierarchy)
            {
                StopCheckTargetCoroutine();
                targetTransform = null;
                OnTargetNull?.Invoke();
            }
        }        
    }

    private void StartCheckTargetCoroutine()
    {
        StopCheckTargetCoroutine();
        _checkTargetCoroutine = StartCoroutine(CheckTargetRoutine());
    }

    private void StopCheckTargetCoroutine()
    {
        if (_checkTargetCoroutine != null)
        {
            StopCoroutine(_checkTargetCoroutine);
            _checkTargetCoroutine = null;
        }
    }

    private IEnumerator CheckTargetRoutine()
    {
        while (true)
        {
            CheckTarget();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void CheckTarget()
    {
        float shortestDistance = float.MaxValue;
        targetTransform = null;

        foreach (Transform target in targets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                targetTransform = target;
                OnTargetChanged?.Invoke(targetTransform);
            }
        }
    }

    private void AddTarget(Collider collider)
    {        
        if (!targets.Contains(collider.transform)) targets.Add(collider.transform);
    }

    private void RemoveTarget(Collider collider)
    {
        if (targets.Contains(collider.transform)) targets.Remove(collider.transform);
    }

    public void StopDetect()
    {
        targets.Clear();
        //StopCoroutine(_checkTargetCoroutine);
        _checkTargetCoroutine = null;
        gameObject.SetActive(false);
    }
}
