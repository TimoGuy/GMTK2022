using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorEvents : MonoBehaviour
{
    public UnityEvent disableMovement;
    public UnityEvent enableMovement;
    public UnityEvent[] attacks;
    public UnityEvent[] unAttacks;

    void DisableMovement() { disableMovement.Invoke(); }
    void EnableMovement() { enableMovement.Invoke(); }
    void EnableAttack(int index) { attacks[index].Invoke(); }
    void DisableAttack(int index) { unAttacks[index].Invoke(); }
}
