using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CameraState = CameraSwitch.CameraState;

public class Editor : MonoBehaviour
{
    private Camera mainCam;
    private CameraSwitch cameraSwitch;
    public GameObject prefab;

    void Start()
    {
        mainCam = Camera.main;
        cameraSwitch = GameObject.Find("State Driven Camera").GetComponent<CameraSwitch>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            TryPlaceObjectAtMouse();
        }
    }

    bool TryPlaceObjectAtMouse()
    {
        if(cameraSwitch.state != CameraState.Editor)
        {
            return false;
        }

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        foreach(var hit in Physics.RaycastAll(ray))
        {
            if(hit.collider.CompareTag("Ground"))
            {
                PlaceObject(prefab, new Vector3(hit.point.x, 1.0f, hit.point.z), Quaternion.identity);
                return true;
            }
        }

        return false;
    }

    GameObject PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation);
    }
}
