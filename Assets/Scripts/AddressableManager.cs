using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    AssetLabelReference materialLabel;

    public IList<GameObject> prefabs;
    public IList<Material> materials;
    [HideInInspector]
    public UnityEvent listReady;
    [HideInInspector]
    public bool isListLoaded = false;

    Dictionary<string, GameObject> prefabsIndex;
    Dictionary<string, Material> materialIndex;
    AsyncOperationHandle<IList<GameObject>> loadHandle;
    AsyncOperationHandle<IList<Material>> materialHandle;

    public void Constructor() { }

    void Awake()
    {
        listReady.AddListener(() => isListLoaded = true);
    }

    public IEnumerator Start()
    {
        // Get locations of Addressables here
        var prefab_locations = Addressables.LoadResourceLocationsAsync(prefabLabel.labelString);
        yield return prefab_locations;

        loadHandle = Addressables.LoadAssetsAsync<GameObject>(prefab_locations.Result, (a) => { });
        yield return loadHandle;

        prefabs = loadHandle.Result;

        var material_locations = Addressables.LoadResourceLocationsAsync(materialLabel.labelString);
        yield return material_locations;

        materialHandle = Addressables.LoadAssetsAsync<Material>(material_locations.Result, (a) => { });
        yield return materialHandle;

        materials = materialHandle.Result;


        BuildPrefabsIndex();
        BuildTexturesIndex();

        // We are now ready for Initiate buttons
        listReady.Invoke();
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

    void BuildTexturesIndex()
    {
        materialIndex = new Dictionary<string, Material>();
        foreach (var material in materials)
        {
            materialIndex.TryAdd(material.name, material);
        }
    }
}
