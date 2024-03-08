using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// �� ����
public class RoomInside : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject mainlobby, messagepos, errorTextclone, errorText;
    [SerializeField]
    Text roomname, player1, player2, startbutton, mapType, gameRounds;
    [SerializeField]
    Image player2image;
    [SerializeField]
    Sprite notready, ready, nobody;

    private bool canclick = true, allready = false;

    public override void OnEnable() // ����� �� �������̽� ����
    {
        PhotonNetwork.AddCallbackTarget(this);

        if (PhotonNetwork.IsMasterClient)
        {
            startbutton.text = "�����ϱ�";
            roomname.text = PhotonNetwork.CurrentRoom.Name;

            player1.text = PhotonNetwork.LocalPlayer.NickName;
            player2.text = "�������";
            player2image.sprite = nobody;
        }
        else
        {
            startbutton.text = "�غ�Ϸ�";
            roomname.text = PhotonNetwork.CurrentRoom.Name;

            Photon.Realtime.Player[] players = PhotonNetwork.PlayerListOthers;
            foreach (Photon.Realtime.Player player in players)
            {
                player1.text = player.NickName;
            }
            player2.text = PhotonNetwork.LocalPlayer.NickName;
            player2image.sprite = notready;
        }
    }

    public void CreateErrorMessage(string ErrorMessages) // �ý��� �޽���
    {
        if (errorTextclone != null)
        {
            Destroy(errorTextclone);
        }

        errorTextclone = Instantiate(errorText, messagepos.transform);
        errorTextclone.GetComponent<Text>().text = ErrorMessages;
    }

    public override void OnDisable() // �� ����� ���� �ʱ�ȭ
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        canclick = true;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) // ������ �濡 ������ ���
    {
        photonView.RPC("SyncRoomType", RpcTarget.All, mapType.text, gameRounds.text);

        player2image.sprite = notready;
        player2.text = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // ������ ���������
    {
        startbutton.text = "�����ϱ�";
        roomname.text = PhotonNetwork.CurrentRoom.Name;
        player1.text = PhotonNetwork.LocalPlayer.NickName;
        player2.text = "�������";
        player2image.sprite = nobody;
        allready = false;
    }

    public void RoomBreak() // �� ����
    {
        if (canclick)
        {
            canclick = false;
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom() // ���� �Ϸ�
    {
        StartCoroutine(ReJoinLobby());
    }

    IEnumerator ReJoinLobby() // ���� �� ���� �� �ʱ�ȭ �۾��� ���� ������
    {
        yield return new WaitForSeconds(1.0f);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // �κ�� ����
    {
        mainlobby.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void GameStart() // ���� ����
    {
        if (!canclick)
        {
            return;
        }

        canclick = false;

        if (PhotonNetwork.IsMasterClient)
        {
            Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

            if (players.Length <= 1)
            {
                CreateErrorMessage("��� �÷��̾ �������� �ʽ��ϴ�.");
                canclick = true;
            }
            else if (allready)
            {
                photonView.RPC("GoToGame", RpcTarget.All);
            }
            else
            {
                CreateErrorMessage("��밡 ���� �غ���� �ʾҽ��ϴ�.");
                canclick = true;
            }
        }
        else if (allready)
        {
            photonView.RPC("NotReadyImage", RpcTarget.All);
            canclick = true;
        }
        else
        {
            photonView.RPC("ReadyImage", RpcTarget.All);
            canclick = true;
        }
    }

    [PunRPC] // �� ���� ��ȯ
    void SyncRoomType(string map, string round)
    {
        mapType.text = map;
        gameRounds.text = round;
        GameManager.rounds = round;
    }

    [PunRPC]
    private void ReadyImage() // �غ� �Ϸ�
    {
        player2image.sprite = ready;
        allready = true;
    }

    [PunRPC]
    private void NotReadyImage() // �غ� �ȵ�
    {
        player2image.sprite = notready;
        allready = false;
    }

    [PunRPC]
    private void GoToGame() // ���� ������ �̵�
    {
        canclick = true;
        PhotonNetwork.LoadLevel(mapType.text);
    }
}
