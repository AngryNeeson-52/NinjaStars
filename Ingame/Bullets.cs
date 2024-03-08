using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 총알 코드

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

    void Update() // 부드러운 이동
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, movePosition, Time.fixedDeltaTime * 10);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // 피격시 불렛 풀로 이동
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

    public void BulletSleep() // 비활성화 RPC 호출
    {
        photonView.RPC("BulletSetSleep", RpcTarget.All);
    }
    [PunRPC]
    void BulletSetSleep() // 비활성화
    {
        rb.AddForce(-rb.velocity);
        gameObject.SetActive(false);
    }

    public void BulletActive(float atk, Vector3 startpos, Quaternion startrot, Vector3 shotDir, float bulletSpeed) // 활성화, 세팅 RPC 호출
    {
        photonView.RPC("BulletSetActive", RpcTarget.All, atk, startpos, startrot, shotDir, bulletSpeed);
    }
    [PunRPC]
    void BulletSetActive(float atk, Vector3 startpos, Quaternion startrot, Vector3 shotDir, float bulletSpeed) // 활성화. 세팅
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
