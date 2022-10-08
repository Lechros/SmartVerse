using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Editor : MonoBehaviour
{
    //
    private Camera mainCam;
    public Animator animator;
    private CameraState state;
    public GameObject prefab;
    public GameObject parentObject;
    float dist;
    void Start()
    {
        mainCam = Camera.main;
        state = animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCamera") ? CameraState.Player : CameraState.Editor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }
    }

    void PlaceObject()
    {
        state = animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCamera") ? CameraState.Player : CameraState.Editor;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(-Vector3.forward, Vector3.zero);
        RaycastHit hit;
        if (state == CameraState.Editor && Physics.Raycast(ray, out hit))
        {
            GameObject instance = Instantiate(prefab);
            Vector3 tempVec = hit.point; //First, get this vector
            instance.transform.position = new Vector3(tempVec.x, 1.0f, tempVec.z);
            instance.transform.parent = parentObject.transform;
        }
    }
    enum CameraState
    {
        Editor = 0,
        Player = 1,
    }
}
