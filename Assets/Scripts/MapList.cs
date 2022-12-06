using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject MapTypeList;

    void Start()
    {
        foreach (string worldDir in SaveManager.GetWorldDirectories())
        {
            SpawnButton(worldDir);
        }
        //새 월드
        SpawnButton(null);
        backButton.GetComponent<Button>().onClick.AddListener(OnBackButtonClick);
    }

    void OnDirectoryButtonClick(string worldName)
    {
        if (worldName == null)
        {
            gameObject.SetActive(false);
            MapTypeList.SetActive(true);
        }
        else
        {
            GlobalVariables.SelectedWorld = worldName;
            SceneManager.LoadScene("WorldEditScene", LoadSceneMode.Single);
        }
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
