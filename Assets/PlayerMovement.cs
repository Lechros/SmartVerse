using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 2.0f;
    public float JumpHeight = 1.0f;

    Rigidbody rigid;
    Vector3 moveInput;
    public bool IsGrounded { get; set; }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector3(moveInput.x * MoveSpeed, rigid.velocity.y, moveInput.z * MoveSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }

    void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        if(input != null)
        {
            moveInput = new Vector3(input.x, 0f, input.y);
        }
    }

    void OnJump()
    {
        if(IsGrounded)
        {
            rigid.AddForce(-Physics.gravity * 0.5f * JumpHeight, ForceMode.VelocityChange);
        }
    }
}
