using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponFunctions : MonoBehaviour
{
    [SerializeField]
    private Animator currentAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void swingComplete()
    {
        currentAnimator.SetBool("isAttacking", false);
    }
}
