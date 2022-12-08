using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public TMP_Text chatArea;
    public TMP_InputField chatInput;

    ChatClient client;

    const string notifyChannel = "notify";
    const string chatChannel = "chat";

    private void Awake()
    {
        chatInput.onSubmit.AddListener((message) =>
        {
            if(message.Length > 0)
            {
                client.PublishMessage(chatChannel, message);
            }
            chatInput.text = "";
        });
    }

    private void Start()
    {
        Application.runInBackground = true;

        client = new(this);
        client.ChatRegion = "ASIA";
        client.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.NickName));
        chatArea.text = "";
    }

    private void Update()
    {
        client.Service();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnConnected()
    {
        client.Subscribe(new string[] { notifyChannel, chatChannel });
        client.SetOnlineStatus(ChatUserStatus.Online);
        client.PublishMessage(notifyChannel, $"{PhotonNetwork.NickName}님이 입장하였습니다.");
    }

    public void OnDisconnected()
    {

    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string timeStr = System.DateTime.Now.ToShortTimeString();

        for(int i = 0; i < senders.Length; i++)
        {
            switch(channelName)
            {
                case notifyChannel:
                    chatArea.text += "[" + timeStr + "] " + messages[i] + "\n";
                    break;
                case chatChannel:
                    chatArea.text += "[" + timeStr + "] " + senders[i] + " : " + messages[i] + "\n";
                    break;
                default:
                    break;
            }
            
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
}
