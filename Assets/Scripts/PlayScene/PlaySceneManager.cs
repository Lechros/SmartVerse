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

    private void Start()
    {
        if(GlobalVariables.ShouldLoadWorld)
        {
            string dataJson = PhotonNetwork.CurrentRoom.CustomProperties["world"] as string;
            SaveManager.WorldData data = SaveManager.JsonToWorldData(dataJson);
            addressableManager.listReady.AddListener(() => saveManager.ApplyWorldData(data));
            GlobalVariables.ShouldLoadWorld = false;
        }

        if(ThirdPersonMovement.LocalPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5.0f, Quaternion.identity, 0);
        }
    }

    public void LeaveOnClick()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }
}
