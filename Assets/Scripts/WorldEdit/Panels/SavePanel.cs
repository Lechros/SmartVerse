using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour, IPanel
{
    SaveManager saveManager;
    public InputField saveField;

    void Awake()
    {
        saveManager = EditSceneManager.instance.saveManager;
        if (GlobalVariables.SelectedWorld != null)
            saveField.text = GlobalVariables.SelectedWorld;
    }

    public void OnSetActive(bool value)
    {

    }

    public void OnClickSave(InputField input)
    {
        saveManager.Save(input.text);
        StartCoroutine(TimedPlaceholder("ภ๚ภๅตส!", 3, input));
    }

    IEnumerator TimedPlaceholder(string message, float delay, InputField input)
    {
        var tempPlaceholder = ((Text)input.placeholder).text;
        var tempText = input.text;
        ((Text)input.placeholder).text = message;
        input.text = null;
        yield return new WaitForSeconds(delay);
        ((Text)input.placeholder).text = tempPlaceholder;
        input.text = tempText;
    }
}
