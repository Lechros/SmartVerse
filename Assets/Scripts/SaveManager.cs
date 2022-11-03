using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string SAVE_FOLDER = "Worlds";
    public static string SavePath { get; private set; }

    public ObjectManager ObjectManager;

    public Transform objectParent;

    private void Awake()
    {
        SavePath = Path.Join(Application.persistentDataPath, SAVE_FOLDER);
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
            objects.Add(new SvObject(child.gameObject.name, child.position, child.rotation));
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
            ObjectManager.Spawn(obj.name, obj.position, obj.rotation);
        }

        return true;
    }

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
        public SvObject(string name, Vector3 position, Quaternion rotation)
        {
            this.name = name;
            this.position = position;
            this.rotation = rotation;
        }

        public string name;
        public Vector3 position;
        public Quaternion rotation;
    }
}
