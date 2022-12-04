using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public GameObject MapList;
    public GameObject CharacterList;
    public GameObject selectCharButton;
    string dataPath;
    CharSave charSave;
    DataStruct data;

    
    void Awake()
    {
        dataPath = new(GlobalVariables.DataPath);
        LoadData();
    }

    void OnEnable()
    {
        UpdateData();
        UpdateSelectButton();
        SaveData();
    }
    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Connect", LoadSceneMode.Single);
    }

    public void OnWorldEditButtonClick()
    {
        gameObject.SetActive(false);
        MapList.SetActive(true);
    }

    public void OnCharEditButtonClick()
    {
        SceneManager.LoadScene("CharEditScene", LoadSceneMode.Single);
    }

    public void OnExitButtonClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void OnSelectCharacterButtonClick()
    {
        gameObject.SetActive(false);
        CharacterList.SetActive(true);
    }

    void UpdateData()
    {
        data.chosenCharacter = GlobalVariables.ChosenCharacter;
    }
    void UpdateSelectButton()
    {
        if (GlobalVariables.ChosenCharacter != null)
            selectCharButton.GetComponentInChildren<Text>().text = GlobalVariables.ChosenCharacter;
        else
            selectCharButton.GetComponentInChildren<Text>().text = "¾øÀ½";
    }

    public bool SaveData()
    {
        string path = GetDataPath(dataPath);
        string json = JsonUtility.ToJson(data);
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        File.WriteAllText(path, json);
        return true;
    }

    public bool LoadData()
    {
        string path = GetDataPath(dataPath);
        if (!File.Exists(path))
        {
            data = default;
            return false;
        }
        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<DataStruct>(json);
        // Assigning Data to GlobalVariables
        GlobalVariables.ChosenCharacter = data.chosenCharacter;
        // End of Assign
        return true;
    }
    public struct DataStruct
    {
        public bool initialized;
        public string chosenCharacter;
    }
    string GetDataPath(string dataPath) => Path.Join(dataPath, "data.dat");
}
