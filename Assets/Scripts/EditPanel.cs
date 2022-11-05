using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

public class EditPanel : MonoBehaviour, IPanel
{
    ObjectManager objectManager;
    InteractionManager interactionManager;

    [SerializeField]
    TabManager tabManager;
    [SerializeField]
    Transform editCollider;
    [SerializeField]
    TextMeshProUGUI objectName;
    [SerializeField]
    TextMeshProUGUI objectPosition;
    [SerializeField]
    TextMeshProUGUI objectRotation;
    [SerializeField]
    Button groundButton;
    [SerializeField]
    Button deleteButton;

    GameObject editObject;
    Vector3 renderOffset;
    Vector3 cursorRaycastOffset;
    bool isDragging;

    void Awake()
    {
        objectManager = SingletonManager.instance.objectManager;
        interactionManager = SingletonManager.instance.interactionManager;

        interactionManager.objectClick.AddListener(OnSelectObject);
        groundButton.onClick.AddListener(PutOnSurface);
        deleteButton.onClick.AddListener(DeleteEditObject);
    }

    void Update()
    {
        if(editObject)
        {
            // x, z movement
            if(!EventSystem.current.IsPointerOverGameObject()
                && Input.GetMouseButtonDown(0)
                && objectManager.GetRootObject(interactionManager.cursorHitInfo.collider.gameObject) == editObject)
            {
                isDragging = true;
                editCollider.position = editObject.transform.position + renderOffset;
                editCollider.gameObject.SetActive(true);

                foreach(var hit in Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)))
                {
                    if(hit.collider.transform == editCollider)
                    {
                        cursorRaycastOffset = editObject.transform.position - hit.point;
                        break;
                    }
                }
            }
            if(Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                editCollider.gameObject.SetActive(false);
            }
            if(isDragging)
            {
                editCollider.position = editObject.transform.position + renderOffset;

                foreach(var hit in Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)))
                {
                    if(hit.collider.transform == editCollider)
                    {
                        var posXZ = editObject.transform.position;
                        posXZ.x = hit.point.x + cursorRaycastOffset.x;
                        posXZ.z = hit.point.z + cursorRaycastOffset.z;
                        editObject.transform.position = posXZ;
                        break;
                    }
                }

                objectPosition.SetText(GetObjectPositionText());
            }

            // y movement
            var posY = editObject.transform.position;
            posY.y += Input.mouseScrollDelta.y * 0.1f;
            editObject.transform.position = posY;

            objectPosition.SetText(GetObjectPositionText());

            // Rotate cursor object
            if(Input.GetKeyDown(KeyCode.Q))
            {
                Vector3 pivot = editObject.transform.position - editObject.transform.rotation * renderOffset;
                editObject.transform.Rotate(Vector3.up, -30);
                editObject.transform.position = pivot + editObject.transform.rotation * renderOffset;

                objectRotation.SetText(GetObjectRotationText());
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                Vector3 pivot = editObject.transform.position - editObject.transform.rotation * renderOffset;
                editObject.transform.Rotate(Vector3.up, 30);
                editObject.transform.position = pivot + editObject.transform.rotation * renderOffset;

                objectRotation.SetText(GetObjectRotationText());
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteEditObject();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                StopEditObject();
                tabManager.SetTab(0);
            }
        }
    }

    public void OnSetActive(bool value)
    {
        if(!value)
        {
            StopEditObject();
        }
    }

    void OnSelectObject()
    {
        if(objectManager.tempObject)
        {
            return;
        }
        if(interactionManager.cursorState != InteractionManager.CursorState.Default)
        {
            return;
        }
        if(isDragging)
        {
            return;
        }

        var obj = interactionManager.cursorHitInfo.collider.gameObject;
        if(objectManager.IsSpawnedObject(obj.transform))
        {
            tabManager.SetTab(1);
            SetEditObject(obj);
        }
        else
        {
            StopEditObject();
            tabManager.SetTab(0);
        }
    }

    void SetEditObject(GameObject target)
    {
        target = objectManager.GetRootObject(target);

        if(editObject == target) return;

        StopEditObject();

        editObject = target;
        editObject.GetComponent<Outline>().color = 2;
        editObject.GetComponent<Outline>().enabled = true;

        renderOffset = Utils.GetRenderOffset(editObject);
        objectName.SetText(GetObjectNameText());
        objectPosition.SetText(GetObjectPositionText());
        objectRotation.SetText(GetObjectRotationText());
    }

    public void StopEditObject()
    {
        if(!editObject) return;

        editObject.GetComponent<Outline>().enabled = false;
        editObject = null;
    }

    void DeleteEditObject()
    {
        if(!editObject) return;

        Destroy(editObject);
        editObject = null;

        tabManager.SetTab(0);
    }

    void PutOnSurface()
    {
        if(!editObject) return;

        // Find nearest surface
        float minDistance = float.MaxValue;
        RaycastHit minDistanceHit = new RaycastHit();
        foreach(RaycastHit hit in Physics.RaycastAll(new Ray(editObject.transform.position - renderOffset, Vector3.down)))
        {
            if(hit.transform.IsChildOf(editObject.transform))
            {
                continue;
            }
            if(hit.distance < minDistance)
            {
                minDistance = hit.distance;
                minDistanceHit = hit;
            }
        }
        if(minDistance != float.MaxValue)
        {
            Vector3 halfHeight = Vector3.up * Utils.GetBounds(editObject).extents.y;
            editObject.transform.position = minDistanceHit.point + renderOffset + halfHeight;
        }
    }

    string GetObjectNameText() => editObject?.name ?? "-";

    string GetObjectPositionText() => (editObject?.transform.position ?? Vector3.zero).ToString();

    string GetObjectRotationText() => (editObject?.transform.rotation.eulerAngles ?? Quaternion.identity.eulerAngles).ToString();
}
