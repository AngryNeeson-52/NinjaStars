using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EraseObject : MonoBehaviour
{
    private void OnDisable()
    {
        Destroy(this.gameObject);
    }

    public void RoomEnter(Text roomname)
    {
        PhotonNetwork.JoinRoom(roomname.text);
    }
}
