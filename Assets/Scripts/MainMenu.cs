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
    CharEditorManager charEditorManager;
    GlobalConfig data;


    void Awake()
    {
        dataPath = new(GlobalVariables.DataPath);
        LoadGlobalConfig();
    }

    private void Start()
    {
        UpdateSelectedCharacter();
        UpdateSelectButton();
        SaveGlobalConfig();
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

    void UpdateSelectedCharacter()
    {
        data.SelectedAvatar = GlobalVariables.SelectedAvatar;
    }

    void UpdateSelectButton()
    {
        if(GlobalVariables.SelectedAvatar != null)
        {
            selectCharButton.GetComponentInChildren<Text>().text = GlobalVariables.SelectedAvatar;
        }
        else
        {
            selectCharButton.GetComponentInChildren<Text>().text = "(¾øÀ½)";
        }
    }

    public bool SaveGlobalConfig()
    {
        string path = GetDataPath(dataPath);
        string json = JsonUtility.ToJson(data);
        if(!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        File.WriteAllText(path, json);
        return true;
    }

    public bool LoadGlobalConfig()
    {
        string path = GetDataPath(dataPath);
        if(!File.Exists(path))
        {
            data = default;
            return false;
        }
        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<GlobalConfig>(json);
        GlobalVariables.SelectedAvatar = data.SelectedAvatar;
        return true;
    }

    public struct GlobalConfig
    {
        public string SelectedAvatar;
    }

    string GetDataPath(string dataPath) => Path.Join(dataPath, "data.dat");
}
