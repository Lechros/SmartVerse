using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SubsystemsImplementation;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TMP_InputField roomName;
    public TMP_InputField roomPassword;
    public TMP_Dropdown worldDropdown;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItems = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    private void Start()
    {
        roomPanel.SetActive(false);

        worldDropdown.ClearOptions();
        worldDropdown.AddOptions(SaveManager.GetWorldDirectories().ToList());

        PhotonNetwork.JoinLobby();
    }

    public void OnClickOpenPanel()
    {
        roomPanel.SetActive(true);
    }

    public void OnClickCancel()
    {
        roomName.text = string.Empty;
        roomPassword.text = string.Empty;
        worldDropdown.value = 0;
        roomPanel.SetActive(false);
    }

    public void OnClickCreate()
    {
        if(roomName.text.Length == 0)
        {
            return;
        }

        ExitGames.Client.Photon.Hashtable table = new();

        bool isPassword = roomPassword.text.Length > 0;
        if(isPassword)
        {
            table.Add("secret", roomPassword.text);
        }
        // Some code to load world json and add to table
        //table.Add("world", worldList)

        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = 20;
        roomOptions.CustomRoomProperties = table;
        if(isPassword)
        {
            roomOptions.IsVisible = false;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "secret" };
        }

        PhotonNetwork.CreateRoom(roomName.text);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach(RoomItem item in roomItems)
        {
            Destroy(item.gameObject);
        }
        roomItems.Clear();

        foreach(RoomInfo room in roomList)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItems.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
}
