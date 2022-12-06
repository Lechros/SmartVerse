using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditSceneManager : MonoBehaviour
{
    public static EditSceneManager instance { get; private set; }

    [HideInInspector]
    public AddressableManager addressableManager;
    [HideInInspector]
    public ObjectManager objectManager;
    [HideInInspector]
    public SaveManager saveManager;
    [HideInInspector]
    public InteractionManager interactionManager;
    [HideInInspector]
    public MaterialManager materialManager;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }

        addressableManager = FindObjectOfType<AddressableManager>();
        objectManager = FindObjectOfType<ObjectManager>();
        saveManager = FindObjectOfType<SaveManager>();
        interactionManager = FindObjectOfType<InteractionManager>();
        materialManager = FindObjectOfType<MaterialManager>();

        addressableManager.Constructor();
        objectManager.Constructor(addressableManager);
        saveManager.Constructor(addressableManager, objectManager, materialManager);
        interactionManager.Constructor();
        materialManager.Constructor(addressableManager);

        if(GlobalVariables.SelectedWorld != null)
        {
            addressableManager.listReady.AddListener(() => saveManager.Load(GlobalVariables.SelectedWorld));
        }
    }

    public void LeaveOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
