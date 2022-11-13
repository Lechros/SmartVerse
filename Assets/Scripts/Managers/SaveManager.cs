using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MaterialType = MaterialManager.MaterialType;

public class SaveManager : MonoBehaviour
{
    public static string SavePath { get; private set; }

    [SerializeField]
    Transform objectParent;

    AddressableManager addressableManager;
    ObjectManager objectManager;
    MaterialManager materialManager;

    public void Constructor(AddressableManager addressableManager, ObjectManager objectManager, MaterialManager materialManager)
    {
        this.addressableManager = addressableManager;
        this.objectManager = objectManager;
        this.materialManager = materialManager;
    }

    private void Awake()
    {
        SavePath = GlobalVariables.SavePath;
    }

    void DestroyAllObjects()
    {
        foreach(Transform child in objectParent)
        {
            Destroy(child.gameObject);
        }
    }

    public bool Save(string worldName)
    {
        string fullPath = WorldNameToPath(worldName);
        string path = JoinPath(fullPath);

        // Build world data
        WorldData world = new WorldData(worldName);
        foreach(Transform child in objectParent)
        {
            world.objects.Add(ToSvObject(child));
        }

        // Save world data to file.
        string json = JsonUtility.ToJson(world);
        if(!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        File.WriteAllText(path, json);

        return true;
    }

    public bool Load(string worldName)
    {
        string fullPath = WorldNameToPath(worldName);
        string path = JoinPath(fullPath);

        if (!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);
        WorldData world = JsonUtility.FromJson<WorldData>(json);

        //Destroy All Objects First
        DestroyAllObjects();

        foreach(SvObject obj in world.objects)
        {
            GameObject gameObject = null;
            try
            {
                gameObject = SpawnFromSvObject(obj);
            }
            catch(InvalidDataException e)
            {
                Destroy(gameObject);
                Debug.LogError($"Error on loading object: {obj.name}.\n" + e.Message);
            }
        }

        return true;
    }

    SvObject ToSvObject(Transform transform)
    {
        SvObject obj = new SvObject(transform.name, transform.position, transform.rotation, GetSvMaterial(transform));
        foreach(Transform child in transform)
        {
            obj.children.Add(ToSubObject(child));
        }
        return obj;
    }

    SubObject ToSubObject(Transform transform)
    {
        SubObject obj = new SubObject(transform.name, GetSvMaterial(transform));
        foreach(Transform child in transform)
        {
            obj.children.Add(ToSubObject(child));
        }
        return obj;
    }
        
    GameObject SpawnFromSvObject(SvObject svObject)
    {
        GameObject gameObject = objectManager.Spawn(svObject.name, svObject.position, svObject.rotation);
        if(!gameObject)
        {
            throw new InvalidDataException($"Unknown prefab name: {svObject.name}.");
        }

        ApplySvMaterial(svObject.material, gameObject);

        for(int i = 0; i < svObject.children.Count; i++)
        {
            SubObject subObject = svObject.children[i];
            Transform child = gameObject.transform.GetChild(i);
            if(subObject.name != child.name)
            {
                throw new InvalidDataException($"Invalid child in object: {svObject.name}.");
            }
            ApplySubObject(subObject, child);
        }
        return gameObject;
    }

    void ApplySubObject(SubObject subObject, Transform target)
    {
        ApplySvMaterial(subObject.material, target.gameObject);

        for(int i = 0; i < subObject.children.Count; i++)
        {
            SubObject subSubObject = subObject.children[i];
            Transform child = target.transform.GetChild(i);
            if(subSubObject.name != child.name)
            {
                throw new InvalidDataException($"Invalid child in object: {subObject.name}.");
            }
            ApplySubObject(subSubObject, child);
        }
    }

    SvMaterial GetSvMaterial(Transform transform)
    {
        var type = transform.GetComponent<SvInfo>().materialType;
        switch(type)
        {
            case MaterialType.Default:
                return SvMaterial.Default;
            case MaterialType.Addressable:
                return new SvMaterial(transform.GetComponent<Renderer>().sharedMaterial.name);
            case MaterialType.Color:
                return new SvMaterial(transform.GetComponent<Renderer>().sharedMaterial.color);
            default:
                throw new Exception();
        }
    }

    bool ApplySvMaterial(SvMaterial svMaterial, GameObject target)
    {
        Material material;
        switch(svMaterial.type)
        {
            case MaterialType.Default:
                return false;
            case MaterialType.Addressable:
                if(addressableManager.TryGetMaterial(svMaterial.name, out material))
                {
                    materialManager.SetMaterial(target, material, MaterialType.Addressable);
                    return true;
                }
                else
                {
                    throw new InvalidDataException($"Unknown material name: {svMaterial.name}.");
                }
            case MaterialType.Color:
                materialManager.SetMaterial(target, materialManager.GetColorMaterial(svMaterial.color), MaterialType.Color);
                return true;
            default:
                throw new InvalidDataException($"Unknown material type: {svMaterial.type}.");
        }
    }

    string WorldNameToPath(string worldName) => Path.Join(SavePath, worldName);
    string JoinPath(string fullPath) => Path.Join(fullPath, "world.sv");

    [Serializable]
    struct WorldData
    {
        public WorldData(string name)
        {
            this.name = name;
            this.objects = new List<SvObject>();
        }

        public string name;
        public List<SvObject> objects;
    }

    [Serializable]
    struct SvObject
    {
        public SvObject(string name, Vector3 position, Quaternion rotation, SvMaterial material)
        {
            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.material = material;
            this.children = new List<SubObject>(0);
        }

        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public SvMaterial material;
        public List<SubObject> children;
    }

    [Serializable]
    struct SubObject
    {
        public SubObject(string name, SvMaterial material)
        {
            this.name = name;
            this.material = material;
            this.children = new List<SubObject>(0);
        }

        public string name;
        public SvMaterial material;
        public List<SubObject> children;
    }

    [Serializable]
    struct SvMaterial
    {
        public static SvMaterial Default { get; } = new SvMaterial("");

        public SvMaterial(string name)
        {
            this.type = MaterialType.Addressable;
            this.name = name;
            this.color = Color.white;
        }

        public SvMaterial(Color color)
        {
            this.type = MaterialType.Color;
            this.name = "";
            this.color = color;
        }
        
        public MaterialType type;
        public string name;
        public Color color;
    }
}
