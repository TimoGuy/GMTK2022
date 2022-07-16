using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputToFlatRBMovement : MonoBehaviour
{
    public FlatRBMovement flatRBMovement;
    private Camera _mainCamera;


    void Start()
    {
        _mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        //
        // Joystick controller movement to camera direction velocity.
        // Copy Unreal Engine lol
        //
        Vector3 flatCameraForward = _mainCamera.transform.forward;
        flatCameraForward.y = 0.0f;
        flatCameraForward.Normalize();

        Vector3 flatCameraRight = _mainCamera.transform.right;
        flatCameraRight.y = 0.0f;
        flatCameraRight.Normalize();

        Vector2 controllerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        controllerInput = Vector2.ClampMagnitude(controllerInput, 1.0f);

        Vector3 movement = flatCameraForward * controllerInput.y + flatCameraRight * controllerInput.x;

        //
        // Insert values into the flatRBMovement
        //
        flatRBMovement.movementMagnitude = movement.magnitude;
        flatRBMovement.movementDirection = movement.normalized;
    }
}
