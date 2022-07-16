using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorEvents : MonoBehaviour
{
    public UnityEvent disableMovement;
    public UnityEvent enableMovement;

    void DisableMovement() { disableMovement.Invoke(); }
    void EnableMovement() { enableMovement.Invoke(); }
}
