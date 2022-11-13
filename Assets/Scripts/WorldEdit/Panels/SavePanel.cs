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
    [SerializeField]
    InputField saveField;
    private int type;
    void Awake()
    {
        saveManager = EditSingleton.instance.saveManager;
        type = -1;

        saveButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(0));
        loadButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(1));
        saveField.onSubmit.AddListener(delegate { OnSubmit(saveField); });
        saveField.onEndEdit.AddListener(delegate { OnEndEdit(); });
    }

    private void Update()
    {
    }

    public void OnSetActive(bool value)
    {

    }

    public void OnButtonClick(int value)
    {
        type = value;
        saveField.gameObject.SetActive(true);
    }

    public void OnSubmit(InputField input)
    {
        switch (type)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Return)) saveManager.Save(input.text);
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Return)) saveManager.Load(input.text);
                break;
            default:
                break;
        }
        saveField.gameObject.SetActive(false);
        type = -1;
    }
    public void OnEndEdit()
    {
        saveField.gameObject.SetActive(false);
        type = -1;
    }
}
