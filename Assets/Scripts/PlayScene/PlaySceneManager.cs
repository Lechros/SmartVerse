using JetBrains.Annotations;
using Photon.Pun;
using Sunbox.Avatars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour, IPunInstantiateMagicCallback
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
            if(!string.IsNullOrEmpty(dataJson))
            {
                SaveManager.WorldData data = SaveManager.JsonToWorldData(dataJson);
                addressableManager.listReady.AddListener(() => saveManager.ApplyWorldData(data));
                GlobalVariables.ShouldLoadWorld = false;
            }
        }

        if(ThirdPersonMovement.LocalPlayerInstance == null)
        {
            string charData = string.Empty;
            if(!string.IsNullOrEmpty(GlobalVariables.SelectedAvatar))
            {
                charData = avatarManager.LoadCharData(GlobalVariables.SelectedAvatar);
            }
            object[] initData = new string[]
            {
                charData
            };
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5.0f, Quaternion.identity, 0, initData);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        string avatarData = (string)info.photonView.InstantiationData[0];
        if(!string.IsNullOrEmpty(avatarData))
        {
            avatarManager.ApplyAvatarCustomization(avatarData, info.photonView.gameObject.GetComponent<AvatarCustomization>());
        }
    }

    public void LeaveOnClick()
    {
        Destroy(ThirdPersonMovement.LocalPlayerInstance);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }
}
