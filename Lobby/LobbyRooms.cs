using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

// �κ��� �� ���� ���� �ڵ�
// ���� chat, pun �ΰ��� ������ �̿��ϱ� ������ �ΰ��� �ݹ��� �޾ƾ���

public class LobbyRooms : MonoBehaviourPunCallbacks
{
    public static LobbyRooms instance;

    [SerializeField]
    GameObject[] button;
    [SerializeField]
    Text[] buttonText, roomPerson;
    [SerializeField]
    GameObject LeftButton, RightButton, messagepos, matchRoom, mainLobby, roomMaker;
    [SerializeField]
    Text pagestext, errorTextclone, errorText;

    public List<string> rooms = new List<string>();
    public List<int> roomPop = new List<int>();

    int roomsCount, pages, currentpages = 0;
    bool canclick = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        canclick = true;
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // �� ����Ʈ ������Ʈ
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo currentRoom = roomList[i];

            if (!currentRoom.RemovedFromList)
            {
                if (!rooms.Contains(currentRoom.Name))
                {
                    rooms.Add(currentRoom.Name);
                    roomPop.Add(currentRoom.PlayerCount);
                }
                else
                {
                    rooms[rooms.IndexOf(currentRoom.Name)] = currentRoom.Name;
                    roomPop[rooms.IndexOf(currentRoom.Name)] = currentRoom.PlayerCount;
                }
            }
            else if (rooms.Contains(currentRoom.Name))
            {
                roomPop[rooms.IndexOf(currentRoom.Name)] = -1;
                roomPop.Remove(-1);
                rooms.Remove(currentRoom.Name);
            }
        }

        roomsCount = rooms.Count;
        if (roomsCount < 4)
        {
            pages = 1;
        }
        else if (roomsCount % 4 > 0)
        {
            pages = (roomsCount / 4) + 1;
        }
        else
        {
            pages = roomsCount / 4;
        }

        currentpages = 1;
        pagestext.text = currentpages + " / " + pages;

        RoomListUpdate();
    }

    public void RoomListUpdate() // �¿� ��ư Ȱ��ȭ & ���ΰ�ħ ��ư
    {
        if (currentpages < pages)
        {
            RightButton.SetActive(true);
        }
        else
        {
            RightButton.SetActive(false);
        }

        if (currentpages > 1)
        {
            LeftButton.SetActive(true);
        }
        else
        {
            LeftButton.SetActive(false);
        }

        RoomNameUpdate();
    }

    private void RoomNameUpdate() // �������� ��
    {
        int roomsnum = (currentpages - 1) * 4;

        for (int i = 0; i < 4; i++)
        {
            button[i].SetActive(false);
        }

        for (int i = 0; i < 4; i++)
        {
            if (roomsnum >= roomsCount)
            {
                break;
            }
            else
            {
                button[i].SetActive(true);
                buttonText[i].text = rooms[roomsnum];
                roomPerson[i].text = roomPop[roomsnum].ToString() + " / 2";
                roomsnum++;
            }
        }
    }

    public void LeftButtonPressed() // ���� ��ư
    {
        if (currentpages > 0)
        {
            currentpages--;
            pagestext.text = currentpages + " / " + pages;
        }
        RoomListUpdate();
    }

    public void RightButtonPressed() // ������ ��ư
    {
        if (currentpages < pages)
        {
            currentpages++;
            pagestext.text = currentpages + " / " + pages;
        }
        RoomListUpdate();
    }

    public void EnterRoomMake() // �� ����
    {
        if (canclick)
        {
            canclick = false;
            mainLobby.SetActive(false);
            roomMaker.SetActive(true);
        }
    }

    public void EnterRoom(int numb) // �� ����
    {
        if (canclick)
        {
            canclick = false;

            PhotonNetwork.JoinRoom(buttonText[numb].text);
        }
    }

    public override void OnJoinedRoom() // �� ���� �Ϸ�
    {
        mainLobby.SetActive(false);
        matchRoom.SetActive(true);

        rooms.Clear();
        roomPop.Clear();
        canclick = true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message) // �� ���� ���н�
    {
        //Debug.LogError("Failed to join room. Error Code: " + returnCode + ", Message: " + message);

        errorTextclone = Instantiate(errorText, messagepos.transform);
        errorTextclone.GetComponent<Text>().text = "�濡 �������� ���߽��ϴ�.";

        canclick = true;
    }

    public override void OnDisconnected(DisconnectCause cause) // ����� ���� �ߴܽ�
    {
        rooms.Clear();
        roomPop.Clear();
    }
}
