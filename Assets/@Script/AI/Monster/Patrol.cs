using System.Collections.Generic;
using UnityEngine;

public class Patrol
{
    private List<Vector3> PatrolPositionListKeep { get; set; }
    private List<Transform> PatrolPositionList { get; set; }

    public Transform[] PatrolTransforms { get; set; }

    private Monster _monster;
    private MonsterV2 _monsterV2;

    private int _patrolPositionsIndex = 0;

    public Patrol(Monster monster)
    {
        _monster = monster;
        PatrolPositionListKeep = new();
        PatrolPositionList = new();
    }

    public Patrol(MonsterV2 monster)
    {
        _monsterV2 = monster;
        PatrolPositionListKeep = new();
        PatrolPositionList = new();
    }

    public void SetPatrolPositions(bool isKeep)
    {
        for (int i = 1; i < PatrolTransforms.Length; i++)
        {
            if (isKeep) PatrolPositionListKeep.Add(PatrolTransforms[i].position);
            else PatrolPositionList.Add(PatrolTransforms[i]);
        }
        PatrolTransforms = null;

        //if (isKeep) SufflePositions(PatrolPositionListKeep);
        //else SufflePositions(PatrolPositionList);
    }

    public Vector3 GetPatrolPosition(bool isKeep)
    {
        if (isKeep)
        {
            if (_patrolPositionsIndex < PatrolPositionListKeep.Count)
            {
                int index = _patrolPositionsIndex;
                _patrolPositionsIndex++;
                return PatrolPositionListKeep[index];
            }
            else
            {
                //SufflePositions(PatrolPositionListKeep);
                _patrolPositionsIndex = 0;
                int index = _patrolPositionsIndex;
                _patrolPositionsIndex++;
                return PatrolPositionListKeep[index];
            }
        }
        else
        {
            if (_patrolPositionsIndex < PatrolPositionList.Count)
            {
                _patrolPositionsIndex++;
                return PatrolPositionList[_patrolPositionsIndex - 1].position;
            }
            else
            {
                //SufflePositions(PatrolPositionList);
                _patrolPositionsIndex = 0;
                _patrolPositionsIndex++;
                return PatrolPositionList[_patrolPositionsIndex - 1].position;
            }
        }
    }

    //public Vector3 GetPatrolPosition(bool isKeep)
    //{
    //    if (isKeep)
    //    {
    //        if (_patrolPositionsIndex < PatrolPositionListKeep.Count)
    //        {
    //            _patrolPositionsIndex++;
    //            return PatrolPositionListKeep[_patrolPositionsIndex - 1];
    //        }
    //        else
    //        {
    //            SufflePositions(PatrolPositionListKeep);
    //            _patrolPositionsIndex = 0;
    //            _patrolPositionsIndex++;
    //            return PatrolPositionListKeep[_patrolPositionsIndex - 1];
    //        }
    //    }
    //    else
    //    {
    //        if (_patrolPositionsIndex < PatrolTransforms.Length)
    //        {
    //            _patrolPositionsIndex++;
    //            return PatrolTransforms[_patrolPositionsIndex - 1].position;
    //        }
    //        else
    //        {
    //            SufflePositions(PatrolTransforms);
    //            _patrolPositionsIndex = 0;
    //            _patrolPositionsIndex++;
    //            return PatrolTransforms[_patrolPositionsIndex - 1].position;
    //        }
    //    }
    //}

    //private void SufflePositions(List<Vector3> datas)
    //{
    //    for (int i = 0; i < datas.Count; i++)
    //    {
    //        int j = UnityEngine.Random.Range(0, i + 1);
    //        (datas[i], datas[j]) = (datas[j], datas[i]);
    //    }
    //}

    //private void SufflePositions(List<Transform> datas)
    //{
    //    for (int i = 0; i < datas.Count; i++)
    //    {
    //        int j = UnityEngine.Random.Range(0, i + 1);
    //        (datas[i], datas[j]) = (datas[j], datas[i]);
    //    }
    //}

    //private void SufflePositions(Transform[] datas)
    //{
    //    for (int i = 0; i < datas.Length; i++)
    //    {
    //        int j = UnityEngine.Random.Range(0, i + 1);
    //        (datas[i], datas[j]) = (datas[j], datas[i]);
    //    }
    //}
}
