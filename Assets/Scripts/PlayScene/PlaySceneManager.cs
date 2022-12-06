using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour
{
    public static PlaySceneManager instance { get; private set; }

    [HideInInspector]
    public AddressableManager addressableManager;
    [HideInInspector]
    public ObjectManager objectManager;
    [HideInInspector]
    public SaveManager saveManager;
    [HideInInspector]
    public MaterialManager materialManager;

    [HideInInspector]
    public AvatarManager avatarManager;

    public GameObject playerPrefab;

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
        materialManager = FindObjectOfType<MaterialManager>();
        avatarManager = FindObjectOfType<AvatarManager>();

        addressableManager.Constructor();
        objectManager.Constructor(addressableManager);
        saveManager.Constructor(addressableManager, objectManager, materialManager);
        materialManager.Constructor(addressableManager);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LeaveOnClick()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }
}
