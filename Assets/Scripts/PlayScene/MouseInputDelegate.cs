using Cinemachine;
using UnityEngine;

public class MouseInputDelegate : MonoBehaviour
{
    bool isRightDown;

    void Start()
    {
        CinemachineCore.GetInputAxis = GetInputAxisDelegate;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            isRightDown = true;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            isRightDown = false;
        }
    }

    float GetInputAxisDelegate(string axisName)
    {
        // These 2 axis only activate when mouse right button is in down state
        if(axisName == "Mouse X Right Down")
        {
            return isRightDown ? Input.GetAxis("Mouse X") : 0;
        }
        else if(axisName == "Mouse Y Right Down")
        {
            return isRightDown ? Input.GetAxis("Mouse Y") : 0;
        }

        return Input.GetAxis(axisName);
    }
}
