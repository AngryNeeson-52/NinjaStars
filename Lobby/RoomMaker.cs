using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

// 룸 만들기

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

    public void CreateErrorMessage(string ErrorMessages) // 시스템 메시지
    {
        if (errorTextclone != null)
        {
            Destroy(errorTextclone);
        }

        errorTextclone = Instantiate(errorText, messagepos.transform);
        errorTextclone.GetComponent<Text>().text = ErrorMessages;
    }

    public void GoLobby() // 로비로 이동
    {
        if (canclick)
        {
            mainlobby.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private bool DuplicationCheck(string inputroomname) // 방 이름 중복 확인
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

    public void RoomMake() // 룸 만들기
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

                CreateErrorMessage("방 이름이 너무 깁니다. \n 방 이름은 20자 이내여야합니다.");
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
                CreateErrorMessage("해당 방이 이미 존재합니다.");
                canclick = true;
            }
        }
    }

    public override void OnCreatedRoom() // 룸 만들기 성공시 콜백함수
    {
        mapType.text = mapTypeSet.text;
        gameRounds.text = roundSet.text;

        matchroom.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) // 룸 만들기 실패시 콜백함수
    {
        CreateErrorMessage("방 만들기를 실패했습니다.");
        canclick = true;
    }
}
