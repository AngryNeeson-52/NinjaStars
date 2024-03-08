using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

// �κ� ����

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text serverpopulation, welcome;
    [SerializeField]
    GameObject login;

    WaitForSeconds waittime = new WaitForSeconds(5.0f);

    public override void OnEnable() // �κ� ����� ȣ��
    {
        PhotonNetwork.AddCallbackTarget(this);

        welcome.text = PhotonNetwork.LocalPlayer.NickName + "�� ȯ���մϴ�.";
        StartCoroutine(PopulationUpdate());
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    IEnumerator PopulationUpdate()  // �κ� �ο� ���� (������ ���� �ο� ���� �ֱⰡ 5��)
    {
        while (true)
        {
            serverpopulation.text = "���� " + PhotonNetwork.CountOfPlayers.ToString() + "�� ������...";
            yield return waittime;
        }
    }

    public override void OnDisconnected(DisconnectCause cause) // ����� ���� �ߴܽ�
    {
        StopAllCoroutines();
        login.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
