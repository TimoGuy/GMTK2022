using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUntilDestroy : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(30.0f);
        Destroy(gameObject);
    }
}
