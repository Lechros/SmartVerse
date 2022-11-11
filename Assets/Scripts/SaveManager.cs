using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string SAVE_FOLDER = "Worlds";
    public static string SavePath { get; private set; }

    [SerializeField]
    Transform objectParent;

    ObjectManager objectManager;
    AddressableManager addressableManager;

    public void Constructor(ObjectManager objectManager)
    {
        this.objectManager = objectManager;
        this.addressableManager = objectManager.addressableManager;
    }

    private void Awake()
    {
        SavePath = Path.Join(Application.persistentDataPath, SAVE_FOLDER);
        Debug.Log(SavePath);
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
        string path = WorldNameToPath(worldName);

        // Build world data
        WorldData world = new WorldData(worldName);

        List<SvObject> objects = new List<SvObject>();
        foreach(Transform child in objectParent)
        {
            //child.gameObject.GetComponent<Renderer>().material.name = child.gameObject.GetComponent<Renderer>().material.name.Replace(" (Instance)", "");
            SvObject tempObject = new SvObject(child.gameObject.name, child.position, child.rotation, new SvMaterial(child.gameObject.GetComponent<Renderer>().sharedMaterial));
            foreach (Transform nestedchild in child)
            {
                //nestedchild.gameObject.GetComponent<Renderer>().material.name = nestedchild.gameObject.GetComponent<Renderer>().material.name.Replace(" (Instance)", "");
                var tempMaterial = new SvMaterial(nestedchild.gameObject.GetComponent<Renderer>().sharedMaterial);
                tempObject.sublist.Add(new SubObject(nestedchild.gameObject.name, tempMaterial));
            }
            objects.Add(tempObject);
        }
        world.objects = objects.ToArray();

        // Save world data to file.
        string json = JsonUtility.ToJson(world);
        if(!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
        File.WriteAllText(path, json);

        return true;
    }

    public bool Load(string worldName)
    {
        string path = WorldNameToPath(worldName);

        if(!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);
        WorldData world = JsonUtility.FromJson<WorldData>(json);

        //Destroy All Objects First
        DestroyAllObjects();

        foreach(var obj in world.objects)
        {
            var instance = objectManager.Spawn(obj.name, obj.position, obj.rotation);
            Material parentMat;
            //if (FindMaterial(obj.material, out parentMat)) instance.GetComponent<Renderer>().material = parentMat;
            foreach (Transform childObj in instance.transform)
            {
                var matchObject = obj.sublist.Find(x => x.name == childObj.gameObject.name);
                Material childMat;
                //if (FindMaterial(matchObject.material, out childMat)) childObj.gameObject.GetComponent<Renderer>().material = childMat;
            }
        }

        return true;
    }
    /*
    bool FindMaterial(SvMaterial svmat, out Material mat)
    {
        Material foundMat;
        if (svmat.name == "Standard")
        {
            var selectedMaterial = new(defaultMaterial.GetComponent<Renderer>().material.shader);
            selectedMaterial.color = colorPreview.color;
            mat = new(selectedMaterial);
            return true;
        }
        else if (addressableManager.TryGetMaterial(svmat.name, out foundMat))
        {
            mat = foundMat;
            return true;
        }
        else
        {
            mat = null;
            return false;
        }
    }
    */
    string WorldNameToPath(string worldName) => Path.Join(SavePath, worldName + ".sv");

    [Serializable]
    struct WorldData
    {
        public WorldData(string name)
        {
            this.name = name;
            this.objects = new SvObject[0];
        }

        public string name;
        public SvObject[] objects;
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
            this.sublist = new List<SubObject>();
        }

        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public SvMaterial material;
        public List<SubObject> sublist;
    }

    [Serializable]
    struct SubObject
    {
        public SubObject(string name, SvMaterial material)
        {
            this.name = name;
            this.material = material;
        }

        public string name;
        public SvMaterial material;
    }

    [Serializable]
    struct SvMaterial
    {
        //If Material is found
        public SvMaterial(Material material)
        {
            this.name = material.name;
            this.color = material.color;
        }
        public SvMaterial(string str)
        {
            this.name = str;
            this.color = new Color();
        }

        //If Material name is "Standard", not found
        public string name;
        public Color color;
    }
}
