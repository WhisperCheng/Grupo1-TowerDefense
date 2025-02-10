using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeWalkCycleOffset : StateMachineBehaviour
{
    bool _hasRandomized;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_hasRandomized)
        {
            float offset = Random.Range(0f, 1);
            animator.SetFloat("WalkingOffset", offset);
            _hasRandomized = true;
        }
    }
}
