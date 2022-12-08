using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour, IPanel
{
    SaveManager saveManager;

    public TMP_InputField nameInput;
    public Button saveButton;
    public TMP_Text saveSuccessMsg;

    void Awake()
    {
        saveManager = EditSceneManager.instance.saveManager;
    }

    private void Start()
    {
        nameInput.text = GlobalVariables.SelectedWorld ?? "";
    }

    public void OnSetActive(bool value)
    {

    }

    public void OnClickSave()
    {
        if(nameInput.text.Length == 0)
        {
            return;
        }

        saveManager.Save(nameInput.text);
        GlobalVariables.SelectedWorld = nameInput.text;
        StartCoroutine(ShowSaveMsg());
    }

    IEnumerator ShowSaveMsg()
    {
        saveSuccessMsg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        saveSuccessMsg.gameObject.SetActive(false);
    }
}
