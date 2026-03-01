using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSystemManager : MonoBehaviour
{
    [Header("Fan Settings")]
    [SerializeField] private Fan fan;
    [SerializeField] private Transform fanForward; 
    [SerializeField] private float initialPushPower = 10f;  
    [SerializeField] private float continuousPushPower = 5f; 
    [SerializeField] private float fanDuration = 2.0f; 

    private List<FanTarget> targetsInArea = new List<FanTarget>();

    private bool fanActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Monster"))
        {
            FanTarget target = other.GetComponent<FanTarget>();
            if (target != null)
            {
                Debug.Log("Trigger in");

                targetsInArea.Add(target);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Monster"))
        {
            FanTarget target = other.GetComponent<FanTarget>();
            if (target != null)
            {
                Debug.Log("Trigger out");

                targetsInArea.Remove(target);
                target.ResetFan();
            }
        }
    }

    public void ActivateFan()
    {
        if (fanActive) return;

        fan.Play();
        StartCoroutine(FanPowerRoutine());
    }

    private IEnumerator FanPowerRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Fan On");
        fanActive = true;

        Vector3 dir = fanForward.forward.normalized;

        foreach (var t in targetsInArea)
        {
            if (t is MonsterFanTarget monster)
                monster.OnInitialPush(dir * initialPushPower); // Monster
            else
                t.AddVelocity(dir * initialPushPower);  // Player
        }

        float timer = 0f;

        while (timer < fanDuration)
        {
            timer += Time.deltaTime;

            foreach (var t in targetsInArea)
                t.AddVelocity(dir * continuousPushPower * Time.deltaTime);

            yield return null;
        }

        Debug.Log("Fan Off");

        fan.Stop();
        fanActive = false;

        foreach (var t in targetsInArea)
        {
            t.ResetFan();
        }
    }
}
