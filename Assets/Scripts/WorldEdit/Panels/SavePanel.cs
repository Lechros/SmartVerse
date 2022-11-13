using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour, IPanel
{
    SaveManager saveManager;

    [SerializeField]
    Button saveButton;
    [SerializeField]
    Button loadButton;

    void Awake()
    {
        saveManager = EditSingleton.instance.saveManager;

        saveButton.GetComponent<Button>().onClick.AddListener(() => saveManager.Save("TempWorld"));
        loadButton.GetComponent<Button>().onClick.AddListener(() => saveManager.Load("TempWorld"));
    }

    private void Update()
    {
    }

    public void OnSetActive(bool value)
    {

    }
}
