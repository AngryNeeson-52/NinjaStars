using Newtonsoft.Json.Bson;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// 주인이 아닐경우 비활성화 할 플레이 관련 코드

public class PlayerControl : MonoBehaviourPunCallbacks
{
    GameCounter GC;
    BulletPool BP;

    [SerializeField]
    Text namae;
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    Animator animator;
    [SerializeField]
    GameObject arm, gun;
    [SerializeField]
    Image HPbar;
    [SerializeField]
    bool canJump = true, jumpCool = true, isFocused = false;

    WaitForSeconds waittime = new WaitForSeconds(0.1f);
    WaitForSeconds waittime2 = new WaitForSeconds(0.5f);

    void Awake() // 기본 설정
    {
        if (photonView.IsMine)
        {
            namae.text = PhotonNetwork.LocalPlayer.NickName;
            namae.color = Color.green;
        }
        else
        {
            namae.text = photonView.Owner.NickName;
            namae.color = Color.red;
        }
    }

    private void Start() // 소유가 아닐 시 비활성화
    {
        GC = GameCounter.instance;
        BP = BulletPool.bulletinstance;

        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        //이동
        if (GC.canMove)
        {
            float horizontal = Input.GetAxis("Horizontal");

            if (horizontal == 0)
            {
                animator.SetBool("Moving", false);
            }
            else 
            {
                animator.SetBool("Moving", true);
                animator.SetFloat("Dir", horizontal);
            }

            Vector2 move = new Vector2(horizontal, 0);
            transform.Translate(move * GC.speed * Time.deltaTime);
        }
        
        // 조준
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arm.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 총알 발사 처리
        if (Input.GetMouseButtonDown(0) && GC.canFire)
        {
            GC.canFire = false;
            StartCoroutine(FireCoroutine());

            photonView.RPC("BulletFire", RpcTarget.Others, 
                GC.atk, gun.transform.position, gun.transform.rotation, 
                gun.transform.right, GC.bulletSpeed);
        }

        // 채팅 처리
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!isFocused)
            {
                GC.canMove = false;
                GC.inputField.gameObject.SetActive(true);
                GC.inputField.ActivateInputField();
                GC.inputField.Select();
                isFocused = true;
            }
            else
            {
                if (GC.inputField.text != "")
                {
                    photonView.RPC("ShowChat", RpcTarget.All, GC.inputField.text);
                }
                GC.inputField.text = "";

                GC.inputField.DeactivateInputField();
                GC.inputField.gameObject.SetActive(false);
                isFocused = false;
                GC.canMove = true;
            }
        }
    }

    IEnumerator FireCoroutine() // 총 연사 속도
    {
        for (int i = 0; i < GC.teatime; i++)
        {
            yield return waittime;
        }

        if (GC.canRoundOver)
        {
            GC.canFire = true;
        }
    }

    [PunRPC]
    void BulletFire(float atk, Vector3 gunpos, Quaternion gunrot, 
        Vector3 gundir, float bulletspeed) // 총알 발사
    {
        GameObject bullet = BP.GetBullet();
        bullet.GetComponent<Bullets>().BulletActive(
            atk, gunpos, gunrot, gundir, bulletspeed);
    }

    void FixedUpdate()
    {
        //점프
        if (GC.canMove && canJump && jumpCool && Input.GetKey(KeyCode.Space))
        {
            canJump = false;
            animator.SetBool("Jumping", true);

            jumpCool = false;
            StartCoroutine(JumpCoolCoroutine());

            rb.AddForce(Vector2.up * (GC.speed + 30), ForceMode2D.Impulse);
        }
        
        // 급강하
        float vertical = Input.GetAxisRaw("Vertical");
        if (vertical < 0 && !canJump)
        {
            rb.AddForce(-rb.velocity);
            rb.AddForce(Vector2.down * (GC.speed + 10), ForceMode2D.Impulse);
        }
    }

    IEnumerator JumpCoolCoroutine() // 무한 연속 점프 조절
    {
        yield return waittime2;
        jumpCool = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true;
            animator.SetBool("Jumping", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = false;
            animator.SetBool("Jumping", true);
        }
    }


    private void GetHit(float damage) // 체력 변동 RPC 호출
    {
        photonView.RPC("HPChange", RpcTarget.Others, damage);
    }
    [PunRPC]
    void HPChange(float damage) // 체력 변동
    {
        GC.BulletHitplayer();
        GC.playerHP -= damage;

        photonView.RPC("HPBarChange", RpcTarget.All, GC.playerHP, GC.playerMaxHP);
    }
    [PunRPC]
    void HPBarChange(float HP, float maxHP) // 체력바 변동
    {
        HPbar.fillAmount = HP / maxHP;

        if (HP <= 0 && GC.canRoundOver)
        {
            GC.canRoundOver = false;
            GC.RoundCalc(photonView.Owner.ActorNumber);
        }
    }
    public void HPFULL() // 체력바 완충 RPC 호출
    {
        photonView.RPC("HPBarChange", RpcTarget.All, 1.0f, 1.0f);
    }
}
