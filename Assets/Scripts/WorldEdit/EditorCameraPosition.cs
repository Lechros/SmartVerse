using UnityEngine;

public class EditorCameraPosition : MonoBehaviour
{
    private Transform frontFacing;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        frontFacing = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = frontFacing.forward * Input.GetAxis("Vertical")
             + frontFacing.right * Input.GetAxis("Horizontal")
             + frontFacing.up * Input.GetAxis("Keyboard Y Axis");
        transform.position += Vector3.ClampMagnitude(move, 1) * moveSpeed * Time.deltaTime;
    }
}
