using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    [Header("Tab Container")]
    [SerializeField] private GameObject tabContainer;
    [SerializeField] private GameObject[] tabs;

    [Header("Tab Buttons")]
    [SerializeField] private NotebookTabButton tabButton;

    private Dictionary<string, GameObject> tabDict = new();
    private GameObject currentTab;

    public string CurrentTabName => currentTab != null ? currentTab.name : null;

    private void Awake()
    {
        gameObject.SetActive(false);
        tabContainer.SetActive(true);

        foreach (GameObject tab in tabs)
        {
            tab.SetActive(false);
            tabDict[tab.name] = tab;
        }

        currentTab = null;
    }

    public void OpenNotebook(string tabName)
    {
        gameObject.SetActive(true);
        OpenTab(tabName);
    }

    public void OpenTab(string tabName)
    {
        if (CurrentTabName == tabName) return;

        if (currentTab != null)
            currentTab.SetActive(false);

        if (tabDict.TryGetValue(tabName, out GameObject tab))
        {
            tab.SetActive(true);
            currentTab = tab;
            tabButton.UpdateImage(tabName);
        }
        else
        {
            Debug.LogWarning($"존재하지 않는 탭: {tabName}");
        }
    }
    public void CloseNotebook()
    {
        gameObject.SetActive(false);

        if (currentTab != null)
            currentTab.SetActive(false);

        currentTab = null;
    }
}
