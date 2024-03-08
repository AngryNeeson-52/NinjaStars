using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

// �� �����

public class RoomMaker : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject mainlobby, errorText, errorTextclone, messagepos, matchroom;
    [SerializeField]
    InputField roomName;
    [SerializeField]
    Text mapTypeSet, roundSet, mapType, gameRounds;

    LobbyRooms LR;

    private bool canclick = true;

    public override void OnEnable()
    {
        LR = LobbyRooms.instance;
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        canclick = true;
        PhotonNetwork.RemoveCallbackTarget(this);
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

    public void GoLobby() // �κ�� �̵�
    {
        if (canclick)
        {
            mainlobby.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private bool DuplicationCheck(string inputroomname) // �� �̸� �ߺ� Ȯ��
    {
        foreach (string rn in LR.rooms)
        {
            if (rn == inputroomname)
            {
                return false;
            }
        }

        return true;
    }

    public void RoomMake() // �� �����
    {
        if (canclick)
        {
            canclick = false;

            string inputroomname = roomName.text;

            if (inputroomname == "")
            {
                inputroomname = PhotonNetwork.LocalPlayer.NickName;
            }

            if (inputroomname.Length > 20)
            {

                CreateErrorMessage("�� �̸��� �ʹ� ��ϴ�. \n �� �̸��� 20�� �̳������մϴ�.");
                canclick = true;
            }
            else if(DuplicationCheck(inputroomname))
            {
                RoomOptions roomOptions = new RoomOptions
                {
                    MaxPlayers = 2,
                    IsOpen = true,
                    IsVisible = true
                };

                PhotonNetwork.CreateRoom(inputroomname, roomOptions);
            }
            else
            {
                CreateErrorMessage("�ش� ���� �̹� �����մϴ�.");
                canclick = true;
            }
        }
    }

    public override void OnCreatedRoom() // �� ����� ������ �ݹ��Լ�
    {
        mapType.text = mapTypeSet.text;
        gameRounds.text = roundSet.text;

        matchroom.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) // �� ����� ���н� �ݹ��Լ�
    {
        CreateErrorMessage("�� ����⸦ �����߽��ϴ�.");
        canclick = true;
    }
}
