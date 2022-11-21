using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform frontFacing;
    private CameraSwitch cameraSwitch;

    public float speed = 6;
    public float gravity = -9.81f;
    public float jumpHeight = 3;

    public float groundDistance = 0.4f;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private void Start()
    {
        frontFacing = Camera.main.transform;
/*        if(isLocalPlayer)
        {
            var thirdPersonCam_cmFreeLook = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
            thirdPersonCam_cmFreeLook.Follow = this.transform;
            thirdPersonCam_cmFreeLook.LookAt = this.transform;
            cameraSwitch = GameObject.Find("State Driven Camera").GetComponent<CameraSwitch>();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
/*        if(!isLocalPlayer)
        {
            return;
        }*/

        if(ShouldMove())
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
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

    bool ShouldMove()
    {
        return cameraSwitch.state == CameraSwitch.CameraState.Player;
    }
}
