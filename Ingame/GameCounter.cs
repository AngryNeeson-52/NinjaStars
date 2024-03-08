using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCounter : MonoBehaviourPunCallbacks
{
    public static GameCounter instance;

    [SerializeField]
    GameObject P1pos, P2pos, winimage, loseimage, roundWin, roundLose,
        startimage, lostconnect, statsPick, statsPickWait, playerObject;
    [SerializeField]
    GameObject[] ingameops;
    [SerializeField]
    Text leftscore, rightscore;
    [SerializeField]
    int myround = 0, enemyround = 0, endround = 3;
    

    public InputField inputField;

    public bool canRoundOver = true, canMove = true, gameOver = false, canFire = true;

    public int playernum, teatime;

    public float playerMaxHP, playerHP, atk, speed, bulletSpeed;
    
    Coroutine hurtCoroutine;

    WaitForSeconds waittime = new WaitForSeconds(0.4f);

    void Awake()
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

        StartCoroutine(ImageShow(startimage, 3.0f));

        if (PhotonNetwork.IsMasterClient)
        {
            playerObject = PhotonNetwork.Instantiate("Player", P1pos.transform.position, P1pos.transform.rotation);
        }
        else
        {
            playerObject = PhotonNetwork.Instantiate("Player2", P2pos.transform.position, P2pos.transform.rotation);
        }
        playerObject.GetComponent<Rigidbody2D>().gravityScale = 5;
    }

    private void Start()
    {
        playernum = PhotonNetwork.LocalPlayer.ActorNumber;
        if (GameManager.rounds == "7")
        {
            endround = 7;
        }
        else if (GameManager.rounds == "5")
        {
            endround = 5;
        }
        else 
        {
            endround = 3;
        }
    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void BulletHitplayer() // 개인 피격 효과
    {
        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
        }
        hurtCoroutine = StartCoroutine(HurtDelay());
    }

    IEnumerator HurtDelay() // 피격시 경직
    {
        canMove = false;
        yield return waittime;
        canMove = true;
    }

    // 스텟 향상
    #region
    public void ATKUP() // 공격력
    {
        atk *= 1.5f;
        bulletSpeed -= 50;

        statsPick.SetActive(false);
        RoundStart();
    }

    public void SpeedUP() // 이동속도
    {
        speed += 10;
        playerMaxHP *= 0.6f;

        statsPick.SetActive(false);
        RoundStart();
    }

    public void BulletSpeedUP() // 탄속
    {
        bulletSpeed += 100;
        atk *= 0.6f;

        statsPick.SetActive(false);
        RoundStart();
    }

    public void HPUP() // 체력
    {
        playerMaxHP *= 1.5f;

        statsPick.SetActive(false);
        RoundStart();
    }

    public void ATKrateUP() // 공격 속도 향상
    {
        teatime -= 2;
        bulletSpeed -= 50;
        atk *= 0.7f;

        statsPick.SetActive(false);
        RoundStart();
    }
    #endregion

    public void RoundCalc(int loser) // 라운드 계산
    {
        canFire = false;
        canMove = false;

        if (loser == playernum)
        {
            StartCoroutine(ImageShow(roundLose, 2.0f));
            enemyround++;
            statsPick.SetActive(true);
        }
        else
        {
            StartCoroutine(ImageShow(roundWin, 2.0f));
            myround++;
            statsPickWait.SetActive(true);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            playerObject.transform.position = P1pos.transform.position;
            rightscore.text = enemyround.ToString();
            leftscore.text = myround.ToString();
        }
        else
        {
            playerObject.transform.position = P2pos.transform.position;
            rightscore.text = myround.ToString();
            leftscore.text = enemyround.ToString();
        }

        if (enemyround >= endround)
        {
            loseimage.SetActive(true);
        }
        else if (myround >= endround)
        {
            winimage.SetActive(true);
        }
    }

    IEnumerator ImageShow(GameObject showImage, float showtime) // 이미지 띄우기
    {
        showImage.SetActive(true);
        yield return new WaitForSeconds(showtime);
        showImage.SetActive(false);
    }

    public void RoundStart() // 라운드 리셋 RPC 호출
    {
        if (!gameOver)
        {
            photonView.RPC("RoundReset", RpcTarget.All);
        }
    }
    [PunRPC]
    void RoundReset() // 라운드 리셋
    {
        for (int i = 0; i < ingameops.Length; i++)
        {
            ingameops[i].SetActive(true);
            ingameops[i].GetComponent<BreakableWall>().Reset();
        }

        statsPickWait.SetActive(false);
        playerObject.GetComponent<PlayerControl>().HPFULL();
        playerHP = playerMaxHP;
        canRoundOver = true;
        canFire = true;
        canMove = true;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // 누군가 나갔을경우
    {
        lostconnect.SetActive(true);
    }

    public void GameEnd() // 게임이 끝남
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
}
