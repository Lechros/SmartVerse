using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class CharEditorManager : MonoBehaviour
{
    [SerializeField]
    Button saveButton;
    [SerializeField]
    Button loadButton;
    [SerializeField]
    InputField saveField;
    private submitType type;

    public GameObject avatarInstance;
    AvatarManager avatarManager;
    public static string SavePath { get; private set; }

    void Awake()
    {
        avatarManager = FindObjectOfType<AvatarManager>();
        SavePath = new(GlobalVariables.CharacterPath);
        type = submitType.None;

        saveButton.GetComponent<Button>().onClick.AddListener(() => OnSaveButtonClick(saveField));
        loadButton.GetComponent<Button>().onClick.AddListener(() => OnLoadButtonClick(saveField));

        /*
        saveButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(submitType.Save, saveField));
        loadButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(submitType.Load, saveField));
        saveField.onSubmit.AddListener(delegate { OnSubmit(saveField); });
        saveField.onEndEdit.AddListener(delegate { OnEndEdit(saveField); });
        */
        if (GlobalVariables.SelectedAvatar != null)
        {
            LoadChar(GlobalVariables.SelectedAvatar, avatarInstance);
            saveField.text = GlobalVariables.SelectedAvatar;
        }
    }

    public void OnSetActive(bool value)
    {

    }

    public void OnSaveButtonClick(InputField input)
    {
        SaveChar(input.text);
        StartCoroutine(TimedPlaceholder("ÀúÀåµÊ!", 3, input));
    }

    public void OnLoadButtonClick(InputField input)
    {
        LoadChar(input.text, avatarInstance);
    }

    IEnumerator TimedPlaceholder(string message, float delay, InputField input)
    {
        var tempPlaceholder = ((Text)input.placeholder).text;
        var tempText = input.text;
        ((Text)input.placeholder).text = message;
        input.text = null;
        yield return new WaitForSeconds(delay);
        ((Text)input.placeholder).text = tempPlaceholder;
        input.text = tempText;
    }

    /*
    public void OnButtonClick(submitType value, InputField input)
    {
        type = value;
        input.interactable = true;
        input.text = GlobalVariables.SelectedWorld;
    }

    public bool OnSubmit(InputField input)
    {
        switch (type)
        {
            case submitType.Save:
                if (Input.GetKeyDown(KeyCode.Return))
                    if (input.text != "") SaveChar(input.text);
                break;
            case submitType.Load:
                if (Input.GetKeyDown(KeyCode.Return))
                    if (input.text != "") LoadChar(input.text, avatarInstance);
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
    */
    public bool SaveChar(string filename)
    {
        Debug.Log(SavePath);
        string path = CharNameToPath(filename);

        // Save world data to file.
        var json = avatarManager.GetConfigString(avatarManager.GetAvatarCustomization(avatarInstance));
        Debug.Log(json);
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
        File.WriteAllText(path, json);

        return true;
    }

    public bool LoadChar(string filename, GameObject character)
    {
        string path = CharNameToPath(filename);

        if (!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);
        avatarManager.ApplyAvatarCustomization(json, avatarManager.GetAvatarCustomization(character));
        return true;
    }
    string CharNameToPath(string charName) => Path.Join(SavePath, charName + ".sv");


    public enum submitType
    {
        None, Save, Load
    }
    public void OnLeaveButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
