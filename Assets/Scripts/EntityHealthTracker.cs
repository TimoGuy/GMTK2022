using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthTracker : MonoBehaviour
{
    public int health = 100;
    public float waitAfterLimpSeconds = 0.5f;

    public void ChangeHealthRelative(int amountRelative)
    {
        health += amountRelative;

        CheckIfLimp();
    }

    public void CheckIfLimp()
    {
        if (health > 0)
            return;

        var _rb = GetComponent<Rigidbody>();
        _rb.mass *= 0.5f;
        _rb.useGravity = false;
        _rb.maxAngularVelocity = 200.0f;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.angularVelocity = new Vector3(0.0f, 200.0f, 0.0f);

        foreach (var coll in GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }

        StartCoroutine(DestroyTimerCoroutine());
    }

    IEnumerator DestroyTimerCoroutine()
    {
        yield return new WaitForSeconds(waitAfterLimpSeconds);
        Destroy(gameObject);
    }
}
