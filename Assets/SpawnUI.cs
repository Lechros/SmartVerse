using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnUI : MonoBehaviour
{
    const string PREFAB_PATH = @"Prefabs";

    private GameObject[] prefabs; //Prefabs in PATH "Assets/Resources/prefabPath/..."

    void Start()
    {
        //Load Prefabs in PATH "Assets/Resource/prefabPath"
        prefabs = Resources.LoadAll<GameObject>(PREFAB_PATH);

        InitButtons();
    }

    void Update()
    {
        if(IsCursorObjectSet())
        {
            // Show cursor object at mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // TODO: Change 1.0f to object collider size
                Vector3 normal = hitInfo.normal * 1.0f;
                cursorObject.transform.position = hitInfo.point + normal;
                cursorObject.SetActive(true);
            }
            else
            {
                cursorObject.SetActive(false);
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

    void InitButtons()
    {
        //Insert button objects into list
        foreach(Transform child in buttonParent.transform)
        {
            spawnButtons.Add(child.gameObject);
        }
        totalPages = prefabs.Length / spawnButtons.Count;
        UpdateSpawnButtons();

        // Init page components
        leftPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(-1));
        rightPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(1));
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
            if(prefabIndex >= prefabs.Length)
            {
                button.GetComponent<Button>().interactable = false;
                button.GetComponentInChildren<Text>().text = "";
            }
            else
            {
                var prefab = prefabs[prefabIndex];
                button.GetComponent<Button>().onClick.AddListener(() => OnSpawnButtonClick(prefab));
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

    public GameObject objectParent;
    public GameObject tempObjectParent;

    /// <summary>
    /// 커서 위치에 표시되는 물체
    /// </summary>
    private GameObject cursorObject;

    bool IsCursorObjectSet()
    {
        return cursorObject;
    }

    bool InitCursorObject(GameObject original)
    {
        if(cursorObject)
        {
            return false;
        }

        cursorObject = Instantiate(original, new Vector3(0, 0, 0), Quaternion.identity, tempObjectParent.transform);
        cursorObject.layer = tempObjectParent.layer;
        return true;
    }

    bool DestroyCursorObject()
    {
        if(cursorObject)
        {
            Destroy(cursorObject);
            return true;
        }
        return false;
    }

    bool PlaceCursorObject()
    {
        if(IsCursorObjectSet() && cursorObject.activeInHierarchy)
        {
            cursorObject.transform.SetParent(objectParent.transform);
            cursorObject.layer = objectParent.layer;
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
