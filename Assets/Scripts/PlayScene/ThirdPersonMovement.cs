using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class ThirdPersonMovement : MonoBehaviourPun
{
    public CharacterController controller;
    Transform frontFacing;

    public float speed = 6;
    public float gravity = -9.81f;
    public float jumpHeight = 3;

    public float groundDistance = 0.4f;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if(photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        frontFacing = Camera.main.transform;
        if(photonView.IsMine)
        {
            var freeLook = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
            freeLook.Follow = transform;
            freeLook.LookAt = transform;
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        //walk
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + frontFacing.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
