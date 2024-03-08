using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

// 로비 관리

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text serverpopulation, welcome;
    [SerializeField]
    GameObject login;

    WaitForSeconds waittime = new WaitForSeconds(5.0f);

    public override void OnEnable() // 로비 입장시 호출
    {
        PhotonNetwork.AddCallbackTarget(this);

        welcome.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다.";
        StartCoroutine(PopulationUpdate());
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    IEnumerator PopulationUpdate()  // 로비 인원 갱신 (마스터 서버 인원 갱신 주기가 5초)
    {
        while (true)
        {
            serverpopulation.text = "현재 " + PhotonNetwork.CountOfPlayers.ToString() + "명 접속중...";
            yield return waittime;
        }
    }

    public override void OnDisconnected(DisconnectCause cause) // 포톤과 연결 중단시
    {
        StopAllCoroutines();
        login.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
