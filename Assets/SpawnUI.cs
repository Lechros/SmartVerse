using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnUI : MonoBehaviour
{
    public GameObject buttonParent;
    public GameObject objectParent;
    public GameObject pageText;
    public GameObject[] pageButton = new GameObject[2];
    private string prefabPath = "Prefabs";
    private List<GameObject> buttonObjects = new List<GameObject>(); // Buttons
    private GameObject[] Prefabs; //Prefabs in PATH "Assets/Resources/prefabPath/..."
    private int currPage = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Load Prefabs in PATH "Assets/Resource/prefabPath"
        Prefabs = Resources.LoadAll<GameObject>(prefabPath);
        foreach (GameObject prefab in Prefabs)
        {
            Debug.Log(prefab);
        }

        //Insert button objects into list
        foreach (Transform child in buttonParent.transform) 
        {
            var idx = buttonObjects.Count;

            //Add Listener for Button in child, with index
            child.GetComponent<Button>().onClick.AddListener(() => SpawnOnClick(idx));

            //Add GameObject into List
            buttonObjects.Add(child.gameObject);
        }

        //Initialize button for the first time
        for (int i = 0; i < buttonObjects.Count; i++)
        {
            GameObject currButtonObject = buttonObjects[i];
            var prefabIndex = currPage * buttonObjects.Count + i;
            
            //If Prefabs[prefabIndex] exists
            if (prefabIndex < Prefabs.Length)
            {
                currButtonObject.SetActive(true);
                currButtonObject.GetComponentInChildren<Text>().text = Prefabs[prefabIndex].name;
            }
            else
            {
                currButtonObject.SetActive(false);
            }
        }

        //Add Listener to page button
        for (int i = 0; i < 2; i++)
        {
            var idx = i;
            pageButton[i].GetComponent<Button>().onClick.AddListener(() => MovePageOnClick(idx));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Spawn Objects on button click
    void SpawnOnClick(int idx)
    {
        var prefabIndex = currPage * buttonObjects.Count + idx;

        //Instantiate prefab
        //Place your own code here, right now it instantiate Object at (0, 1, 0)
        var instance = Instantiate(Prefabs[prefabIndex], new Vector3(0, 1.0f, 0), Quaternion.identity);

        instance.transform.parent = objectParent.transform;
    }

    //Move Page on Page Button click
    void MovePageOnClick(int idx)
    {
        //idx 0 - Left click, idx 1 - Right click
        switch (idx)
        {
            case 0:
                if (currPage > 0) currPage -= 1;
                break;
            case 1:
                if (currPage < (int)(Prefabs.Length / buttonObjects.Count) - 1) currPage += 1;
                break;
            default:
                break;
        }

        //Change Button Active Status and text
        for (int i = 0; i < buttonObjects.Count; i++)
        {
            GameObject currButtonObject = buttonObjects[i];
            var prefabIndex = currPage * buttonObjects.Count + i;

            //If Prefabs[prefabIndex] exists
            if (prefabIndex < Prefabs.Length)
            {
                currButtonObject.SetActive(true);
                currButtonObject.GetComponentInChildren<Text>().text = Prefabs[prefabIndex].name;
            }
            else
            {
                currButtonObject.SetActive(false);
            }
        }

        //Change Page Text
        pageText.GetComponentInChildren<Text>().text = "Page " + (currPage + 1).ToString();
    }
}
