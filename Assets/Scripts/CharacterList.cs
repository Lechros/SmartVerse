using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour
{
    public GameObject templateButton;
    public GameObject backButton;
    public GameObject scrollParent;
    public GameObject mainMenu;

    void Start()
    {
        foreach (string charDir in GetCharacterFiles())
        {
            SpawnButton(charDir);
        }
        //새 월드
        //SpawnButton(null);
        backButton.GetComponent<Button>().onClick.AddListener(OnBackButtonClick);
    }

    void OnAvatarButtonClick(string avatarName)
    {
        mainMenu.GetComponent<MainMenu>().SelectAvatar(avatarName);
        ReturnToMainMenu();
    }

    void OnBackButtonClick()
    {
        ReturnToMainMenu();
    }

    void ReturnToMainMenu()
    {
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    void SpawnButton(string charName)
    {
        GameObject newButton = Instantiate(templateButton);
        newButton.SetActive(true);
        newButton.transform.SetParent(scrollParent.transform);
        newButton.transform.localScale = new Vector3(1, 1, 1);
        if (charName == null) newButton.GetComponentInChildren<Text>().text = "캐릭터 없음";
        else newButton.GetComponentInChildren<Text>().text = charName;
        newButton.GetComponent<Button>().onClick.AddListener(() => OnAvatarButtonClick(charName));
    }

    public static IEnumerable<string> GetCharacterFiles()
    {
        DirectoryInfo di = new(GlobalVariables.CharacterPath);
        if (!Directory.Exists(GlobalVariables.CharacterPath))
        {
            Directory.CreateDirectory(GlobalVariables.CharacterPath);
        }
        return di.GetFiles("*.sv").Select(file => Path.GetFileNameWithoutExtension(file.Name));
    }

}
