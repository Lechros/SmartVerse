using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    static string SAVE_FOLDER = "Worlds";
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

    public static string ChosenFile;

}
