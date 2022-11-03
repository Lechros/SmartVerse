using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Outline = cakeslice.Outline;

public class SpawnUI : MonoBehaviour
{
    public ObjectManager ObjectManager;
    public SaveManager SaveManager;

    void Awake()
    {
        ButtonsOnAwake();
        ObjectManager.AssetReady.AddListener(ButtonsOnStart);
    }
    
    void Update()
    {
        if(IsCursorObjectSet())
        {
            // Show cursor object at mouse position
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
            {
                cursorObject.SetActive(true);

                var bounds = cursorObject.GetComponent<Renderer>()?.bounds ?? cursorObject.GetComponent<Collider>()?.bounds ?? new Bounds();
                Vector3 normal = hitInfo.normal * bounds.extents.y;
                Vector3 offset = cursorObject.transform.position - bounds.center;
                cursorObject.transform.position = hitInfo.point + offset + normal;

                if(hitInfo.collider.gameObject.CompareTag("Void"))
                {
                    cursorObject.GetComponent<Outline>().color = 1;
                    canPlace = false;
                }
                else
                {
                    cursorObject.GetComponent<Outline>().color = 0;
                    canPlace = true;
                }
            }
            else
            {
                cursorObject.SetActive(false);
            }

            // Rotate cursor object
            if(Input.GetKeyDown(KeyCode.Q))
            {
                cursorObject.transform.Rotate(Vector3.up, -30);
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                cursorObject.transform.Rotate(Vector3.up, 30);
            }

            // Place object at mouse position
            if(Input.GetMouseButtonDown(0))
            {
                PlaceCursorObject();
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                DestroyCursorObject();
            }
        }
    }

    #region ButtonUI

    private int currentPage = 0;
    private int totalPages;

    public GameObject buttonParent;
    private List<GameObject> spawnButtons = new List<GameObject>();

    public GameObject pageText;
    public GameObject leftPageButton;
    public GameObject rightPageButton;
    public GameObject saveButton;
    public GameObject loadButton;
    public GameObject leaveButton;

    void ButtonsOnAwake()
    {
        //Insert button objects into list
        foreach(Transform child in buttonParent.transform)
        {
            spawnButtons.Add(child.gameObject);
            child.GetComponent<Button>().interactable = false;
            child.GetComponentInChildren<Text>().text = "Loading...";
        }

        // Init page components
        leftPageButton.GetComponent<Button>().interactable = false;
        leftPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(-1));
        rightPageButton.GetComponent<Button>().interactable = false;
        rightPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(1));
        leaveButton.GetComponent<Button>().onClick.AddListener(OnLeaveButtonClick);
        loadButton.GetComponent<Button>().onClick.AddListener(OnLoadButtonClick);
        saveButton.GetComponent<Button>().onClick.AddListener(OnSaveButtonClick);
    }

    void ButtonsOnStart()
    {
        totalPages = Mathf.CeilToInt((float)ObjectManager.prefabs.Count / spawnButtons.Count);
        UpdateSpawnButtons();

        UpdatePageName();
        UpdateMovePageInteractable();
    }

    void UpdateSpawnButtons()
    {
        for(int i = 0; i < spawnButtons.Count; i++)
        {
            var button = spawnButtons[i];
            button.GetComponent<Button>().onClick.RemoveAllListeners();

            var prefabIndex = currentPage * spawnButtons.Count + i;
            if(prefabIndex >= ObjectManager.prefabs.Count)
            {
                button.GetComponent<Button>().interactable = false;
                button.GetComponentInChildren<Text>().text = "";
            }
            else
            {
                var prefab = ObjectManager.prefabs[prefabIndex];
                button.GetComponent<Button>().onClick.AddListener(() => OnSpawnButtonClick(prefab));
                button.GetComponent<Button>().interactable = true;
                button.GetComponentInChildren<Text>().text = prefab.name;
            }
        }
    }

    void UpdatePageName()
    {
        pageText.GetComponent<Text>().text = GetPageName();
    }

    void UpdateMovePageInteractable()
    {
        leftPageButton.GetComponent<Button>().interactable = CanSetPageTo(currentPage - 1);
        rightPageButton.GetComponent<Button>().interactable = CanSetPageTo(currentPage + 1);
    }

    void OnSpawnButtonClick(GameObject gameObject)
    {
        DestroyCursorObject();

        InitCursorObject(gameObject);
    }

    void OnMovePageClick(int addValue)
    {
        TrySetPage(currentPage + addValue);
    }

    void OnLeaveButtonClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    void OnSaveButtonClick()
    {
        SaveManager.Save("TempWorld");
    }

    void OnLoadButtonClick()
    {
        SaveManager.Load("TempWorld");
    }

    bool TrySetPage(int page)
    {
        if(!CanSetPageTo(page))
        {
            return false;
        }

        currentPage = page;
        UpdateSpawnButtons();
        UpdatePageName();
        UpdateMovePageInteractable();

        return true;
    }

    bool CanSetPageTo(int page) => page >= 0 && page < totalPages;

    string GetPageName() => $"Page {currentPage + 1}";

    #endregion

    #region CursorObject

    public Transform objectParent;
    public Transform tempObjectParent;

    /// <summary>
    /// 커서 위치에 표시되는 물체
    /// </summary>
    private GameObject cursorObject;

    private bool canPlace;

    public bool IsCursorObjectSet()
    {
        return cursorObject;
    }

    bool InitCursorObject(GameObject original)
    {
        if(cursorObject)
        {
            return false;
        }

        cursorObject = ObjectManager.Spawn(original, Vector3.zero, Quaternion.identity, tempObjectParent);
        cursorObject.GetComponent<Outline>().enabled = true;

        return true;
    }

    bool DestroyCursorObject()
    {
        if(cursorObject)
        {
            Destroy(cursorObject);
            cursorObject = null;
            return true;
        }
        return false;
    }

    bool PlaceCursorObject()
    {
        if(IsCursorObjectSet() && cursorObject.activeInHierarchy && canPlace)
        {
            ObjectManager.SetParentAndLayer(cursorObject, objectParent);
            cursorObject.GetComponent<Outline>().enabled = false;
            cursorObject = null;
            return true;
        }
        else
        {
            DestroyCursorObject();
            return false;
        }
    }

    #endregion
}
