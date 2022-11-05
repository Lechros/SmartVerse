using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    InteractionManager interactionManager;

    [SerializeField]
    List<TabPanel> tabPanels = new List<TabPanel>();

    void Awake()
    {
        interactionManager = SingletonManager.instance.interactionManager;

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
        SetTab(2);
    }

    void PanelSetActive(GameObject panel, bool call)
    {
        panel.GetComponent<IPanel>().OnSetActive(call);
        panel.SetActive(call);
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
