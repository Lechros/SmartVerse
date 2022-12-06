using UnityEngine;

public class EditorCameraPosition : MonoBehaviour
{
    private Transform frontFacing;
    public float moveSpeed;
    public float fastMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        frontFacing = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float multiplier = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
        Vector3 move = frontFacing.forward * Input.GetAxis("Vertical")
             + frontFacing.right * Input.GetAxis("Horizontal")
             + frontFacing.up * Input.GetAxis("Keyboard Y Axis");
        transform.position += Vector3.ClampMagnitude(move, 1) * multiplier * Time.deltaTime;
    }
}
