using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// �÷��� �� ���ŵǾ�� �� �÷��� ĳ���� �ڵ�

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    GameObject arm, chatPos, chatClone, chatText;
    [SerializeField]
    Animator animator;

    private Vector3 movePosition;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // ���� ��ȯ
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

    void Update() // �ε巯�� �̵�
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, movePosition, Time.fixedDeltaTime * 10);
        }
    }

    [PunRPC]
    void ShowChat(string chattext) // ä�� �Խ�
    {
        if (chatClone != null)
        {
            Destroy(chatClone);
        }

        chatClone = Instantiate(chatText, chatPos.transform);
        chatClone.GetComponentInChildren<Text>().text = chattext;
    }
}
