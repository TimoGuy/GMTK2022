using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReceiveAttackHitbox : MonoBehaviour
{
    public string allowedTag = "<TAG>";
    public UnityEvent onHitboxDetect;
    public Vector3 _previousHitPosition;
    public float debounce = 0.25f;
    private float _debounceTimer = 0.0f;


    void Update()
    {
        _debounceTimer -= Time.deltaTime;
    }


    void OnTriggerStay(Collider collider)
    {
        if (_debounceTimer > 0.0f)
            return;

        if (!collider.gameObject.CompareTag(allowedTag))
            return;

        _debounceTimer = debounce;
        collider.gameObject.SendMessageUpwards("OnAttackSuccess");

        _previousHitPosition = collider.transform.position;
        onHitboxDetect.Invoke();
    }
}
