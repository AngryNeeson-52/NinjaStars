using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// �α��� ���� �ڵ�

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

    public void CreateErrorMessage(string errorMessages) // �ý��� �޽���
    {
        if (errorTextclone != null)
        {
            Destroy(errorTextclone);
        }

        errorTextclone = Instantiate(errorText, messagepos.transform);
        errorTextclone.GetComponent<Text>().text = errorMessages;
    }

    public void LoginPressed() // �α��� ��ư
    {
        if (playerName.text.Length > 20)
        {
            CreateErrorMessage("���̵� ���� ���� �ʹ� �����ϴ�.\n 20�� �̳��� �ؾ��մϴ�.");
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

    public override void OnConnectedToMaster() // ������ ���� ���� ����
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // �κ� ���� ����
    {
        PhotonNetwork.RemoveCallbackTarget(this);

        lobby.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause) // ���� ���� ����
    {
        base.OnDisconnected(cause);

        loading.SetActive(false);

        CreateErrorMessage("������ ������ �Ұ����մϴ�.");
    }
}
