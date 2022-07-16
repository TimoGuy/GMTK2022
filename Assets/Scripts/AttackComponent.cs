using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackComponent : MonoBehaviour
{
    public bool triggerAttack = false;
    public bool triggerChargedAttack = false;
    public Animator modelAnimator;
    public UnityEvent onAttackSuccess;
    

    // Update is called once per frame
    void Update()
    {
        // @FIXME: So if you attack and do a chargedattack at the same time, then player will be softlocked
        if (triggerChargedAttack)
        {
            modelAnimator.SetTrigger("doChargedAttack");
        }
        else if (triggerAttack)
        {
            modelAnimator.SetTrigger("doAttack");
        }

        triggerChargedAttack = false;
        triggerAttack = false;
    }

    public void OnAttackSuccess()
    {
        onAttackSuccess.Invoke();
    }
}
