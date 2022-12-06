using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour, IPanel
{
    SaveManager saveManager;

    void Awake()
    {
        saveManager = EditSceneManager.instance.saveManager;
    }

    public void OnSetActive(bool value)
    {

    }

    public void OnClickSave()
    {
        saveManager.Save(GlobalVariables.SelectedWorld);
    }
}
