using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
/*    [SerializeField]
    AssetLabelReference textureLabel;
    public IList<GameObject> textures;
    [HideInInspector]
    public UnityEvent textureReady;
    [HideInInspector]
    public bool isTextureLoaded = false;*/

    [SerializeField]
    AssetLabelReference prefabLabel;
    public IList<GameObject> prefabs;
    [HideInInspector]
    public UnityEvent prefabReady;
    [HideInInspector]
    public bool isPrefabLoaded = false;

    Dictionary<string, GameObject> prefabsIndex;
    AsyncOperationHandle<IList<GameObject>> loadHandle;

    public void Constructor() { }

    void Awake()
    {
        prefabReady.AddListener(() => isPrefabLoaded = true);
    }

    public IEnumerator Start()
    {
        // Get locations of Addressables here
        var locations = Addressables.LoadResourceLocationsAsync(prefabLabel.labelString);
        yield return locations;

        loadHandle = Addressables.LoadAssetsAsync<GameObject>(locations.Result, (a) => { });
        yield return loadHandle;

        prefabs = loadHandle.Result;
        BuildPrefabsIndex();

        // We are now ready for Initiate buttons
        prefabReady.Invoke();
    }

    private void OnDestroy()
    {
        Addressables.Release(loadHandle);
    }

    public bool TryGetPrefab(string name, out GameObject prefab)
    {
        return prefabsIndex.TryGetValue(name, out prefab);
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
