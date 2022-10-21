using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorCameraPosition : MonoBehaviour
{
    private Transform frontFacing;
    public float MoveSpeed;
    public float VerticalMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        frontFacing = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += frontFacing.forward * Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;
        transform.position += frontFacing.right * Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime;
        transform.position += frontFacing.up * Input.GetAxis("Keyboard Y Axis") * VerticalMoveSpeed * Time.deltaTime;
    }
}
