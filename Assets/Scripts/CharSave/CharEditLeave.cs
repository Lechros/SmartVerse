using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharEditLeave : MonoBehaviour
{
    public void ButtonOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
