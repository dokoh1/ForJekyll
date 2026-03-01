using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cons_Door_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = false;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    private ConforenceScene_Manager _conforenceSceneManager => ConforenceScene_Manager.Instance;

    Dictionary<GameObject, bool> chairsDict = new Dictionary<GameObject, bool>();
    
    [Header("GameObject")]
    [SerializeField] private GameObject pointLight;
    [SerializeField] private List<GameObject> chairs;
    [SerializeField] private int chairCount = 0;
    [field: SerializeField] public bool consDoorEvent { get; set; }

    private void Awake()
    {
        foreach (GameObject chair in chairs)
        {
            chairsDict.Add(chair, false);
        }
        _conforenceSceneManager.bossBattleScript.doors.Add(this);
    }

    public override void Interact()
    {
        if (!consDoorEvent && !IsInteractable) return;

        if (_conforenceSceneManager.bossBattleScript.playerPickUp)
        {
            foreach (GameObject chair in chairs)
            {
                if (chairsDict[chair] == false)
                {
                    chairCount++;
                    chairsDict[chair] = true;
                    _conforenceSceneManager.bossBattleScript.playerPickUp = false;
                    _conforenceSceneManager.playerChair.SetActive(false);
                    chair.SetActive(true);
                    return;
                }
            }
        }
    }

    public IEnumerator DoorMonsterEvent()
    {
        float timer = 0f;
        IsInteractable = true;
        consDoorEvent = true;
        pointLight.SetActive(true);
        
        while (true)
        {
            timer += Time.deltaTime;
            if (chairCount == 4)
            {
                pointLight.SetActive(false);
                IsInteractable = false;
                break;
            }

            if (timer >= 30 && !_conforenceSceneManager.bossBattleScript.battleEnd)
            {
                
            }
            yield return null;
        }
    }
}
