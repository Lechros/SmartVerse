using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapList : MonoBehaviour
{
    public GameObject templateButton;
    public GameObject backButton;
    public GameObject scrollParent;
    public GameObject MainMenu;
    void Start()
    {
        DirectoryInfo di = new DirectoryInfo(GlobalVariables.SavePath);
        foreach (DirectoryInfo subDir in di.GetDirectories())
        {
            SpawnButton(subDir.Name);
        }
        //새 월드
        SpawnButton(null);
        backButton.GetComponent<Button>().onClick.AddListener(OnBackButtonClick);
    }

    void OnDirectoryButtonClick(string worldName)
    {
        GlobalVariables.ChosenFile = worldName;
        SceneManager.LoadScene("WorldEditScene", LoadSceneMode.Single);
    }

    void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        MainMenu.SetActive(true);

    }

    void SpawnButton(string worldName)
    {
        GameObject newButton = Instantiate(templateButton);
        newButton.SetActive(true);
        newButton.transform.SetParent(scrollParent.transform);
        newButton.transform.localScale = new Vector3(1, 1, 1);
        if (worldName == null) newButton.GetComponentInChildren<Text>().text = "새 월드";
        else newButton.GetComponentInChildren<Text>().text = worldName;
        newButton.GetComponent<Button>().onClick.AddListener(() => OnDirectoryButtonClick(worldName));
    }
}
