using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �⺻ ȭ�� �� ������ ���� ���� �ڵ�

public class WindowSetting : MonoBehaviour
{
    [SerializeField]
    GameObject gamestart, lobby, setting, login;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
    }

    private void Start() // �κ� �� ���� �� ���� �������� ���� ���� �� �շ����� �Ǵ�
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            lobby.SetActive(true);
        }
        else
        {
            gamestart.SetActive(true);
            Application.runInBackground = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }

    public void WindowRate(Dropdown rate) // ȭ�� �ػ� ����
    {
        if (rate.value == 1)
        {
            Screen.SetResolution(1280, 720, false);
        }
        else if (rate.value == 2)
        {
            Screen.SetResolution(1920, 1080, false);
        }
        else if (rate.value == 3)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Screen.SetResolution(960, 540, false);
        }
    }

    public void GoEnter() // ���� ȭ�� ����
    {
        gamestart.SetActive(true);

        setting.SetActive(false);
        login.SetActive(false);
    }

    public void GoSetting() // ���� ȭ�� ����
    {
        setting.SetActive(true);

        gamestart.SetActive(false);
    }

    public void GoLogin() // �α��� ȭ�� ����
    {
        login.SetActive(true);

        gamestart.SetActive(false);
    }

    public void GameClose() // ���� ����
    {
        Application.Quit();
    }
}
