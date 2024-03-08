using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;

using ExitGames.Client.Photon;
using Photon.Chat.Demo;

// �κ� ä�� ���� �ڵ�

public class LobbyChat : MonoBehaviour, IChatClientListener
{
    public ChatClient chatClient;

    //LobbyRooms LR;

    [SerializeField]
    GameObject chatcontent, newchats, loading;
    [SerializeField]
    InputField chatinput;
    [SerializeField]
    Scrollbar chatScroll;

    bool endlobbytype = false;

    WaitForSeconds waittime = new WaitForSeconds(1.0f);


    public void OnEnable() // �κ� ����� ȣ�� ä�� ���� ����
    {
        chatClient = new ChatClient(this);
        chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
    }

    private void Update() // ���� ���� ä�� ���� �ڷ�ƾ
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void Chating() // ä�� ����
    {
        if (chatinput.text != "" && chatinput.text != "\n")
        {
            string chats = PhotonNetwork.LocalPlayer.NickName + " : " + chatinput.text;
            chatClient.PublishMessage("Lobby", chats);
            chatScroll.value = 0;
        }

        chatinput.ActivateInputField();
        chatinput.text = "";
    }

    public void MakeChat(string chats) // ä�� ����
    {
        GameObject newText = Instantiate(newchats, chatcontent.transform);

        newText.GetComponent<Text>().text = chats;

        if (chatScroll.value < 0.1)
        {
            StartCoroutine(ScrollBarPos());
        }
    }

    IEnumerator ScrollBarPos() // ��ũ�� �� ��ġ ����
    {
        yield return null;
        yield return null;
        chatScroll.value = 0;
    }

    public void GoToGame() // �������� �̵�
    {
        endlobbytype = true;

        StartCoroutine(LobbyOut());
    }

    public void LogoutPressed() // �α׾ƿ� 
    {
        loading.SetActive(true);
        endlobbytype = false;

        StartCoroutine(LobbyOut());
    }

    IEnumerator LobbyOut() //�κ� ���� �޽��� �۽� �� ����
    {
        if (endlobbytype)
        {
            chatClient.PublishMessage("Lobby", PhotonNetwork.LocalPlayer.NickName + "���� ���ӿ� ���̽��ϴ�.");
        }
        else
        {
            chatClient.PublishMessage("Lobby", PhotonNetwork.LocalPlayer.NickName + "���� �����ϼ̽��ϴ�.");
        }

        yield return waittime;

        chatClient.Unsubscribe(new string[] { "Lobby" });
    }

    // ichatclientlistener �ʼ� ���� �Լ�
    #region
    public void DebugReturn(DebugLevel level, string message) // ����� �߻���
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnDisconnected() // ���� ä�� ���� ���� ��
    {
        StopAllCoroutines();

        if (endlobbytype)
        {

        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void OnConnected() // ���� ä�� ���� ��
    {
        chatClient.Subscribe(new string[] { "Lobby" });
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat state changed: {state}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) // ���� �ż��� ���� ��
    {
        for (int i = 0; i < messages.Length; i++)
        {
            MakeChat(messages[i] as string);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) // ���� �ż��� ���� ��
    {
    }

    public void OnSubscribed(string[] channels, bool[] results) // ä�� �� ���� ��
    {
        loading.SetActive(false);
        chatClient.PublishMessage("Lobby", PhotonNetwork.LocalPlayer.NickName + "���� �����ϼ̽��ϴ�.");
    }

    public void OnUnsubscribed(string[] channels) // ä�� �� Ż�� ��
    {
        chatClient.Disconnect();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }
    public void OnUserSubscribed(string channel, string user)
    {
    }
    public void OnUserUnsubscribed(string channel, string user)
    {
    }
    #endregion

}
