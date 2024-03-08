using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;

using ExitGames.Client.Photon;
using Photon.Chat.Demo;

// 로비 채팅 관련 코드

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


    public void OnEnable() // 로비 입장시 호출 채팅 서버 접속
    {
        chatClient = new ChatClient(this);
        chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
    }

    private void Update() // 무한 루프 채팅 수신 코루틴
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void Chating() // 채팅 실행
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

    public void MakeChat(string chats) // 채팅 적용
    {
        GameObject newText = Instantiate(newchats, chatcontent.transform);

        newText.GetComponent<Text>().text = chats;

        if (chatScroll.value < 0.1)
        {
            StartCoroutine(ScrollBarPos());
        }
    }

    IEnumerator ScrollBarPos() // 스크롤 바 위치 조정
    {
        yield return null;
        yield return null;
        chatScroll.value = 0;
    }

    public void GoToGame() // 게임으로 이동
    {
        endlobbytype = true;

        StartCoroutine(LobbyOut());
    }

    public void LogoutPressed() // 로그아웃 
    {
        loading.SetActive(true);
        endlobbytype = false;

        StartCoroutine(LobbyOut());
    }

    IEnumerator LobbyOut() //로비 퇴장 메시지 송신 후 퇴장
    {
        if (endlobbytype)
        {
            chatClient.PublishMessage("Lobby", PhotonNetwork.LocalPlayer.NickName + "님이 게임에 들어가셨습니다.");
        }
        else
        {
            chatClient.PublishMessage("Lobby", PhotonNetwork.LocalPlayer.NickName + "님이 퇴장하셨습니다.");
        }

        yield return waittime;

        chatClient.Unsubscribe(new string[] { "Lobby" });
    }

    // ichatclientlistener 필수 구현 함수
    #region
    public void DebugReturn(DebugLevel level, string message) // 디버그 발생시
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

    public void OnDisconnected() // 포톤 채팅 연결 해제 시
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

    public void OnConnected() // 포톤 채팅 연결 시
    {
        chatClient.Subscribe(new string[] { "Lobby" });
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat state changed: {state}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) // 공통 매세지 수신 시
    {
        for (int i = 0; i < messages.Length; i++)
        {
            MakeChat(messages[i] as string);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) // 개인 매세지 수신 시
    {
    }

    public void OnSubscribed(string[] channels, bool[] results) // 채팅 방 입장 시
    {
        loading.SetActive(false);
        chatClient.PublishMessage("Lobby", PhotonNetwork.LocalPlayer.NickName + "님이 입장하셨습니다.");
    }

    public void OnUnsubscribed(string[] channels) // 채팅 방 탈퇴 시
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
