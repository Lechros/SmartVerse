using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Sunbox.Avatars;

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
    public static string SavePath { get; private set; }

    void Awake()
    {
        SavePath = new(GlobalVariables.CharacterPath);
        type = submitType.None;

        saveButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(submitType.Save, saveField));
        loadButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(submitType.Load, saveField));
        saveField.onSubmit.AddListener(delegate { OnSubmit(saveField); });
        saveField.onEndEdit.AddListener(delegate { OnEndEdit(saveField); });

        if (GlobalVariables.SelectedAvatar != null)
        {
            LoadChar(GlobalVariables.SelectedAvatar, avatarInstance);
        }
    }

    public void OnSetActive(bool value)
    {

    }

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

    public bool SaveChar(string filename)
    {
        Debug.Log(SavePath);
        string path = CharNameToPath(filename);

        // Save world data to file.
        var json = AvatarManager.GetConfigString(avatarInstance.GetComponent<AvatarCustomization>());
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
        AvatarManager.ApplyAvatarCustomization(json, character.GetComponent<AvatarCustomization>());
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
