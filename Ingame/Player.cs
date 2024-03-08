using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// 플레이 시 갱신되어야 할 플레이 캐릭터 코드

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    GameObject arm, chatPos, chatClone, chatText;
    [SerializeField]
    Animator animator;

    private Vector3 movePosition;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // 정보 교환
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(arm.transform.rotation);
            stream.SendNext(animator.GetBool("Moving"));
            stream.SendNext(animator.GetBool("Jumping"));
            stream.SendNext(animator.GetFloat("Dir"));
        }
        else
        {
            movePosition = (Vector3)stream.ReceiveNext();
            arm.transform.rotation = (Quaternion)stream.ReceiveNext();
            animator.SetBool("Moving", (bool)stream.ReceiveNext());
            animator.SetBool("Jumping", (bool)stream.ReceiveNext());
            animator.SetFloat("Dir", (float)stream.ReceiveNext());
        }
    }

    void Update() // 부드러운 이동
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, movePosition, Time.fixedDeltaTime * 10);
        }
    }

    [PunRPC]
    void ShowChat(string chattext) // 채팅 게시
    {
        if (chatClone != null)
        {
            Destroy(chatClone);
        }

        chatClone = Instantiate(chatText, chatPos.transform);
        chatClone.GetComponentInChildren<Text>().text = chattext;
    }
}
