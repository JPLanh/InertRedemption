using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackBehavior : MonoBehaviour
{
    [SerializeField]
    private Animator attackAnimator;

    void attackFinish()
    {

        attackAnimator.SetBool("isAttacking", false);
    }
}
