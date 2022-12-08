using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorPanel : MonoBehaviour, IPanel
{
    ObjectManager objectManager;

    [SerializeField]
    TMP_InputField worldSizeInput;

    public float worldSize;

    void Awake()
    {
        objectManager = EditSceneManager.instance.objectManager;

        worldSizeInput.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        worldSizeInput.onEndEdit.AddListener(SetWorldSize);
        worldSizeInput.text = worldSize.ToString();
        SetWorldSize(worldSize.ToString());
    }

    void SetWorldSize(string input)
    {
        worldSize = float.Parse(input);
        objectManager.SetWorldSize(worldSize);
    }

    public void SetWorldType(string mapType)
    {
        objectManager.SetWorldType(mapType);
    }

    public void OnSetActive(bool value)
    {

    }
}
