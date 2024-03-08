using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//비활성화시 삭제되는 오브젝트 (채팅내역)

public class EraseObject : MonoBehaviour
{
    private void OnDisable()
    {
        Destroy(this.gameObject);
    }
}
