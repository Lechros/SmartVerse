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
    InputField saveField;
    private submitType type;
    void Awake()
    {
        saveManager = EditSceneManager.instance.saveManager;
        type = submitType.None;

        saveButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(submitType.Save, saveField));
        saveField.onSubmit.AddListener(delegate { OnSubmit(saveField); });
        saveField.onEndEdit.AddListener(delegate { OnEndEdit(saveField); });
    }

    private void Update()
    {
    }

    public void OnSetActive(bool value)
    {

    }

    public void OnButtonClick(submitType value, InputField input)
    {
        type = value;
        input.interactable = true;
        input.text = GlobalVariables.ChosenFile;
    }

    public bool OnSubmit(InputField input)
    {
        switch (type)
        {
            case submitType.Save:
                if (Input.GetKeyDown(KeyCode.Return)) 
                    if (input.text != "") saveManager.Save(input.text);
                break;
            case submitType.Load:
                if (Input.GetKeyDown(KeyCode.Return))
                    if (input.text != "") saveManager.Load(input.text);
                break;
            default:
                break;
        }
        type = submitType.None;
        input.interactable = false;
        input.text = null;
        return true;
    }
    public bool OnEndEdit(InputField input)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            type = submitType.None;
            input.interactable = false;
            input.text = null;
        }
        return true;
    }

    public enum submitType
    {
        None, Save, Load
    }
}
