using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditUI : MonoBehaviour
{
    public GameObject SpawnUI;
    private SpawnUI spawnUIScript;

    public Transform objectParent;

    void Start()
    {
        spawnUIScript = SpawnUI.GetComponent<SpawnUI>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !spawnUIScript.IsCursorObjectSet())
        {
            if(!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
            {
                if(hitInfo.collider.transform.IsChildOf(objectParent))
                {
                    SetEditObject(hitInfo.collider.gameObject);
                }
                else
                {
                    StopEditObject();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteEditObject();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StopEditObject();
        }
    }

    private GameObject editObject;

    void SetEditObject(GameObject target)
    {
        if(editObject == target) return;

        StopEditObject();

        editObject = target;
        editObject.GetComponent<Outline>().color = 2;
        editObject.GetComponent<Outline>().enabled = true;
    }

    void DeleteEditObject()
    {
        if(!editObject) return;

        Destroy(editObject);
        editObject = null;
    }

    void StopEditObject()
    {
        if(!editObject) return;

        editObject.GetComponent<Outline>().enabled = false;
        editObject = null;
    }
}
