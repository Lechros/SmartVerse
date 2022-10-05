using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField]
    private InputAction action;

    private Animator animator;
    private CameraState state;

    private void Start()
    {
        animator = GetComponent<Animator>();
        state = animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCamera") ? CameraState.Player : CameraState.Editor;

        action.performed += _ => SwitchState();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void SwitchState()
    {
        switch(state)
        {
            case CameraState.Player:
                animator.Play("EditorCamera");
                Cursor.lockState = CursorLockMode.None;
                state = CameraState.Editor;
                break;
            case CameraState.Editor:
                animator.Play("PlayerCamera");
                Cursor.lockState = CursorLockMode.Locked;
                state = CameraState.Player;
                break;
        }
    }

    void Update()
    {

    }

    enum CameraState
    {
        Editor = 0,
        Player = 1,
    }
}