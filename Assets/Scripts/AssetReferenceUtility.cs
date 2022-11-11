using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetReferenceUtility : MonoBehaviour
{
    public AssetReference objectToLoad;
    public AssetReference accessoryObjectToLoad;

    private GameObject instantiatedObject;
    private GameObject instantiatedAccessoryObject;


    void Start()
    {
        // 에셋을 비동기 방식으로 로드합니다.
        // 번들에서 에셋을 로드하는 코드입니다. <타입>(불러올 에셋의 키) 
        // AssetReference 전역 변수를 이용해서 유니티 에디터에서 불러올 에셋의 키를 참조할 수 있도록 할 예정입니다.
        Addressables.LoadAssetAsync<GameObject>(objectToLoad).Completed += ObjectLoadDone;
    }

    private void ObjectLoadDone(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedObject = obj.Result;
            Debug.Log("오브젝트 로드 완료");

            // Instantiate 를 통해 Hierarchy 뷰에 게임오브젝트를 생성합니다.
            instantiatedObject = Instantiate(loadedObject);
            Debug.Log("오브젝트 인스턴스화 완료");


            if (accessoryObjectToLoad != null)
            {
                // 비동기 방식으로 에셋을 로드한 다음 바로 Hierarchy 뷰에 게임오브젝트를 생성합니다.
                // 람다식으로 작성된 코드
                accessoryObjectToLoad.InstantiateAsync(instantiatedObject.transform).Completed += op =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        instantiatedAccessoryObject = op.Result;
                        Debug.Log("Accessory Object 로드와 생성 완료");
                    }
                };
            }
        }
    }
}