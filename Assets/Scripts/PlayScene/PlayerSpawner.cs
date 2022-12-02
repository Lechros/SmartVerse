using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        string name = PhotonNetwork.LocalPlayer.NickName;
        // TODO : use avatar api to spawn
        GameObject playerToSpawn = null;
        PhotonNetwork.Instantiate(name, Vector3.up, Quaternion.identity);
    }
}
