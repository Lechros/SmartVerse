using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractionManager : MonoBehaviour
{
    public bool isCursorHit;
    public RaycastHit cursorHitInfo;
    public UnityEvent objectClick;

    public CursorState cursorState;

    public void Constructor() { }

    void Update()
    {
        isCursorHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out cursorHitInfo)
            && !EventSystem.current.IsPointerOverGameObject();

        if(isCursorHit && Input.GetMouseButtonUp(0))
        {
            objectClick.Invoke();
        }
    }

    public enum CursorState
    {
        Default,
        Placing,
        Texture,
    }
}
