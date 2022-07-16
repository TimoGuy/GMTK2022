using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public bool triggerAttack = false;
    public bool triggerChargedAttack = false;
    public Animator modelAnimator;
    

    // Update is called once per frame
    void Update()
    {
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
}
