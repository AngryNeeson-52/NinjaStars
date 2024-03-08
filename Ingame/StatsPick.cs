using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsPick : MonoBehaviour
{
    GameCounter GC;

    [SerializeField]
    Text timeText;

    Coroutine selectCoroutine;
    WaitForSeconds waittime = new WaitForSeconds(1.0f);

    private void Awake()
    {
        GC = GameCounter.instance;
    }

    private void OnEnable()
    {
        if (selectCoroutine != null)
        {
            StopCoroutine(selectCoroutine);
        }
        selectCoroutine = StartCoroutine(SelectTime());
    }

    IEnumerator SelectTime()
    {
        for (int i = 12; i > -1; i--)
        {
            timeText.text = i.ToString();
            yield return waittime;
        }

        GC.RoundStart();
        gameObject.SetActive(false);
    }

}
