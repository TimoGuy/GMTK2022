using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTextBlowup : MonoBehaviour
{
    public string text = "<HERE>";
    private TextMesh myTextUI;

    void Start()
    {
        myTextUI = GetComponent<TextMesh>();
        myTextUI.text = text;
    }


    void Update()
    {
        transform.localScale *= 4.0f * Time.deltaTime;
        var colR = myTextUI.color;
        colR.a -= 1.0f * Time.deltaTime;
        myTextUI.color = colR;

        if (colR.a < 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
