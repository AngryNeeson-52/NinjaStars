using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 기본 화면 및 윈도우 설정 관련 코드

public class WindowSetting : MonoBehaviour
{
    [SerializeField]
    GameObject gamestart, lobby, setting, login;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
    }

    private void Start() // 로비 씬 시작 시 최초 시작인지 게임 종료 후 합류인지 판단
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

    public void WindowRate(Dropdown rate) // 화면 해상도 설정
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

    public void GoEnter() // 시작 화면 진입
    {
        gamestart.SetActive(true);

        setting.SetActive(false);
        login.SetActive(false);
    }

    public void GoSetting() // 세팅 화면 진입
    {
        setting.SetActive(true);

        gamestart.SetActive(false);
    }

    public void GoLogin() // 로그인 화면 진입
    {
        login.SetActive(true);

        gamestart.SetActive(false);
    }

    public void GameClose() // 게임 종료
    {
        Application.Quit();
    }
}
