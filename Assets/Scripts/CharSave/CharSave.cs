using System.Collections;
using System.Collections.Generic;
using Sunbox.Avatars;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class CharSave : MonoBehaviour
{

    [SerializeField]
    Button saveButton;
    [SerializeField]
    Button loadButton;
    [SerializeField]
    InputField saveField;
    private submitType type;

    public GameObject avatarInstance;

    static string SAVE_FOLDER = "Characters";
    public static string SavePath { get; private set; }
    public AvatarReferences AvatarReferences;

    void Start()
    {
        type = submitType.None;

        saveButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(submitType.Save, saveField));
        loadButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(submitType.Load, saveField));
        saveField.onSubmit.AddListener(delegate { OnSubmit(saveField); });
        saveField.onEndEdit.AddListener(delegate { OnEndEdit(saveField); });
        SavePath = Path.Join(Application.persistentDataPath, SAVE_FOLDER);
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
                    if (input.text != "") SaveChar(input.text);
                break;
            case submitType.Load:
                if (Input.GetKeyDown(KeyCode.Return))
                    if (input.text != "") LoadChar(input.text);
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
        AvatarCustomization avatar = avatarInstance.GetComponent<AvatarCustomization>();
        string json = AvatarCustomization.ToConfigString(avatar);
        Debug.Log(json);
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
        File.WriteAllText(path, json);

        return true;
    }

    public bool LoadChar(string filename)
    {
        string path = CharNameToPath(filename);

        if (!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);
        AvatarCustomization avatar = avatarInstance.GetComponent<AvatarCustomization>();
        ApplyCharacter(json, avatar);
        //AvatarCustomization.ApplyConfigFile(json, avatar);
        return true;
    }

    string CharNameToPath(string charName) => Path.Join(SavePath, charName + ".sv");

    public bool ApplyCharacter(string json, AvatarCustomization avatar)
    {
        avatar.ClothingItemHat = null;
        avatar.ClothingItemTop = null;
        avatar.ClothingItemBottom = null;
        avatar.ClothingItemGlasses = null;
        avatar.ClothingItemShoes = null;
        foreach (string str in json.Split('\n')) {
            var type = str.Split('=')[0];
            if (type == "") break;
            var value = str.Split('=')[1];
            float floatValue; int intValue;
            float.TryParse(value, out floatValue);
            int.TryParse(value, out intValue);
            switch (type)
            {
                case "g_gender":
                    if (value == "Male")
                        avatar.SetGender(AvatarCustomization.AvatarGender.Male);
                    else if (value == "Female")
                        avatar.SetGender(AvatarCustomization.AvatarGender.Female);
                    break;
                case "f_bodyFat":
                    avatar.BodyFat = floatValue;
                    break;
                case "f_bodyMuscle":
                    avatar.BodyMuscle = floatValue;
                    break;
                case "f_bodyHeightMetres":
                    avatar.BodyHeight = floatValue;
                    break;
                case "f_breastSize":
                    avatar.BreastSize = floatValue;
                    break;
                case "f_noseLength":
                    avatar.NoseLength = floatValue;
                    break;
                case "f_lipsWidth":
                    avatar.LipsWidth = floatValue;
                    break;
                case "f_jawWidth":
                    avatar.JawWidth = floatValue;
                    break;
                case "f_browWidth":
                    avatar.BrowWidth = floatValue;
                    break;
                case "f_browHeight":
                    avatar.BrowHeight = floatValue;
                    break;
                case "f_eyesSize":
                    avatar.EyesSize = floatValue;
                    break;
                case "f_eyesClosedDefault":
                    avatar.EyesClosedDefault = floatValue;
                    break;
                case "i_skinMaterialIndex":
                    avatar.SkinMaterialIndex = intValue;
                    break;
                case "i_hairStyleIndex":
                    avatar.HairStyleIndex = intValue;
                    break;
                case "i_hairMaterialIndex":
                    avatar.HairMaterialIndex = intValue;
                    break;
                case "i_facialHairStyleIndex":
                    avatar.FacialHairStyleIndex = intValue;
                    break;
                case "i_facialHairMaterialIndex":
                    avatar.FacialHairMaterialIndex = intValue;
                    break;
                case "i_eyeMaterialIndex":
                    avatar.EyeMaterialIndex = intValue;
                    break;
                case "i_lashesMaterialIndex":
                    avatar.LashesMaterialIndex = intValue;
                    break;
                case "i_browMaterialIndex":
                    avatar.BrowMaterialIndex = intValue;
                    break;
                case "i_nailsMaterialIndex":
                    avatar.NailsMaterialIndex = intValue;
                    break;
                case "clothingItem":
                    string itemStr = value.Split('-')[0];
                    ClothingItem item = Array.Find(AvatarReferences.AvailableClothingItems, item => item.Name == itemStr);
                    int varIndex = int.Parse(value.Split('-')[1]);
                    avatar.AttachClothingItem(
                        item: item,
                        variationIndex: varIndex
                    );
                    break;
                default:
                    break;
            }
        }
        avatar.UpdateCustomization();
        avatar.UpdateClothing();
        return true;
    }
    public enum submitType
    {
        None, Save, Load
    }
}
