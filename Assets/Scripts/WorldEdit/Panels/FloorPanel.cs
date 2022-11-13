using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorPanel : MonoBehaviour, IPanel
{
    [SerializeField]
    Transform ground;
    [SerializeField]
    TMP_InputField worldSizeInput;

    public float worldSize;

    void Awake()
    {
        worldSizeInput.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        worldSizeInput.onEndEdit.AddListener(SetWorldSize);
        worldSizeInput.text = worldSize.ToString();
        SetWorldSize(worldSize.ToString());
    }

    void SetWorldSize(string input)
    {
        worldSize = float.Parse(input);
        ground.localScale = new Vector3(worldSize / 10f, 1f, worldSize / 10f);
    }

    public void OnSetActive(bool value)
    {

    }
}
