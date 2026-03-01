using UnityEngine;
using System;

public class ColliderHandler : MonoBehaviour
{
    public event Action OnTriggerEntered;
    public LayerMask layerMask;
    private bool oneTime = false;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0 && !oneTime && OnTriggerEntered != null && !GameManager.Instance.IsTimeStop)
        {
            oneTime = true;
            OnTriggerEntered?.Invoke();
            OnTriggerEntered = null;
        }
    }
}
