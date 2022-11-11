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
        // ������ �񵿱� ������� �ε��մϴ�.
        // ���鿡�� ������ �ε��ϴ� �ڵ��Դϴ�. <Ÿ��>(�ҷ��� ������ Ű) 
        // AssetReference ���� ������ �̿��ؼ� ����Ƽ �����Ϳ��� �ҷ��� ������ Ű�� ������ �� �ֵ��� �� �����Դϴ�.
        Addressables.LoadAssetAsync<GameObject>(objectToLoad).Completed += ObjectLoadDone;
    }

    private void ObjectLoadDone(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedObject = obj.Result;
            Debug.Log("������Ʈ �ε� �Ϸ�");

            // Instantiate �� ���� Hierarchy �信 ���ӿ�����Ʈ�� �����մϴ�.
            instantiatedObject = Instantiate(loadedObject);
            Debug.Log("������Ʈ �ν��Ͻ�ȭ �Ϸ�");


            if (accessoryObjectToLoad != null)
            {
                // �񵿱� ������� ������ �ε��� ���� �ٷ� Hierarchy �信 ���ӿ�����Ʈ�� �����մϴ�.
                // ���ٽ����� �ۼ��� �ڵ�
                accessoryObjectToLoad.InstantiateAsync(instantiatedObject.transform).Completed += op =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        instantiatedAccessoryObject = op.Result;
                        Debug.Log("Accessory Object �ε�� ���� �Ϸ�");
                    }
                };
            }
        }
    }
}