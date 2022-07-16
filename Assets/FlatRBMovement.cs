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
    public Vector3 movementDirection;

    private Rigidbody _rb;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        _rb.AddForce(movementDirection * movementMagnitude * movementSpeed, ForceMode.Force);
    }
}
