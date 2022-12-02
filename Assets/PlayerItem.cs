using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TMP_Text playerName;

    ExitGames.Client.Photon.Hashtable playerProperties = new();

    Player player;

    private void Awake()
    {
        // TODO : Send my avatar info
        playerProperties["avatar"] = "Liam";
    }

    public void SetPlayerInfo(Player player)
    {
        this.player = player;
        playerName.text = player.NickName;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable playerProperties)
    {
        if(player == targetPlayer)
        {

        }
    }

    void UpdatePlayerItem(Player player)
    {
        if(player.CustomProperties.ContainsKey("avatar"))
        {
        }
    }
}
