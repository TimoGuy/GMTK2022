using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityKnockback : MonoBehaviour
{
    public ReceiveAttackHitbox receiveAttackHitbox;
    public MonoBehaviour[] behaviorsToDisable;
    public float timeToDisableInput = 1.0f;

    private Coroutine _disableInputTempCoroutine = null;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void ReceiveKnockbackFrom(float force)
    {
        Vector3 flatDirection = transform.position - receiveAttackHitbox._previousHitPosition;
        flatDirection.y = 0.0f;
        flatDirection.Normalize();
        _rb.AddForce(flatDirection * force, ForceMode.Impulse);

        if (_disableInputTempCoroutine != null)
        {
            StopCoroutine(_disableInputTempCoroutine);
            _disableInputTempCoroutine = null;
        }
        _disableInputTempCoroutine = StartCoroutine(DisableInputTemporarilyCoroutine());
    }

    IEnumerator DisableInputTemporarilyCoroutine()
    {
        foreach (var beh in behaviorsToDisable)
        {
            beh.enabled = false;
        }

        yield return new WaitForSeconds(timeToDisableInput);

        foreach (var beh in behaviorsToDisable)
        {
            beh.enabled = true;
        }
    }
}
