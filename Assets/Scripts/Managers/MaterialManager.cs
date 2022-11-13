using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    AddressableManager addressableManager;

    public Material colorMaterial;
    public Material colorUIMaterial;
    Shader defaultShader;
    Shader defaultUIShader;

    public void Constructor(AddressableManager addressableManager)
    {
        this.addressableManager = addressableManager;
    }

    void Awake()
    {
        defaultShader = colorMaterial.shader;
        defaultUIShader = colorUIMaterial.shader;
    }

    public void SetMaterial(GameObject gameObject, Material material, MaterialType type)
    {
        gameObject.GetComponent<Renderer>().material = material;
        gameObject.GetComponent<SvInfo>().materialType = type;
    }

    public Material ConvertToUIMaterial(Material material)
    {
        Material _material = new(material);
        _material.shader = defaultUIShader;
        return _material;
    }

    public Material GetColorMaterial(Color color)
    {
        Material material = new(colorMaterial);
        material.color = color;
        return material;
    }

    public enum MaterialType
    {
        Default = 0,
        Addressable = 1,
        Color = 2,
    }
}
