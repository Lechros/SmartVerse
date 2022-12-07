using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TMP_InputField roomName;
    public TMP_InputField roomPassword;
    public TMP_Dropdown worldDropdown;

    public GameObject enterPanel;
    public TMP_InputField enteringRoomName;
    public TMP_InputField enteringRoomPassword;

    public GameObject errorText;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItems = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    private void Start()
    {
        roomPanel.SetActive(false);
        enterPanel.SetActive(false);
        errorText.SetActive(false);

        worldDropdown.ClearOptions();
        worldDropdown.AddOptions(SaveManager.GetWorldDirectories().ToList());

        PhotonNetwork.JoinLobby();
    }

    public void OnClickOpenPanel()
    {
        roomPanel.SetActive(true);
    }

    public void OnClickOpenEnterPanel()
    {
        enterPanel.SetActive(true);
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
        // Load world json and add to table
        string worldName = worldDropdown.options[worldDropdown.value].text;
        SaveManager.LoadTextFromFile(worldName, out string dataJson);
        table.Add("world", dataJson);

        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = 20;
        roomOptions.CustomRoomProperties = table;
        if(isPassword)
        {
            roomOptions.IsVisible = false;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "secret" };
        }

        PhotonNetwork.CreateRoom(roomName.text, roomOptions);
    }

    public void OnClickEnter()
    {
        if(enteringRoomName.text.Length == 0)
        {
            return;
        }

        PhotonNetwork.JoinRoom(enteringRoomName.text);
    }

    public void OnClickEnterCancel()
    {
        enteringRoomName.text = string.Empty;
        enteringRoomPassword.text = string.Empty;
        enterPanel.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        // check password
        if(PhotonNetwork.CurrentRoom.PropertiesListedInLobby.Length > 0 && PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0] == "secret")
        {
            string secret = (string)PhotonNetwork.CurrentRoom.CustomProperties["secret"];
            if(!string.IsNullOrWhiteSpace(secret) && roomPassword.text != secret && enteringRoomPassword.text != secret)
            {
                errorText.SetActive(true);
                StartCoroutine(HideErrorText());
                PhotonNetwork.LeaveRoom();
                return;
            }
        }

        GlobalVariables.ShouldLoadWorld = true;
        SceneManager.LoadScene("PlayScene");
    }

    IEnumerator HideErrorText()
    {
        yield return new WaitForSeconds(2f);
        errorText.SetActive(false);
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

    public void OnClickExit()
    {
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.SetActive(true);
        StartCoroutine(HideErrorText());
        OnClickCancel();
        OnClickEnterCancel();
        base.OnJoinRoomFailed(returnCode, message);
    }
}
