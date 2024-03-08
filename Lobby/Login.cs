using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// 로그인 관련 코드

public class Login : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject lobby, messagepos, errorTextclone, errorText, loading;
    [SerializeField]
    InputField playerName;

    public override void OnEnable()
    {
        loading.SetActive(false);
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void CreateErrorMessage(string errorMessages) // 시스템 메시지
    {
        if (errorTextclone != null)
        {
            Destroy(errorTextclone);
        }

        errorTextclone = Instantiate(errorText, messagepos.transform);
        errorTextclone.GetComponent<Text>().text = errorMessages;
    }

    public void LoginPressed() // 로그인 버튼
    {
        if (playerName.text.Length > 20)
        {
            CreateErrorMessage("아이디 글자 수가 너무 많습니다.\n 20자 이내로 해야합니다.");
        }
        else if (playerName.text == "")
        {
            loading.SetActive(true);

            PhotonNetwork.LocalPlayer.NickName = "Nobody";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            loading.SetActive(true);

            PhotonNetwork.LocalPlayer.NickName = playerName.text;
            playerName.text = "";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() // 마스터 서버 접속 성공
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // 로비 진입 성공
    {
        PhotonNetwork.RemoveCallbackTarget(this);

        lobby.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause) // 서버 접속 실패
    {
        base.OnDisconnected(cause);

        loading.SetActive(false);

        CreateErrorMessage("서버와 접속이 불가능합니다.");
    }
}
