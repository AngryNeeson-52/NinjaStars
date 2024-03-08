using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//총알에 부서지는 오브젝트

public class BreakableWall : MonoBehaviourPunCallbacks
{
    [SerializeField]
    int maxStack;

    private int stack = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
        {
            photonView.RPC("WallDamage", RpcTarget.All);
        }
    }

    [PunRPC]
    void WallDamage()
    {
        stack++;

        if (stack >= maxStack)
        {
            stack = 0;
            gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        stack = 0;
    }
}
