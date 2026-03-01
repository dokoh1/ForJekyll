using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[Serializable]
public class TimeLine
{
    [ShowInInspector, DictionaryDrawerSettings(KeyLabel = "Index", ValueLabel = "Dialogue")]
    [OdinSerialize] private Dictionary<int, IDialogue> data = new();
    [SerializeField] private List<int> skipIndex = new();

    private int currentIndex = 0;
    public int CurrentIndex => currentIndex - 1 < 0 ? 0 : currentIndex - 1;

    public void AddData(int index, IDialogue dialogue)
    {
        data.Add(index, dialogue);
    }

    public void AddSkipIndex(int index)
    {
        skipIndex.Add(index);
    } 

    public void MoveToIndex(int index)
    {
        currentIndex = index;
    }

    public void SetSkipIndex()
    {
        int skip = 0;
        foreach (var index in skipIndex)
        {
            if (currentIndex <= index)
            {
                skip = index;
                break;
            }
        }

        for (int i = currentIndex; i < skip; i++)
        {
            if (data[i] is ILog logData)
                logData.CreateLog(UIManager.Instance.view);
        }

        currentIndex = skip;
    }

    public IDialogue ReturnData()
    {
        return data[currentIndex++];
    }

    public int CreateLogs(int chapter, int index)
    {
        if (data[index] is ILog log && log.CreateLogJump(UIManager.Instance.view, out int next))
        {
            index = next - 1;
        }
        else if (data[index] is Select sel)
        {
            return sel.CreateLog(UIManager.Instance.view, DataManager.Instance.GetSelectData(chapter,index)) - 1;
        }

        return index;
    }

    public void CreateLogs(int chapter)
    {
        for (int i = 0; i < data.Count; )
        {
            if (data[i] is ILog log && log.CreateLogJump(UIManager.Instance.view, out int next))
            {
                i = next;
                continue;
            }
            else if (data[i] is Select sel)
            {
                i = sel.CreateLog(UIManager.Instance.view, DataManager.Instance.GetSelectData(chapter,i));
                continue;
            }

            i++;
        }
    }

    public void Clear()
    {
        data?.Clear();
        skipIndex?.Clear();
    }
}