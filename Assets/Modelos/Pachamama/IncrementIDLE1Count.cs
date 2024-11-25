using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementIDLE1Count : StateMachineBehaviour
{
   
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int currentCount = animator.GetInteger("Animacion1Count");
        animator.SetInteger("Animacion1Count", currentCount + 1);
    }
}

