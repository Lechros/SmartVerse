using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TexturePanel : MonoBehaviour, IPanel
{
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

    Material selectedMaterial;

    Shader standardShader;
    Shader uiDefaultShader;

    List<RawImage> materialImages;

    void Awake()
    {
        objectManager = SingletonManager.instance.objectManager;
        interactionManager = SingletonManager.instance.interactionManager;

        colorPreview.GetComponent<Button>().onClick.AddListener(SelectColorTexture);
        interactionManager.objectClick.AddListener(ApplyTextureToObject);

        standardShader = defaultMaterial.GetComponent<Renderer>().material.shader;
        uiDefaultShader = defaultUIMaterial.GetComponent<Image>().material.shader;

        materialImages = new List<RawImage>();
        foreach(Transform child in materialsParent)
        {
            materialImages.Add(child.GetComponent<RawImage>());
        }
    }

    private void Start()
    {
        foreach(var image in materialImages)
        {
            Material mat = new(image.material);
            mat.shader = uiDefaultShader;
            image.material = mat;
            var button = image.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectPreTexture(materialImages.IndexOf(image)));
        }
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

    void SelectPreTexture(int i)
    {
        selectedMaterial = new(materialImages[i].material);
        selectedMaterial.shader = standardShader;

        texturePreview.material = materialImages[i].material;

        interactionManager.cursorState = InteractionManager.CursorState.Texture;
    }
}
