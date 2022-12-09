using cakeslice;
using Photon.Pun.Demo.Procedural;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    AddressableManager addressableManager;

    public Transform objectParent;
    public Transform tempObjectParent;
    public Transform wallParent;
    public Renderer floor;

    public GameObject tempObject;

    public List<Material> floorTypes;
    public List<Material> wallTypes;

    public void Constructor(AddressableManager addressableManager)
    {
        this.addressableManager = addressableManager;
    }

    private void Start()
    {
        if(!string.IsNullOrEmpty(GlobalVariables.SelectedWorldType))
        {
            SetWorldType(GlobalVariables.SelectedWorldType);
        }
    }

    public float GetWorldSize()
    {
        return floor.transform.localScale.x * 10f;
    }

    public void SetWorldSize(float size)
    {
        floor.transform.localScale = wallParent.localScale = new Vector3(size / 10f, 1f, size / 10f);
    }

    public bool SetWorldType(string worldType)
    {
        Material floorMaterial = floorTypes.Find(x => x.name == worldType);
        if(!floorMaterial)
        {
            return false;
        }
        Material wallMaterial = wallTypes.Find(x => x.name == worldType);
        if (!wallMaterial)
        {
            return false;
        }

        floor.material = floorMaterial;
        foreach (Transform wall in wallParent)
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }
        GlobalVariables.SelectedWorldType = worldType;

        return true;
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
        var newObject = Instantiate(original, position, rotation, parent);

        newObject.name = original.name;

        MeshEditor.SplitSubMesh(newObject);

        newObject.layer = parent.gameObject.layer;
        newObject.AddComponent<Outline>().enabled = false;
        newObject.AddComponent<SvInfo>();
        if(!newObject.GetComponent<Collider>())
        {
            newObject.AddComponent<MeshCollider>();
        }

        foreach(Transform child in newObject.transform)
        {
            child.gameObject.layer = parent.gameObject.layer;
            child.AddComponent<Outline>().enabled = false;
            child.AddComponent<SvInfo>();
            if(!child.GetComponent<Collider>())
            {
                child.AddComponent<MeshCollider>();
            }
        }

        return newObject;
    }

    public IList<Transform> GetObjects()
    {
        List<Transform> list = new();
        foreach(Transform t in objectParent)
        {
            list.Add(t);
        }
        return list;
    }

    public void DestroyAll()
    {
        foreach(Transform child in objectParent)
        {
            Destroy(child.gameObject);
        }
    }

    public GameObject SpawnTempObject(GameObject original, Vector3 position, Quaternion rotation)
    {
        DestoryTempObject();

        tempObject = Spawn(original, position, rotation, tempObjectParent);

        tempObject.GetComponent<Outline>().enabled = true;
        foreach(Transform child in tempObject.transform)
        {
            child.GetComponent<Outline>().enabled = true;
        }

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
        foreach(Transform child in tempObject.transform)
        {
            child.GetComponent<Outline>().enabled = false;
        }
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
