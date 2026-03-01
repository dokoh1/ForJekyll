using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPassage : MonoBehaviour
{
    [SerializeField] private Transform[] waitSlots;
    private Queue<NavMeshAgent> queue = new Queue<NavMeshAgent>();
    private NavMeshAgent current;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Monster")) return;

        var agent = other.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        if (current == null)
        {
            current = agent;
            agent.isStopped = false;
        }
        else
        {
            queue.Enqueue(agent);
            agent.SetDestination(waitSlots[Mathf.Min(queue.Count - 1, waitSlots.Length - 1)].position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Monster")) return;

        var agent = other.GetComponent<NavMeshAgent>();
        if (agent != current) return;

        current = null;

        if (queue.Count > 0)
        {
            current = queue.Dequeue();
            current.isStopped = false;
        }
    }
}