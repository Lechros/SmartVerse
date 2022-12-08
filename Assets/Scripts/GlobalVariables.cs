using Sunbox.Avatars;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    static string SAVE_FOLDER = "Worlds";
    static string CHARACTER_FOLDER = "Characters";
    static string DATA_FOLDER = "Data";

    static string _savePath;
    public static string SavePath
    {
        get
        {
            if(_savePath == null)
            {
                _savePath = Path.Join(Application.persistentDataPath, SAVE_FOLDER);
            }
            return _savePath;
        }
    }

    static string _characterPath;
    public static string CharacterPath
    {
        get
        {
            if (_characterPath == null)
            {
                _characterPath = Path.Join(Application.persistentDataPath, CHARACTER_FOLDER);
            }
            return _characterPath;
        }
    }

    static string _dataPath;
    public static string DataPath
    {
        get
        {
            if (_dataPath == null)
            {
                _dataPath = Path.Join(Application.persistentDataPath, DATA_FOLDER);
            }
            return _dataPath;
        }
    }

    public static string SelectedWorld;
    public static string SelectedWorldType;

    public static bool ShouldLoadWorld;

    public static string SelectedAvatar;

    public static AvatarReferences avatarReferences;

    public AvatarReferences avatarReferencesSetter;

    private void Awake()
    {
        avatarReferences = avatarReferencesSetter;
    }
}
