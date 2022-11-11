using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TexturePanel : MonoBehaviour, IPanel
{
    AddressableManager addressableManager;
    ObjectManager objectManager;
    InteractionManager interactionManager;

    [SerializeField]
    Transform defaultMaterial;
    [SerializeField]
    Transform defaultUIMaterial;
    [SerializeField]
    RawImage texturePreview;
    [SerializeField]
    RawImage oldColorPreview;
    [SerializeField]
    RawImage colorPreview;

    [SerializeField]
    Transform materialsParent;
    [SerializeField]
    TextMeshProUGUI pageText;
    [SerializeField]
    Button leftPageButton;
    [SerializeField]
    Button rightPageButton;

    int totalPages;
    int currentPage;

    Shader standardShader;
    Shader uiDefaultShader;

    Material selectedMaterial;

    List<RawImage> materialImages;

    void Awake()
    {
        addressableManager = SingletonManager.instance.addressableManager;
        objectManager = SingletonManager.instance.objectManager;
        interactionManager = SingletonManager.instance.interactionManager;

        colorPreview.GetComponent<Button>().onClick.AddListener(SelectColorTexture);
        interactionManager.objectClick.AddListener(ApplyTextureToObject);

        standardShader = defaultMaterial.GetComponent<Renderer>().material.shader;
        uiDefaultShader = defaultUIMaterial.GetComponent<Image>().material.shader;

        ButtonsOnAwake();
        addressableManager.listReady.AddListener(ButtonsOnStart);
    }

    private void Start()
    {
        ButtonsOnStart();
    }

    void ButtonsOnAwake()
    {
        materialImages = new List<RawImage>();
        foreach(Transform child in materialsParent)
        {
            materialImages.Add(child.GetComponent<RawImage>());
        }

        // Init page components
        leftPageButton.GetComponent<Button>().interactable = false;
        leftPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(-1));
        rightPageButton.GetComponent<Button>().interactable = false;
        rightPageButton.GetComponent<Button>().onClick.AddListener(() => OnMovePageClick(1));
    }

    void ButtonsOnStart()
    {
        totalPages = Mathf.CeilToInt((float)addressableManager.materials.Count / materialImages.Count);
        currentPage = 0;

        UpdateMaterialButtons();

        UpdatePageName();
        UpdateMovePageInteractable();
    }

    void UpdateMaterialButtons()
    {
        for(int i = 0; i < materialImages.Count; i++)
        {
            RawImage imageButton = materialImages[i];
            imageButton.GetComponent<Button>().onClick.RemoveAllListeners();

            var materialIndex = currentPage * materialImages.Count + i;
            if(materialIndex >= addressableManager.materials.Count)
            {
                imageButton.material = null;
                imageButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                var material = addressableManager.materials[materialIndex];
                Material displayMaterial = new(material);
                displayMaterial.shader = uiDefaultShader;
                imageButton.material = displayMaterial;
                imageButton.GetComponent<Button>().onClick.AddListener(() => OnMaterialButtonClick(material, displayMaterial));
                imageButton.GetComponent<Button>().interactable = true;
            }
        }
    }

    void OnMaterialButtonClick(Material material, Material displayMaterial)
    {
        selectedMaterial = material;
        texturePreview.material = displayMaterial;
        interactionManager.cursorState = InteractionManager.CursorState.Texture;
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

    public void OnSetActive(bool value)
    {
        if(!value)
        {
            interactionManager.cursorState = InteractionManager.CursorState.Default;
        }
        else
        {
            interactionManager.cursorState = InteractionManager.CursorState.Texture;
        }
    }

    void ApplyTextureToObject()
    {
        if(interactionManager.cursorState != InteractionManager.CursorState.Texture)
        {
            return;
        }
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var obj = interactionManager.cursorHitInfo.collider.transform;
        if(!objectManager.IsSpawnedObject(obj))
        {
            return;
        }
        obj.GetComponent<Renderer>().material = selectedMaterial;
    }

    void SelectColorTexture()
    {
        oldColorPreview.color = colorPreview.color;

        selectedMaterial = new(standardShader);
        selectedMaterial.color = colorPreview.color;

        Material mat = new(selectedMaterial);
        mat.shader = uiDefaultShader;
        texturePreview.material = mat;

        interactionManager.cursorState = InteractionManager.CursorState.Texture;
    }

    bool TrySetPage(int page)
    {
        if(!CanSetPageTo(page))
        {
            return false;
        }

        currentPage = page;
        UpdateMaterialButtons();
        UpdatePageName();
        UpdateMovePageInteractable();

        return true;
    }

    bool CanSetPageTo(int page) => page >= 0 && page < totalPages;

    string GetPageName() => $"Page {currentPage + 1} / {totalPages}";
}
