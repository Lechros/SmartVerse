using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CanvasManager : MonoBehaviour
{
    public GameObject SpawnUI;
    public GameObject EditUI;

    void Start()
    {
        SpawnUI.SetActive(true);
        EditUI.SetActive(true);
    }
}
