using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using cakeslice;
using Outline = cakeslice.Outline;

public class SpawnUI : MonoBehaviour
{
    // Label of Addressables that we need to load
    public AssetLabelReference assetLabel;

    // Locations of Addressables (will be initialized inside InitAddressables()
    private AsyncOperationHandle<IList<IResourceLocation>> _locations;

    // Now elements in prefabs need .Result before get text, transform or anything
    private List<AsyncOperationHandle<GameObject>> prefabs = new List<AsyncOperationHandle<GameObject>>();

    void Start()
    {
        ButtonsOnAwake();
        Ready.AddListener(OnAssetsReady);
        StartCoroutine(InitAddressables());
        //InitButton will be activated after InitAddressables invoke Ready.
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

            cursorObject.transform.Rotate(Vector3.up, Input.mouseScrollDelta.y * 10);

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
    public GameObject leaveButton;
    public UnityEvent Ready;

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
    }

    void ButtonsOnStart()
    {
        totalPages = prefabs.Count / spawnButtons.Count;
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
            if(prefabIndex >= prefabs.Count)
            {
                button.GetComponent<Button>().interactable = false;
                button.GetComponentInChildren<Text>().text = "";
            }
            else
            {
                var prefab = prefabs[prefabIndex];
                button.GetComponent<Button>().onClick.AddListener(() => OnSpawnButtonClick(prefab.Result));
                button.GetComponent<Button>().interactable = true;
                button.GetComponentInChildren<Text>().text = prefab.Result.name;
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

    #endregion

    #region Addressables
    public IEnumerator InitAddressables()
    {
        // Get locations of Addressables here
        _locations = Addressables.LoadResourceLocationsAsync(assetLabel.labelString);
        yield return _locations;

        var loadOps = new List<AsyncOperationHandle>(_locations.Result.Count);
        Debug.Log(_locations.Result.Count);

        // Now we have locations for each Addressables, load assets
        foreach(IResourceLocation location in _locations.Result)
        {
            AsyncOperationHandle<GameObject> handle =
                Addressables.LoadAssetAsync<GameObject>(location);
            handle.Completed += obj =>
            {
                prefabs.Add(handle);
            };
            loadOps.Add(handle);
        }
        yield return Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true);

        // We are now ready for Initiate buttons
        Ready.Invoke();
    }

    private void OnAssetsReady()
    {
        // Activate InitButtons after all async job is done.
        ButtonsOnStart();
    }
    /*
    public IEnumerator InstantiateAll()
    {
        foreach (var location in _locations)
        {
            var instantiateOne = Addressables.InstantiateAsync(location);
            instantiateOne.Completed +=
            (handle) =>
            {
                Debug.Log(handle.Result);
                prefabs.Add(handle.Result);
            };
            yield return instantiateOne;
        }
    }
    

    public void Release()
    {
        if (_gameObjects.Count == 0)
            return;

        var index = _gameObjects.Count - 1;
        // InstantiateAsync <-> ReleaseInstance
        // Destroy함수로써 ref count가 0이면 메모리 상의 에셋을 언로드한다.
        Addressables.ReleaseInstance(_gameObjects[index]);
        _gameObjects.RemoveAt(index);
    }
    */
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

        cursorObject = Instantiate(original, new Vector3(0, 0, 0), Quaternion.identity, tempObjectParent.transform);
        cursorObject.name = original.name;
        cursorObject.layer = tempObjectParent.layer;
        if(!cursorObject.GetComponent<Collider>())
        {
            cursorObject.AddComponent<MeshCollider>();
        }
        cursorObject.AddComponent<Outline>();
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
            cursorObject.transform.SetParent(objectParent.transform);
            cursorObject.layer = objectParent.layer;
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
