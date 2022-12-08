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
        //Get PhotonView
        var photonView = gameObject.GetComponent<PhotonView>();
        string name = photonView.Owner.NickName;

        //Style of nametag
        GUIContent content = new(name);
        GUIStyle style = new();
        style.normal = new GUIStyleState();
        style.normal.textColor = Color.black;
        style.normal.background = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        style.alignment = TextAnchor.MiddleCenter;

        //Point
        Vector3 offset = new Vector3(0, 2.1f, 0); // height above the target position
        Vector3 point = Camera.main.WorldToScreenPoint(transform.position + offset);
        point.y = Screen.height - point.y;

        //Put Label
        var size = style.CalcSize(content);
        Vector2 padding = new(4.0f, 2.0f);
        GUI.Label(
            new Rect(
                point.x - (size.x * 0.5f) - padding.x,
                point.y - padding.y,
                size.x + (padding.x * 2f),
                size.y + (padding.y * 2f)),
            name,
            style);
    }
}
