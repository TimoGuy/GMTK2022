using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public FlatRBMovement flatRBMovement;
    public AttackComponent attackComponent;
    public GameManagerScript gameManagerScript;
    private Camera _mainCamera;


    void Start()
    {
        _mainCamera = Camera.main;
    }

    void OnDestroy()
    {
        gameManagerScript.ReportPlayerDied();
    }

    void Update()
    {
        attackComponent.triggerAttack = Input.GetButtonDown("Fire1");
        attackComponent.triggerChargedAttack = Input.GetButtonDown("Fire2");
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

        Vector2 controllerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (controllerInput.sqrMagnitude < 0.5f * 0.5f)
        {
            controllerInput = Vector2.zero;
        }
        else
        {
            controllerInput.Normalize();
        }

        Vector3 movement = flatCameraForward * controllerInput.y + flatCameraRight * controllerInput.x;

        //
        // Insert values into the flatRBMovement
        //
        flatRBMovement.SendMovement(movement.normalized, movement.magnitude);
    }
}
