using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    static readonly int StartTab = 0;

    [SerializeField]
    List<TabPanel> tabPanels = new List<TabPanel>();

    void Awake()
    {
        foreach(TabPanel tp in tabPanels)
        {
            tp.tab.onValueChanged.AddListener((call) => PanelSetActive(tp.panel, call));
            tp.panel.SetActive(true);
        }
    }

    void Start()
    {
        foreach(TabPanel tp in tabPanels)
        {
            tp.panel.SetActive(false);
        }
        SetTab(StartTab);
    }

    void PanelSetActive(GameObject panel, bool call)
    {
        panel.GetComponent<IPanel>().OnSetActive(call);
        panel.SetActive(call);
    }

    public int GetActiveTabIndex()
    {
        for(int i = 0; i < tabPanels.Count; i++)
        {
            if(tabPanels[i].tab.isOn)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetTab(int index)
    {
        if(index < 0 && index >= tabPanels.Count)
        {
            return;
        }

        tabPanels[index].tab.isOn = true;
        PanelSetActive(tabPanels[index].panel, true);
        Debug.Log("Done set tab. " + tabPanels[index].panel.name + ": " + tabPanels[index].panel.activeSelf);
    }

    [Serializable]
    public class TabPanel
    {
        public Toggle tab;
        public GameObject panel;
    }
}
