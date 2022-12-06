using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TMP_Text buttonText;

    public void OnClickConnect()
    {
        if(usernameInput.text.Length > 0)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Conncting...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCancelClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
