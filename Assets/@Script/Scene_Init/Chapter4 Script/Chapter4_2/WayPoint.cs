using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public static event Action OnDoorCrashRequest;

    public bool OnMonster;
    public float SpawnDelay;
    public bool MonsterSound;
    public bool MonsterDead;

    private bool screamPlayed = false;

    [SerializeField] private GameObject monsterToSpawn;
    [SerializeField] private List<GameObject> monsters;

    private void OnTriggerEnter(Collider other)
    {
        MakeNoise();

        if (other.CompareTag("Player") && OnMonster)
        {
            Debug.Log("WayPoint in");

            StartCoroutine(SpawnMonster(SpawnDelay));
        }

        if(other.CompareTag("Monster"))
        {
            MonsterNoise();
        }

        if (MonsterDead && other.CompareTag("Monster"))
        {
            KillAllMonsters();
        }
    }

    private IEnumerator SpawnMonster(float delayTime = 1.0f)
    {
        OnDoorCrashRequest?.Invoke();

        yield return new WaitForSeconds(delayTime);

        monsterToSpawn.SetActive(true);

        var sound = monsterToSpawn.GetComponent<MonsterSoundSystem>();
        if (sound != null)
        {
            sound.PlaySoundByKey(MonsterSoundKey.Find, false);
            Debug.Log("Monster Find");
        }

        MakeNoise();
    }

    private void MakeNoise()
    {
        var player = GameManager.Instance.Player;
        player.CurNoiseAmount = player.MaxNoiseAmount;

    }

    private void MonsterNoise()
    {
        if (MonsterSound && !screamPlayed)
        {
            var sound = monsterToSpawn.GetComponent<MonsterSoundSystem>();
            if (sound != null)
            {
                sound.PlaySoundByKey(MonsterSoundKey.Chase, false);
                Debug.Log("Monster Chase");
                screamPlayed = true;
            }
        }
    }
    private void KillAllMonsters()
    {
        foreach (var m in monsters)
        {
            if (m == null) continue;
            StartCoroutine(KillAfterSound(m));
        }
    }

    private IEnumerator KillAfterSound(GameObject m)
    {
        var sound = m.GetComponent<MonsterSoundSystem>();

        if (sound != null)
        {
            sound.PlaySoundByKey(MonsterSoundKey.Lost, false);
            Debug.Log("Monster Lost");
        }

        yield return new WaitForSeconds(2.0f);

        m.SetActive(false);
    }
}