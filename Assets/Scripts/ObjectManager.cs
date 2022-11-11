using cakeslice;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    AddressableManager addressableManager;

    [SerializeField]
    Transform objectParent;
    [SerializeField]
    Transform tempObjectParent;

    public GameObject tempObject;

    public void Constructor(AddressableManager addressableManager)
    {
        this.addressableManager = addressableManager;
    }

    public GameObject Spawn(string name, Vector3 position, Quaternion rotation)
    {
        if(!addressableManager.TryGetPrefab(name, out GameObject prefab))
        {
            return null;
        }

        return Spawn(prefab, position, rotation);
    }

    public GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation)
    {
        return Spawn(original, position, rotation, objectParent);
    }

    public GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
    {
        var obj = Instantiate(original, position, rotation, parent);

        obj.name = original.name;

        obj.layer = parent.gameObject.layer;
        if(!obj.GetComponent<Collider>())
        {
            obj.AddComponent<MeshCollider>();
        }
        foreach(Transform child in obj.transform)
        {
            child.gameObject.layer = parent.gameObject.layer;
            if(!child.GetComponent<Collider>())
            {
                child.AddComponent<MeshCollider>();
            }
        }
        obj.AddComponent<Outline>().enabled = false;

        return obj;
    }

    public GameObject SpawnTempObject(GameObject original, Vector3 position, Quaternion rotation)
    {
        DestoryTempObject();

        tempObject = Spawn(original, position, rotation, tempObjectParent);

        tempObject.GetComponent<Outline>().enabled = true;

        return tempObject;
    }

    public bool PlaceTempObject()
    {
        if(!tempObject)
        {
            return false;
        }

        SetParentAndLayer(tempObject, objectParent);
        tempObject.GetComponent<Outline>().enabled = false;
        tempObject = null;
        return true;
    }

    public bool DestoryTempObject()
    {
        if(!tempObject)
        {
            return false;
        }

        Destroy(tempObject);
        tempObject = null;
        return true;
    }

    public bool IsSpawnedObject(Transform transform)
    {
        return transform.IsChildOf(objectParent);
    }

    public GameObject GetRootObject(GameObject gameObject)
    {
        if(!IsSpawnedObject(gameObject.transform))
        {
            return gameObject;
        }

        Transform transform = gameObject.transform;
        while(transform.parent != objectParent)
        {
            transform = transform.parent;
        }
        return transform.gameObject;
    }

    public void SetParentAndLayer(GameObject child, Transform parent)
    {
        child.transform.SetParent(parent.transform);
        child.gameObject.layer = parent.gameObject.layer;
        foreach(Transform gChild in child.transform)
        {
            gChild.gameObject.layer = parent.gameObject.layer;
        }
    }
}
