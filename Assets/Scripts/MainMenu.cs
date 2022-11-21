using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject MapList;

    public Quaternion rot1, rot2;

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Connect", LoadSceneMode.Single);
    }

    public void OnWorldEditButtonClick()
    {
        // SceneManager.LoadScene("WorldEditScene", LoadSceneMode.Single);
        gameObject.SetActive(false);
        MapList.SetActive(true);
    }

    public void OnCharEditButtonClick()
    {
        // SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void OnExitButtonClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
