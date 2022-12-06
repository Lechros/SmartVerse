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

    void Awake()
    {
        dataPath = new(GlobalVariables.DataPath);
    }

    private void Start()
    {
        LoadGlobalConfig();
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

    public void SelectAvatar(string avatar)
    {
        GlobalVariables.SelectedAvatar = avatar;
        selectCharButton.GetComponentInChildren<Text>().text = string.IsNullOrEmpty(avatar) ? "(����)" : avatar;
    }

    public bool SaveGlobalConfig()
    {
        Preferences pre = new Preferences() { SelectedAvatar = GlobalVariables.SelectedAvatar };
        string path = GetDataPath(dataPath);
        string json = JsonUtility.ToJson(pre);
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
            return false;
        }
        string json = File.ReadAllText(path);
        Preferences pre = JsonUtility.FromJson<Preferences>(json);

        SelectAvatar(pre.SelectedAvatar);

        return true;
    }

    public struct Preferences
    {
        public string SelectedAvatar;
    }

    string GetDataPath(string dataPath) => Path.Join(dataPath, "data.dat");
}
