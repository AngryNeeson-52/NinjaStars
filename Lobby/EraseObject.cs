using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//��Ȱ��ȭ�� �����Ǵ� ������Ʈ (ä�ó���)

public class EraseObject : MonoBehaviour
{
    private void OnDisable()
    {
        Destroy(this.gameObject);
    }
}
