using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MaterialType = MaterialManager.MaterialType;

public class SaveManager : MonoBehaviour
{
    public static string SavePath { get; private set; }

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

    public static IEnumerable<string> GetWorldDirectories()
    {
        DirectoryInfo di = new(GlobalVariables.SavePath);
        return di.GetDirectories().Select(subDir => subDir.Name);
    }

    public bool Save(string worldName)
    {
        return SaveTextToFile(worldName, WorldDataToJson(BuildWorldData(worldName)));
    }

    public bool Load(string worldName)
    {
        if(!LoadTextFromFile(worldName, out string json))
        {
            return false;
        }

        return ApplyWorldData(JsonToWorldData(json));
    }

    public WorldData BuildWorldData(string worldName)
    {
        WorldData data = new WorldData(worldName, GlobalVariables.SelectedWorldType);
        foreach(Transform child in objectManager.GetObjects())
        {
            data.objects.Add(ToSvObject(child));
        }
        return data;
    }

    public static bool SaveTextToFile(string worldName, string text)
    {
        string worldPath = WorldNameToPath(worldName);
        string worldDataPath = Path.Join(worldPath, "world.sv");

        if(!Directory.Exists(worldPath))
        {
            Directory.CreateDirectory(worldPath);
        }
        File.WriteAllText(worldDataPath, text);

        return true;
    }

    public static bool LoadTextFromFile(string worldName, out string text)
    {
        string worldPath = WorldNameToPath(worldName);
        string worldDataPath = Path.Join(worldPath, "world.sv");

        if(!File.Exists(worldDataPath))
        {
            text = default;
            return false;
        }

        text = File.ReadAllText(worldDataPath);
        return true;
    }

    public static string WorldDataToJson(WorldData data)
    {
        return JsonUtility.ToJson(data);
    }

    public static WorldData JsonToWorldData(string json)
    {
        return JsonUtility.FromJson<WorldData>(json);
    }

    public bool ApplyWorldData(WorldData data)
    {
        objectManager.DestroyAll();

        foreach(SvObject obj in data.objects)
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

        objectManager.SetWorldType(data.type);
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

    static string WorldNameToPath(string worldName) => Path.Join(SavePath, worldName);

    [Serializable]
    public struct WorldData
    {
        public WorldData(string name, string maptype)
        {
            this.name = name;
            this.type = maptype;
            this.objects = new List<SvObject>();
        }

        public string name;
        public string type;
        public List<SvObject> objects;
    }

    [Serializable]
    public struct SvObject
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
    public struct SubObject
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
    public struct SvMaterial
    {
        public static SvMaterial Default { get; } = new SvMaterial
        {
            type = MaterialType.Default,
            name = "",
            color = Color.white
        };

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
