using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject WorldEditButton;
    public GameObject CharEditButton;
    public GameObject ExitButton;

    // Start is called before the first frame update
    void Start()
    {
        // Add Listeners to buttons
        StartButton.GetComponent<Button>().onClick.AddListener(OnStartButtonClick);
        WorldEditButton.GetComponent<Button>().onClick.AddListener(OnWorldEditButtonClick);
        CharEditButton.GetComponent<Button>().onClick.AddListener(OnCharEditButtonClick);
        ExitButton.GetComponent<Button>().onClick.AddListener(OnExitButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnStartButtonClick()
    {
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Single);
    }

    void OnWorldEditButtonClick()
    {
        SceneManager.LoadScene("WorldEditScene", LoadSceneMode.Single);
    }

    void OnCharEditButtonClick()
    {
        // SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
    void OnExitButtonClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
