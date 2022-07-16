using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatRBMovement : MonoBehaviour
{
    //
    // @NOTE: these movement variables are camera direction agnostic
    //
    public float movementSpeed = 1.0f;
    public float movementMagnitude;
    public Vector3 movementDirection;       // @NOTE: if this isn't normalized, ya STUPID!  -Timo
    public Transform modelReference;        // @NOTE: this moves the model's facing direction for it.
    public Animator modelAnimator;

    private Rigidbody _rb;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void OnDisable()
    {
        _rb.velocity = new Vector3(0.0f, _rb.velocity.y, 0.0f);
    }


    void Update()
    {
        if (movementMagnitude < 0.01f)
            return;

        Vector3 rotation = modelReference.localEulerAngles;
        float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
        float distance = Mathf.Abs(Mathf.DeltaAngle(rotation.y, targetAngle));
        rotation.y = Mathf.MoveTowardsAngle(rotation.y, targetAngle, distance / 0.05f * Time.deltaTime);
        modelReference.localEulerAngles = rotation;
    }


    void FixedUpdate()
    {
        modelAnimator.SetFloat("Walk|Idle BT Param", (movementMagnitude < 0.01f ? 0.0f : 1.0f));

        // Vector3 flatVelocity = _rb.velocity;
        // flatVelocity.y = 0.0f;
        // Vector3 flatVelocityNormalized = flatVelocity.normalized;

        // if (movementMagnitude < 0.01f)
        // {
        //     _rb.AddForce(-flatVelocityNormalized * movementSpeed, ForceMode.Force);
        //     return;
        // }

        // if (Vector3.Dot(movementDirection, flatVelocityNormalized) < 0.0f)
        // {
        //     float flatVelocityMagnitude = flatVelocity.magnitude;
        //     _rb.velocity = movementDirection * flatVelocityMagnitude + new Vector3(0.0f, _rb.velocity.y, 0.0f);
        //     return;
        // }

        // Vector3 force = movementDirection * movementMagnitude * movementSpeed;
        // _rb.AddForce(force - flatVelocityNormalized * force.magnitude, ForceMode.Force);


        //
        // Just do really sharp movement!
        //
        Vector3 force = movementDirection * movementMagnitude * movementSpeed;
        _rb.velocity = force + new Vector3(0.0f, _rb.velocity.y, 0.0f);
    }

    private float fixDegrees(float degreesIn)
    {
        if (degreesIn <= -180.0f)
            degreesIn += 360.0f;
        if (degreesIn > 180.0f)
            degreesIn -= 360.0f;
        return degreesIn;
    }
}
