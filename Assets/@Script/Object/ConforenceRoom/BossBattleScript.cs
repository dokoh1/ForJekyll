using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NPC;
using UnityEngine;

public class BossBattleScript : MonoBehaviour
{
    private ConforenceScene_Manager _conforenceSceneManager => ConforenceScene_Manager.Instance;
    
    public List<CR_Generator_Obj> generatorObjs = new List<CR_Generator_Obj>();
    public List<Pillar_Light> pillarLights = new List<Pillar_Light>();
    public List<Cons_Door_Obj> doors = new List<Cons_Door_Obj>();
    public List<NpcUnit> npcUnits; 
    
    private WaitForSeconds doorRandEvent = new WaitForSeconds(5f);
    private WaitForSeconds npcMove = new WaitForSeconds(5f);

    public DOTweenAnimation playerHandle;
    
    [Header("게이지 증가 배율 설정")]
    public float timeLit = 120; // 시간 제한
    public float baseSpeed = 1f; // 기본 속도 (자동 계산됨)
    public float boostPerObject = 1f; // 켜진 오브젝트 1개당 속도 증가 비율

    public float currentGauge = 0f; // 현재 게이지
    public bool battleEnd { get; private set; }

    [Header("의자 듬")] public bool playerPickUp { get; set; } = false;
    private void Start()
    {
        baseSpeed = 100f / timeLit; // 2분에 100 도달
    }
    
    public void StartBossBattle()
    {
        StartCoroutine(GeneratorSpawner());
        StartCoroutine(AddfillBar());
        StartCoroutine(ConsDoorEvent());
        NPC_Check();
    }

    #region 보스 에너지 게이지

    private IEnumerator AddfillBar()
    {
        while (currentGauge < 100 && !battleEnd)
        {
            float boostedSpeed = baseSpeed * GetBoostMultiplier();
            currentGauge += boostedSpeed * Time.deltaTime;
            currentGauge = Mathf.Clamp(currentGauge, 0f, 100f);

            if (currentGauge is > 0 and < 10)
            {
                PageUp(0);
            }
            else if (currentGauge is > 10 and < 25)
            {
                PageUp(1);
            }
            else if (currentGauge is > 25 and < 40)
            {
                PageUp(2);
            }
            else if (currentGauge is > 40 and < 55)
            {
                PageUp(3);
            }
            else if (currentGauge is > 55 and < 70)
            {
                PageUp(4);
            }
            else if (currentGauge is > 70 and < 80)
            {
                PageUp(5);
            }
            else if (currentGauge is > 80 and < 100)
            {
                PageUp(6);
            }
            
            yield return null;
        }
        battleEnd = true;
        
        _conforenceSceneManager.StartBossBattleTimeLine();
    } // 벽 게이지
    private void PageUp(int page)
    {
        foreach (var go in pillarLights)
        {
            go.ChangeLight(page);
        }
    } // 게이지 색깔
    float GetBoostMultiplier()
    {
        int activeCount = 0;
        foreach (CR_Generator_Obj obj in generatorObjs)
        {
            if (obj.isEnable)
                activeCount++;
        }

        return 1f + (boostPerObject * activeCount); // 예: 3개 켜져있으면 +30%
    }

    #endregion

    #region 발전기
    private IEnumerator GeneratorSpawner()
    {
        float time = 0;

        while (!battleEnd && time < 30)
        {
            GeneratorOn(); // 즉시 실행
            yield return new WaitForSeconds(5f);
            time += 5f;
        }
        SoundManager.Instance.PlaySE(SE_Sound.AMB_Earthquake);
        playerHandle.DOKill();
        playerHandle.CreateTween();
        
        // 🔥 2페이즈: 60초부터 3초마다 실행
        while (!battleEnd && time < 60)
        {
            GeneratorOn();

            // 🎲 30% 확률로 한 번 더 실행
            if (Random.value < 0.3f)
            {
                GeneratorOn();
            }

            yield return new WaitForSeconds(3f);
            time += 3f;
        }
        
        SoundManager.Instance.PlaySE(SE_Sound.AMB_Earthquake);
        playerHandle.DOKill();
        playerHandle.CreateTween();
        
        // 🔥 3페이즈: 90초 부터 3초마다 실행
        while (!battleEnd && time < 90)
        {
            GeneratorOn();

            // 🎲 50% 확률로 한 번 더 실행
            if (Random.value < 0.5f)
            {
                GeneratorOn();
            }

            yield return new WaitForSeconds(3f);
            time += 3f;
        }
    }
    private void GeneratorOn()
    {
        if (generatorObjs.Count == 0) return; 

        int attempts = 0;
        int maxAttempts = generatorObjs.Count * 2; // 무한 루프 방지
        int rand;

        do
        {
            rand = Random.Range(0, generatorObjs.Count);
            attempts++;

            if (attempts > maxAttempts) // 무한 루프 방지
            {
                return;
            }

        } while (generatorObjs[rand].isEnable); // isActive가 true면 다시 뽑기

        generatorObjs[rand].GeneratorOn();
    }

    #endregion

    #region 문

    private void DoorEventOn()
    {
        if (doors.Count == 0) return; 

        int attempts = 0;
        int maxAttempts = doors.Count * 2; // 무한 루프 방지
        int rand;

        do
        {
            rand = Random.Range(0, doors.Count);
            attempts++;

            if (attempts > maxAttempts) // 무한 루프 방지
            {
                return;
            }

        } while (doors[rand].consDoorEvent); // isActive가 true면 다시 뽑기

        StartCoroutine(doors[rand].DoorMonsterEvent());
    }
    private IEnumerator ConsDoorEvent()
    {
        while (!battleEnd)
        {
            var rand = Random.Range(1, 100);

            if (rand <= 20)
            {
                var doorCount = doors.Count(go => go.consDoorEvent);

                if (doorCount == 4) break;
                
                DoorEventOn();
            }
            yield return doorRandEvent;
        }
    }

    #endregion

    #region Npc

        private void NPC_Check()
    {
        foreach (var npc in npcUnits.ToList())
        {
            switch (npc.npcName)
            {
                case NpcName.PJH:
                    if (ScenarioManager.Instance.GetAchieve(ScenarioAchieve.PJH_Dead))
                    {
                        npcUnits.Remove(npc);
                    }
                    break;
                case NpcName.PSY:
                    if (ScenarioManager.Instance.GetAchieve(ScenarioAchieve.PSU_Dead))
                    {
                        npcUnits.Remove(npc);
                    }
                    break;
                case NpcName.YHJ:
                    if (ScenarioManager.Instance.GetAchieve(ScenarioAchieve.YHJ_Dead))
                    {
                        npcUnits.Remove(npc);
                    }
                    break;
            }
        }
        foreach (var npc in npcUnits)
        {
            npc.gameObject.SetActive(true);
            Npc_GeneratorObjsDict.Add(npc.npcName, null);
            StartCoroutine(NpcMove(npc));
        }
    }
    private int CheckGenerator(bool fun)
    {
        if (generatorObjs.Count == 0) return -1; 

        int attempts = 0;
        int maxAttempts = generatorObjs.Count * 2; // 무한 루프 방지
        int rand;
        
        while (true)
        {
            rand = Random.Range(0, generatorObjs.Count);
            attempts++;

            if (attempts > maxAttempts) // 무한 루프 방지
            {
                return -1;
            }

            if (generatorObjs[rand].isEnable == fun)
            {
                return rand;
            }
        }
    }
    private void NpcGeneratorTurn(NpcUnit npc)
    {
        Debug.Log(npc.npcName);
        if (npc.Agent.isStopped && Npc_GeneratorObjsDict.ContainsKey(npc.npcName) 
                                && Npc_GeneratorObjsDict.TryGetValue(npc.npcName, out CR_Generator_Obj go) && go != null)
        {
            if (npc.npcName == NpcName.PSY && ScenarioManager.Instance.GetAchieve(ScenarioAchieve.PSU_Crazy))
            {
                go.GeneratorOn(); 
            }
            else
            {
                go.Interact();
            }
            Npc_GeneratorObjsDict[npc.npcName] = null;
        }
    }

    [SerializeField] private SerializableDictionary<NpcName, CR_Generator_Obj> Npc_GeneratorObjsDict = new SerializableDictionary<NpcName, CR_Generator_Obj>();

    private IEnumerator NpcMove(NpcUnit npc)
    { 
        var waitUntil = new WaitUntil(() => npc.Agent.isStopped);

        while (!battleEnd)
        {
            var rand = npc.npcName == NpcName.PSY ? CheckGenerator(false) : CheckGenerator(true);
            
            if (rand != -1)
            {
                npc.destination = generatorObjs[rand].transform;
                Npc_GeneratorObjsDict[npc.npcName] = generatorObjs[rand];
                npc.MoveToDestination();
                npc.onNpcStop += NpcGeneratorTurn;
                yield return waitUntil; // 이동 멈출때까지 대기
            }
            yield return npcMove; // 다시 움직일 시간
        }
    }

    #endregion

}
