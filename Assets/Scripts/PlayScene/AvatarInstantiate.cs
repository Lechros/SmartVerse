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

    void OnGUI()
    {
        if (true)
        {
            //Style of nametag
            var style = new GUIStyle();
            style.normal = new GUIStyleState();
            style.normal.textColor = Color.black;
            style.normal.background = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            style.alignment = TextAnchor.MiddleCenter;

            //Point
            Vector3 offset = new Vector3(0, 2.1f, 0); // height above the target position
            Vector3 point = Camera.main.WorldToScreenPoint(transform.position + offset);
            point.y = Screen.height - point.y;

            //Get PhotonView
            var photonView = gameObject.GetComponent<PhotonView>();

            //Put Label
            int xOffset = photonView.Owner.NickName.Length;
            GUI.Label(new Rect(point.x - 5 * xOffset, point.y, 10 * xOffset, 20), photonView.Owner.NickName, style);
        }
    }
}
