using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class ThirdPersonMovement : MonoBehaviourPun
{
    public CharacterController controller;

    Transform frontFacing;

    public float speed;
    public float jumpHeight;

    float turnSmoothVelocity;
    public float turnSmoothTime;

    public static GameObject LocalPlayerInstance;

    CinemachineFreeLook thirdPersonCamera;

    public float camDistance;
    public float camZoomTime;
    public float minCamDistance;
    public float maxCamDistance;
    float targetDistance;
    float camZoomVelocity = 0f;

    private void Awake()
    {
        if(!PhotonNetwork.InRoom)
        {
            Destroy(gameObject);
            return;
        }

        if(photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if(!PhotonNetwork.InRoom)
        {
            return;
        }

        if(photonView.IsMine)
        {
            frontFacing = Camera.main.transform;

            thirdPersonCamera = PlaySceneManager.instance.thirdPersonCamera;

            thirdPersonCamera.Follow = transform;
            thirdPersonCamera.LookAt = transform;

            targetDistance = camDistance;
            thirdPersonCamera.m_Orbits[0] = new CinemachineFreeLook.Orbit(camDistance, 0.5f);
            thirdPersonCamera.m_Orbits[1] = new CinemachineFreeLook.Orbit(0, camDistance);
            thirdPersonCamera.m_Orbits[2] = new CinemachineFreeLook.Orbit(-camDistance, 0.5f);
        }
    }

    Vector3 yVel;

    private void FixedUpdate()
    {
        // gravity
        if(!controller.isGrounded)
        {
            yVel += Physics.gravity * Time.deltaTime;
            controller.Move(yVel * Time.deltaTime);
        }
        else
        {
            yVel.y = 0f;
        }
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            HandleMovement();
            HandleCameraControl();
        }
    }

    void HandleMovement()
    {
        //walk
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDir = transform.rotation * new Vector3(horizontal, 0f, vertical);

        controller.Move(moveDir * speed * Time.deltaTime);

        // rotate
        if(moveDir.magnitude >= 0.1f)
        {
            float targetAngle = frontFacing.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    void HandleCameraControl()
    {
        if(!EventSystem.current.IsPointerOverGameObject() && Input.mouseScrollDelta.y != 0)
        {
            targetDistance = Mathf.Clamp(targetDistance - Input.mouseScrollDelta.y, minCamDistance, maxCamDistance);
        }
        if(Mathf.Abs(targetDistance - camDistance) > Mathf.Epsilon)
        {   
            camDistance = Mathf.SmoothDamp(camDistance, targetDistance, ref camZoomVelocity, camZoomTime);

            thirdPersonCamera.m_Orbits[0] = new CinemachineFreeLook.Orbit(camDistance, 0.5f);
            thirdPersonCamera.m_Orbits[1] = new CinemachineFreeLook.Orbit(0, camDistance);
            thirdPersonCamera.m_Orbits[2] = new CinemachineFreeLook.Orbit(-camDistance, 0.5f);
        }
    }
}
