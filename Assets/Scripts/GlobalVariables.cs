using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    static string SAVE_FOLDER = "Worlds";
    public static string SavePath { get; private set; }
    public static string ChosenFile;
    // Start is called before the first frame update
    void Start()
    {
        ChosenFile = null;
        DontDestroyOnLoad(gameObject);
        SavePath = Path.Join(Application.persistentDataPath, SAVE_FOLDER);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
