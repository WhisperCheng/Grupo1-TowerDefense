using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIDLE1Count : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("Animacion1Count", 0);
    }
}
