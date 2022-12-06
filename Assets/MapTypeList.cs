using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTypeList : MonoBehaviour
{
    public GameObject MapList;
    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        MapList.SetActive(true);
    }

    public void OnTypeButtonClick(string type)
    {
        GlobalVariables.SelectedWorldType = type;
        SceneManager.LoadScene("WorldEditScene", LoadSceneMode.Single);
    }
}
