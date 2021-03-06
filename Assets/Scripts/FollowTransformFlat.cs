using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformFlat : MonoBehaviour
{
    public Transform followTransform;
    public float maxSpeed = 100.0f;

    private Vector2 _capturedOffset;


    void Start()
    {
        if (followTransform != null)
        {
            Vector2 flatFrom = new Vector2(transform.position.x, transform.position.z);
            Vector2 flatTo = new Vector2(followTransform.position.x, followTransform.position.z);
            _capturedOffset = flatTo - flatFrom;
        }
    }

    void Update()
    {
        if (followTransform != null)
        {
            Vector2 flatFrom = new Vector2(transform.position.x, transform.position.z);
            Vector2 flatTo = new Vector2(followTransform.position.x, followTransform.position.z) - _capturedOffset;
            flatFrom = Vector2.Lerp(flatFrom, flatTo, maxSpeed * Time.deltaTime);
            transform.position = new Vector3(flatFrom.x, transform.position.y, flatFrom.y);
        }

        // @NOTE: this is in every scene... so we're gonna use this!
        if (Input.GetButtonDown("Pause"))
        {
            Application.Quit();
        }
    }
}
