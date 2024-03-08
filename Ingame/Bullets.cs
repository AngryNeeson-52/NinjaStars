using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ѿ� �ڵ�

public class Bullets : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    CircleCollider2D cc2d;

    private float damage = 0;
    Vector3 movePosition;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
           movePosition = (Vector3)stream.ReceiveNext();
           //transform.position = (Vector3)stream.ReceiveNext();
        }
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            rb.gravityScale = 0;
        }
    }

    void Update() // �ε巯�� �̵�
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, movePosition, Time.fixedDeltaTime * 10);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // �ǰݽ� �ҷ� Ǯ�� �̵�
    {
        if (photonView.IsMine)
        {
            sr.enabled = false;
            cc2d.enabled = false;
        }
        else
        {
            collision.SendMessage("GetHit", damage, SendMessageOptions.DontRequireReceiver);
            photonView.RPC("BulletSetSleep", RpcTarget.All);
        }
    }

    public void BulletSleep() // ��Ȱ��ȭ RPC ȣ��
    {
        photonView.RPC("BulletSetSleep", RpcTarget.All);
    }
    [PunRPC]
    void BulletSetSleep() // ��Ȱ��ȭ
    {
        rb.AddForce(-rb.velocity);
        gameObject.SetActive(false);
    }

    public void BulletActive(float atk, Vector3 startpos, Quaternion startrot, Vector3 shotDir, float bulletSpeed) // Ȱ��ȭ, ���� RPC ȣ��
    {
        photonView.RPC("BulletSetActive", RpcTarget.All, atk, startpos, startrot, shotDir, bulletSpeed);
    }
    [PunRPC]
    void BulletSetActive(float atk, Vector3 startpos, Quaternion startrot, Vector3 shotDir, float bulletSpeed) // Ȱ��ȭ. ����
    {
        damage = atk;
        sr.enabled = true;
        cc2d.enabled = true;
        transform.position = startpos;
        movePosition = startpos;
        transform.rotation = startrot;
        gameObject.SetActive(true);

        if (photonView.IsMine)
        {
            rb.AddForce(shotDir * bulletSpeed, ForceMode2D.Impulse);
        }
    }
}
