using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Events;
using System.Collections;
using cakeslice;

public class ObjectManager : MonoBehaviour
{
    public Transform objectParent;

    // Label of Addressables that we need to load
    public AssetLabelReference assetLabel;

    public IList<GameObject> prefabs;
    Dictionary<string, GameObject> prefabsIndex;

    AsyncOperationHandle<IList<GameObject>> loadHandle;

/*    // Now elements in prefabs need .Result before get text, transform or anything
    List<AsyncOperationHandle<GameObject>> prefabs = new List<AsyncOperationHandle<GameObject>>();*/

    public UnityEvent AssetReady;

    public bool isLoaded = false;

    void Awake()
    {
        prefabs = new List<GameObject>();
        AssetReady.AddListener(() => isLoaded = true);
    }

    public IEnumerator Start()
    {
        // Get locations of Addressables here
        var locations = Addressables.LoadResourceLocationsAsync(assetLabel.labelString);
        yield return locations;

        loadHandle = Addressables.LoadAssetsAsync<GameObject>(locations.Result, (a) => { });

        yield return loadHandle;

        prefabs = loadHandle.Result;
        BuildPrefabsIndex();

        /*        List<AsyncOperationHandle> handles = new();

                // Now we have locations for each Addressables, load assets
                foreach(IResourceLocation location in locations.Result)
                {
                    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(location);
                    handle.Completed += (obj) => prefabs.Add(handle);
                    handles.Add(handle);
                }
                yield return Addressables.ResourceManager.CreateGenericGroupOperation(handles, true);*/

        // We are now ready for Initiate buttons
        AssetReady.Invoke();
    }

    private void OnDestroy()
    {
        Addressables.Release(loadHandle);
    }

    public GameObject Spawn(string name)
    {
        return Spawn(name, Vector3.zero, Quaternion.identity);
    }

    public GameObject Spawn(string name, Vector3 position, Quaternion rotation)
    {
        if(!prefabsIndex.TryGetValue(name, out GameObject prefab))
        {
            return null;
        }

        return Spawn(prefab, position, rotation);
    }

    public GameObject Spawn(GameObject original)
    {
        return Spawn(original, original.transform.position, original.transform.rotation);
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
        obj.AddComponent<Outline>().enabled = false;

        return obj;
    }

    public void SetParentAndLayer(GameObject child, Transform parent)
    {
        child.transform.SetParent(parent.transform);
        child.gameObject.layer = parent.gameObject.layer;
    }

    void BuildPrefabsIndex()
    {
        prefabsIndex = new Dictionary<string, GameObject>();
        foreach(var prefab in prefabs)
        {
            prefabsIndex.TryAdd(prefab.name, prefab);
        }
    }
}
