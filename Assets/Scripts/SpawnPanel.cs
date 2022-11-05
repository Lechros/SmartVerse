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
        addressableManager = SingletonManager.instance.addressableManager;
        objectManager = SingletonManager.instance.objectManager;
        interactionManager = SingletonManager.instance.interactionManager;

        ButtonsOnAwake();
        addressableManager.prefabReady.AddListener(ButtonsOnStart);
    }
    
    void Update()
    {
        if(objectManager.tempObject)
        {
            var obj = objectManager.tempObject;

            // Rotate cursor object (Must be before move to ensure no glitching frame)
            if(Input.GetKeyDown(KeyCode.Q))
            {
                obj.transform.Rotate(Vector3.up, -30);
            }
            if(Input.GetKeyDown(KeyCode.E))
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
