using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// 룸 내부
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

    public override void OnEnable() // 입장시 룸 인터페이스 구축
    {
        PhotonNetwork.AddCallbackTarget(this);

        if (PhotonNetwork.IsMasterClient)
        {
            startbutton.text = "시작하기";
            roomname.text = PhotonNetwork.CurrentRoom.Name;

            player1.text = PhotonNetwork.LocalPlayer.NickName;
            player2.text = "비어있음";
            player2image.sprite = nobody;
        }
        else
        {
            startbutton.text = "준비완료";
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

    public void CreateErrorMessage(string ErrorMessages) // 시스템 메시지
    {
        if (errorTextclone != null)
        {
            Destroy(errorTextclone);
        }

        errorTextclone = Instantiate(errorText, messagepos.transform);
        errorTextclone.GetComponent<Text>().text = ErrorMessages;
    }

    public override void OnDisable() // 룸 퇴장시 변수 초기화
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        canclick = true;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) // 누군가 방에 입장한 경우
    {
        photonView.RPC("SyncRoomType", RpcTarget.All, mapType.text, gameRounds.text);

        player2image.sprite = notready;
        player2.text = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // 누군가 나갔을경우
    {
        startbutton.text = "시작하기";
        roomname.text = PhotonNetwork.CurrentRoom.Name;
        player1.text = PhotonNetwork.LocalPlayer.NickName;
        player2.text = "비어있음";
        player2image.sprite = nobody;
        allready = false;
    }

    public void RoomBreak() // 룸 퇴장
    {
        if (canclick)
        {
            canclick = false;
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom() // 퇴장 완료
    {
        StartCoroutine(ReJoinLobby());
    }

    IEnumerator ReJoinLobby() // 퇴장 후 서버 쪽 초기화 작업을 위한 딜레이
    {
        yield return new WaitForSeconds(1.0f);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // 로비로 복귀
    {
        mainlobby.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void GameStart() // 게임 시작
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
                CreateErrorMessage("상대 플레이어가 존재하지 않습니다.");
                canclick = true;
            }
            else if (allready)
            {
                photonView.RPC("GoToGame", RpcTarget.All);
            }
            else
            {
                CreateErrorMessage("상대가 아직 준비되지 않았습니다.");
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

    [PunRPC] // 맵 정보 교환
    void SyncRoomType(string map, string round)
    {
        mapType.text = map;
        gameRounds.text = round;
        GameManager.rounds = round;
    }

    [PunRPC]
    private void ReadyImage() // 준비 완료
    {
        player2image.sprite = ready;
        allready = true;
    }

    [PunRPC]
    private void NotReadyImage() // 준비 안됨
    {
        player2image.sprite = notready;
        allready = false;
    }

    [PunRPC]
    private void GoToGame() // 게임 씬으로 이동
    {
        canclick = true;
        PhotonNetwork.LoadLevel(mapType.text);
    }
}
