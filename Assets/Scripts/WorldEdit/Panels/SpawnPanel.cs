using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

public class SpawnPanel : MonoBehaviour, IPanel
{
    AddressableManager addressableManager;
    ObjectManager objectManager;
    InteractionManager interactionManager;

    [SerializeField]
    Transform spawnButtonParent;
    [SerializeField]
    TextMeshProUGUI pageText;
    [SerializeField]
    Button leftPageButton;
    [SerializeField]
    Button rightPageButton;

    List<Transform> spawnButtons = new List<Transform>();

    int totalPages;
    int currentPage;

    bool canPlace;

    void Awake()
    {
        addressableManager = EditSceneManager.instance.addressableManager;
        objectManager = EditSceneManager.instance.objectManager;
        interactionManager = EditSceneManager.instance.interactionManager;

        ButtonsOnAwake();
        addressableManager.listReady.AddListener(ButtonsOnStart);
    }
    
    void Update()
    {
        foreach(var button in spawnButtons)
        {
            var preview = button.GetChild(1);

            if(preview.childCount > 0)
            {
                var obj = preview.GetChild(0);
                obj.transform.RotateAround(Utils.GetBounds(obj.gameObject).center, obj.parent.transform.up, 60 * Time.deltaTime);
            }
        }

        if(objectManager.tempObject)
        {
            var obj = objectManager.tempObject;

            // Rotate cursor object (Must be before move to ensure no glitching frame)
            if(Input.GetKeyDown(KeyCode.Z))
            {
                obj.transform.Rotate(Vector3.up, -30);
            }
            if(Input.GetKeyDown(KeyCode.X))
            {
                obj.transform.Rotate(Vector3.up, 30);
            }

            // Move object
            if(interactionManager.isCursorHit)
            {
                var hitInfo = interactionManager.cursorHitInfo;

                obj.SetActive(true);
                Vector3 normal = hitInfo.normal * Utils.GetBounds(obj).extents.y;
                Vector3 offset = Utils.GetRenderOffset(objectManager.tempObject);
                obj.transform.position = hitInfo.point + offset + normal;

                if(hitInfo.collider.gameObject.CompareTag("Void"))
                {
                    obj.GetComponent<Outline>().color = 1;
                    canPlace = false;
                }
                else
                {
                    obj.GetComponent<Outline>().color = 0;
                    canPlace = true;
                }
            }
            else
            {
                obj.SetActive(false);
            }

            // Place object
            if(!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
            {
                if(canPlace)
                {
                    objectManager.PlaceTempObject();
                }
                else
                {
                    objectManager.DestoryTempObject();
                }
                interactionManager.cursorState = InteractionManager.CursorState.Default;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                objectManager.DestoryTempObject();
                interactionManager.cursorState = InteractionManager.CursorState.Default;
            }
        }
    }

    public void OnSetActive(bool value)
    {
        if(!value)
        {
            objectManager.DestoryTempObject();
            interactionManager.cursorState = InteractionManager.CursorState.Default;
        }
    }

    void ButtonsOnAwake()
    {
        foreach(Transform child in spawnButtonParent)
        {
            spawnButtons.Add(child);
        }

        // Init page components
        leftPageButton.GetComponent<Button>().interactable = false;
        leftPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(-1));
        rightPageButton.GetComponent<Button>().interactable = false;
        rightPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(1));
    }

    void ButtonsOnStart()
    {
        totalPages = Mathf.CeilToInt((float)addressableManager.prefabs.Count / spawnButtons.Count);
        currentPage = 0;

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

            var preview = button.GetChild(1);
            if(preview.childCount > 0)
            {
                Destroy(preview.GetChild(0).gameObject);
            }
            var prefabIndex = currentPage * spawnButtons.Count + i;
            if(prefabIndex >= addressableManager.prefabs.Count)
            {
                button.GetComponent<Button>().interactable = false;
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            else
            {
                var prefab = addressableManager.prefabs[prefabIndex];
                button.GetComponent<Button>().onClick.AddListener(() => OnSpawnButtonClick(prefab));
                button.GetComponent<Button>().interactable = true;
                button.GetComponentInChildren<TextMeshProUGUI>().text = prefab.name;

                var obj = Instantiate(prefab, preview);
                // set ui layer
                obj.layer = 5;
                foreach(Transform child in obj.transform)
                {
                    child.gameObject.layer = 5;
                }

                // resize
                Vector3 objSize = Utils.GetBounds(obj).size;
                float maxScaler = Mathf.Min(2 / objSize.x, 2 / objSize.y, 2 / objSize.z);
                obj.transform.localScale *= maxScaler;

                // set position
                Vector3 normal = Vector3.up * Utils.GetBounds(obj).extents.y;
                Vector3 offset = Utils.GetRenderOffset(obj);
                obj.transform.position += offset + normal;
            }
        }
    }

    void OnSpawnButtonClick(GameObject gameObject)
    {
        objectManager.SpawnTempObject(gameObject, Vector3.zero, Quaternion.identity);
        interactionManager.cursorState = InteractionManager.CursorState.Placing;
    }

    void UpdatePageName()
    {
        pageText.GetComponent<TextMeshProUGUI>().SetText(GetPageName());
    }

    void UpdateMovePageInteractable()
    {
        leftPageButton.GetComponent<Button>().interactable = CanSetPageTo(currentPage - 1);
        rightPageButton.GetComponent<Button>().interactable = CanSetPageTo(currentPage + 1);
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

    string GetPageName() => $"Page {currentPage + 1} / {totalPages}";
}
