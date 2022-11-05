using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ColorHex : MonoBehaviour
{
    [SerializeField]
    ColorPicker picker;

    TextMeshProUGUI m_TextMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        m_TextMeshPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        m_TextMeshPro.SetText($"#{picker.color.ToHexString()}");
    }
}
