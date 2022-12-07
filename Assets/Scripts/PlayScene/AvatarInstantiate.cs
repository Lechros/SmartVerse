using Photon.Pun;
using Sunbox.Avatars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarInstantiate : MonoBehaviour, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        string avatarData = (string)info.photonView.InstantiationData[0];
        Debug.Log(avatarData);
        if(!string.IsNullOrEmpty(avatarData))
        {
            PlaySceneManager.instance.avatarManager.ApplyAvatarCustomization(avatarData, info.photonView.gameObject.GetComponent<AvatarCustomization>());
        }
    }
}
